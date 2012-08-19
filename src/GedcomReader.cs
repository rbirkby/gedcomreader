// Copyright (c) ThunderMain Ltd. All rights reserved. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace ThunderMain.GedcomReader
{
    /// <summary>
    /// GEDML only has two attribute types - a REF type which references
    /// other elements - ie the FAMC GEDCOM tag, or the ID type which defines
    /// the identifier for the GEDCOM tag
    /// </summary>
    internal enum GedcomAttributeType { Ref, Id };

    /// <summary>
    /// Provides an implementation of XmlReader for reading GEDCOM files
    /// </summary>
    public class GedcomReader : DefaultedXmlReader, IXmlLineInfo
    {
        private bool _attributeMode;
        private readonly Scanner _scanner;
        private readonly Stack<char[]> _nodeStack = new Stack<char[]>(5); // 5 nodes deep seems reasonable

        // Queue holds a collection of TokenInfo objects
        // The tokens for a typical line might be:
        // <LEVEL><WS><TAG><WS><IDREF><WS><LINE VALUE>
        // So we'll set the initial size to be 7 to hold these
        private readonly Queue<TokenInfo> _nodeQueue = new Queue<TokenInfo>(7);

        private TokenInfo _currentNode = new TokenInfo(XmlNodeType.None, new char[] { });

        private int _currentAttributeIndex = -1; // Pointer into _attributes - only valid when _attributeMode==true
        private readonly List<GedcomAttribute> _attributes = new List<GedcomAttribute>(1);
        private bool _attributeRead; // true if ReadAttributeValue has been called on this attribute

        private sealed class TokenInfo
        {
            public readonly XmlNodeType Type;
            private readonly char[] _lexeme;

            public TokenInfo(XmlNodeType type, char[] lexeme)
            {
                Type = type;
                _lexeme = lexeme;
            }

            public override string ToString()
            {
                return Type.ToString() + "='" + Lexeme + "'";
            }

            public char[] Lexeme
            {
                get { return _lexeme; }
            }
        }

        // Note: These line number properties are slightly out, because we read
        // a line of tokens in advance of returning the token.

        /// <summary>
        /// Returns the number of the current line
        /// </summary>
        public int LineNumber
        {
            get { return _scanner.LineNumber; }
        }

        /// <summary>
        /// Returns the current column position along a line
        /// </summary>
        public int LinePosition
        {
            get { return _scanner.LinePosition; }
        }

        /// <summary>
        /// Returns true to indicate we supply line information
        /// </summary>
        /// <returns></returns>
        public bool HasLineInfo()
        {
            return true;
        }

        /// <summary>
        /// Creates a new GEDCOM Reader to read from a stream
        /// </summary>
        /// <param name="stream"></param>
        public GedcomReader(Stream stream)
        {
            _scanner = new Scanner(stream, false);
        }

        /// <summary>
        /// Gets the <see cref="T:System.Xml.XmlNameTable" /> associated with this implementation.
        /// </summary>
        /// <returns>The XmlNameTable enabling you to get the atomized version of a string within the node.</returns>
        public override XmlNameTable NameTable
        {
            get { return _scanner.NameTable; }
        }

        /// <summary>
        /// parses the attribute value into one or more Text, EntityReference, or EndEntity nodes.
        /// </summary>
        /// <returns>
        /// true if there are nodes to return.false if the reader is not positioned on an attribute node when the initial call is made or if all the attribute values have been read.An empty attribute, such as, misc="", returns true with a single node with a value of String.Empty.
        /// </returns>
        public override bool ReadAttributeValue()
        {
            // Has the attribute already been read?
            if (_attributeRead)
            {
                return false;
            }

            _attributeRead = true;
            return true;
        }

        /// <summary>
        /// Closes the reader
        /// </summary>
        public override void Close()
        {
            _scanner.Close();
        }

        /// <summary>
        /// Returns a human readable string representation of the current node
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Token=" + _scanner.ToString() + ", Depth=" + (_nodeStack.Count - 1);
        }

#if DEBUG
        private void DumpQueue()
        {
            TokenInfo[] nodes = _nodeQueue.ToArray();

            Console.WriteLine("Queue Dump -----------------------------");

            for (int i = 0; i <= nodes.GetUpperBound(0); i++)
            {
                Console.WriteLine(nodes[i].Type.ToString());
            }

            Console.WriteLine("----------------------------------------");
        }
#endif

        /// <summary>
        /// Peeks the item at the end of the queue
        /// </summary>
        /// <param name="queue"></param>
        /// <returns>The object at the end of the queue</returns>
        private TokenInfo SneakyPeek(Queue<TokenInfo> queue)
        {
            TokenInfo[] array = queue.ToArray();

            if (array.GetUpperBound(0) == -1)
            {
                // Queue is empty, so return a null TokenInfo object
                return new TokenInfo(XmlNodeType.None, new char[] { });
            }

            return array[array.GetUpperBound(0)];
        }

        /// <summary>
        /// Reads a line of tokens and places the corresponding XML nodes into a queue
        /// 
        /// Good candidate for performance improvements
        /// GEDCOMReader is actually faster than XmlReader. However, it allocates more strings
        /// and these are objects, so the GC has to do more work. - Since the char[] rewrite, this may 
        /// no longer be true
        /// </summary>
        private void ReadLine()
        {
            _attributes.Clear();

            while (true)
            {
                _scanner.NextToken();

                switch (_scanner.CurrentToken)
                {
                    case Scanner.Level:
                        Debug.Assert(_nodeQueue.Count == 0, "Unexpected level token");

                        int depth = Convert.ToInt32(new string(_scanner.Lexeme)) + 2;

                        while (depth <= _nodeStack.Count)
                        {
                            // Inject EndTag nodes into the queue
                            _nodeQueue.Enqueue(new TokenInfo(XmlNodeType.EndElement, _nodeStack.Pop()));
                        }
                        break;
                    case Scanner.Tag:
                        // add node to queue
                        // don't know whether next token is text or xref
                        _nodeQueue.Enqueue(new TokenInfo(XmlNodeType.Element, _scanner.Lexeme)); //_scanner.LexemeNT));
                        break;
                    case Scanner.Xref:
                        // add node to attribute collection
                        switch (SneakyPeek(_nodeQueue).Type)
                        {
                            case XmlNodeType.Element:
                                _attributes.Add(new GedcomAttribute(GedcomAttributeType.Ref, _scanner.Lexeme));
                                break;

                            case XmlNodeType.None:
                            case XmlNodeType.EndElement:
                                _attributes.Add(new GedcomAttribute(GedcomAttributeType.Id, _scanner.Lexeme));
                                break;
                        }

                        break;
                    case Scanner.Text:
                        // add node to queue
                        _nodeQueue.Enqueue(new TokenInfo(XmlNodeType.Text, _scanner.Lexeme));
                        break;
                    case Scanner.Escape:
                        // add node to queue
                        _nodeQueue.Enqueue(new TokenInfo(XmlNodeType.ProcessingInstruction, _scanner.Lexeme)); //_scanner.LexemeNT));
                        break;
                    case Scanner.Eol:
                        return;
                    case Scanner.Eof:
                        while (_nodeStack.Count != 0)
                        {
                            // Inject EndTag nodes into the queue
                            _nodeQueue.Enqueue(new TokenInfo(XmlNodeType.EndElement, _nodeStack.Pop()));
                        }
                        return;
                    default:
                        throw new GedcomException("UNEXPECTED TOKEN '" + _scanner.Lexeme + "'", _scanner.Lexeme[0], LinePosition, LineNumber);
                }
            }
        }

        /// <summary>
        /// Reads the next node from the stream.
        /// </summary>
        /// <returns>
        /// true if the next node was read successfully; false if there are no more nodes to read.
        /// </returns>
        public override bool Read()
        {
            // if more nodes to pull off queue, then pull them off and push onto the stack
            // return. Otherwise, need more nodes

            _attributeMode = false;

            // Is this the first call to Read() ???
            if (_nodeStack.Count == 0 && _scanner.CurrentToken == Scanner.None)
            {
                //Inject the document element
                _nodeStack.Push("GED".ToCharArray());
                _currentNode = new TokenInfo(XmlNodeType.Element, "GED".ToCharArray()); //NameTable.Add("GED"));

                return true;
            }

            if (_nodeQueue.Count == 0)
                ReadLine();

            if (_nodeQueue.Count == 0)
            {
                return false;
            }

            _currentNode = _nodeQueue.Dequeue();

            if (_currentNode.Type == XmlNodeType.Element)
            {
                _nodeStack.Push(_currentNode.Lexeme);
            }

            return true;
        }

        /// <summary>
        /// Moves to the element that contains the current attribute node.
        /// </summary>
        /// <returns>
        /// true if the reader is positioned on an attribute (the reader moves to the element that owns the attribute); false if the reader is not positioned on an attribute (the position of the reader does not change).
        /// </returns>
        public override bool MoveToElement()
        {
            _attributeMode = false;
            return true;
        }

        /// <summary>
        /// Moves to the next attribute.
        /// </summary>
        /// <returns>
        /// true if there is a next attribute; false if there are no more attributes.
        /// </returns>
        public override bool MoveToNextAttribute()
        {
            if (_attributeMode == false)
            {
                return MoveToFirstAttribute();
            }

            _attributeMode = false;
            return false; // Only 1 attribute allowed in GEDCOM
        }

        /// <summary>
        /// Moves to the first attribute.
        /// </summary>
        /// <returns>
        /// true if an attribute exists (the reader moves to the first attribute); otherwise, false (the position of the reader does not change).
        /// </returns>
        public override bool MoveToFirstAttribute()
        {
            if (_attributes.Count > 0 && _attributeMode == false)
            {
                _attributeMode = true;
                _attributeRead = false;
                _currentAttributeIndex = 0;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Moves to the attribute with the specified <see cref="P:System.Xml.XmlReader.Name" />.
        /// </summary>
        /// <param name="i"></param>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public override void MoveToAttribute(int i)
        {
            if (i == 0 && AttributeCount == 1)
            {
                MoveToFirstAttribute();
            }
            else
            {
                throw new IndexOutOfRangeException("GEDCOM only has 1 attribute");
            }
        }

        /// <summary>
        /// Moves to the attribute with the specified <see cref="P:System.Xml.XmlReader.LocalName" /> and <see cref="P:System.Xml.XmlReader.NamespaceURI" />.
        /// </summary>
        /// <param name="localName"></param>
        /// <param name="namespaceUri"></param>
        /// <returns>
        /// true if the attribute is found; otherwise, false. If false, the reader's position does not change.
        /// </returns>
        public override bool MoveToAttribute(string localName, string namespaceUri)
        {
            return MoveToAttribute(localName);
        }

        /// <summary>
        /// Moves to the attribute with the specified <see cref="P:System.Xml.XmlReader.Name" />.
        /// </summary>
        /// <param name="name">The qualified name of the attribute.</param>
        /// <returns>
        /// true if the attribute is found; otherwise, false. If false, the reader's position does not change.
        /// </returns>
        public override bool MoveToAttribute(string name)
        {
            for (int i = 0; i < _attributes.Count; i++)
            {
                if ((_attributes[i]).Name.Equals(LocalName))
                {
                    _attributeMode = true;
                    _attributeRead = false;
                    _currentAttributeIndex = i;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the value of the attribute with the specified <see cref="P:System.Xml.XmlReader.Name" />.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>
        /// The value of the specified attribute. If the attribute is not found or the value is String.Empty, null is returned.
        /// </returns>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public override string GetAttribute(int i)
        {
            if (i == 0 && AttributeCount == 1)
            {
                return (_attributes[i]).Value;
            }

            throw new IndexOutOfRangeException("GEDCOM only has 1 attribute");
        }

        /// <summary>
        /// Gets the value of the attribute with the specified <see cref="P:System.Xml.XmlReader.LocalName" /> and <see cref="P:System.Xml.XmlReader.NamespaceURI" />.
        /// </summary>
        /// <param name="localName"></param>
        /// <param name="namespaceUri"></param>
        /// <returns>
        /// The value of the specified attribute. If the attribute is not found or the value is String.Empty, null is returned. This method does not move the reader.
        /// </returns>
        public override string GetAttribute(string localName, string namespaceUri)
        {
            return GetAttribute(localName);
        }

        /// <summary>
        /// Gets the value of the attribute with the specified <see cref="P:System.Xml.XmlReader.Name" />.
        /// </summary>
        /// <param name="name">The qualified name of the attribute.</param>
        /// <returns>
        /// The value of the specified attribute. If the attribute is not found or the value is String.Empty, null is returned.
        /// </returns>
        public override string GetAttribute(string name)
        {
            foreach (GedcomAttribute attribute in _attributes)
            {
                if (attribute.Name.Equals(name))
                    return attribute.Value;
            }

            return String.Empty;
        }

        /// <summary>
        /// Gets the number of attributes on the current node.
        /// </summary>
        /// <returns>The number of attributes on the current node.</returns>
        public override int AttributeCount
        {
            get { return _attributes.Count; }
        }

        /// <summary>
        /// Gets the text value of the current node.
        /// </summary>
        /// <returns>The value returned depends on the <see cref="P:System.Xml.XmlReader.NodeType" /> of the node. The following table lists node types that have a value to return. All other node types return String.Empty.Node type Value AttributeThe value of the attribute. CDATAThe content of the CDATA section. CommentThe content of the comment. DocumentTypeThe internal subset. ProcessingInstructionThe entire content, excluding the target. SignificantWhitespaceThe white space between markup in a mixed content model. TextThe content of the text node. WhitespaceThe white space between markup. XmlDeclarationThe content of the declaration. </returns>
        public override string Value
        {
            get
            {
                if (_attributeMode)
                {
                    return _attributes[_currentAttributeIndex].Value;
                }

                return new string(_currentNode.Lexeme);
            }
        }

        /// <summary>
        /// Gets the depth of the current node in the XML document.
        /// </summary>
        /// <returns>The depth of the current node in the XML document.</returns>
        public override int Depth
        {
            get
            {
                if (_attributeMode)
                {
                    return _nodeStack.Count + 1;
                }

                return _nodeStack.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current node can have a <see cref="P:System.Xml.XmlReader.Value" />.
        /// </summary>
        /// <returns>true if the node on which the reader is currently positioned can have a Value; otherwise, false. If false, the node has a value of String.Empty.</returns>
        public override bool HasValue
        {
            get
            {
                if (_attributeMode)
                {
                    return true;
                }

                return (_currentNode.Type == XmlNodeType.Text);
            }
        }

        /// <summary>
        /// Gets the local name of the current node.
        /// </summary>
        /// <returns>The name of the current node with the prefix removed. For example, LocalName is book for the element &lt;bk:book&gt;.For node types that do not have a name (like Text, Comment, and so on), this property returns String.Empty.</returns>
        /// <exception cref="ThunderMain.GedcomReader.GedcomException"></exception>
        public override string LocalName
        {
            get
            {
                if (_attributeMode)
                {
                    return (_attributes[_currentAttributeIndex]).Name;
                }

                switch (_currentNode.Type)
                {
                    case XmlNodeType.EndElement:
                    case XmlNodeType.Element:
                        return new string(_currentNode.Lexeme);
                    case XmlNodeType.Text:
                        return String.Empty;
                    case XmlNodeType.None:
                        return String.Empty;
                    case XmlNodeType.ProcessingInstruction:
                        return new string(_currentNode.Lexeme);
                    default:
                        throw new GedcomException("UNKNOWN TOKEN", _currentNode.Lexeme[0], LinePosition, LineNumber);
                }
            }
        }

        /// <summary>
        /// Gets the type of the current node.
        /// </summary>
        /// <returns>One of the <see cref="T:System.Xml.XmlNodeType" /> values representing the type of the current node.</returns>
        public override XmlNodeType NodeType
        {
            get
            {
                if (_attributeMode)
                {
                    if (_attributeRead)
                    {
                        return XmlNodeType.Text;
                    }

                    return XmlNodeType.Attribute;
                }

                return _currentNode.Type;
            }
        }
    }
}