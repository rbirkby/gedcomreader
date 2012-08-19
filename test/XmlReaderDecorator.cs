// Copyright (c) ThunderMain Ltd. All rights reserved. See LICENSE in the project root for license information.
using System;
using System.Xml;

namespace ThunderMain.GedcomReader.Test
{
    /// <summary>
    /// Useful debugging class. Simply wrap any XmlReader in this Decorator and
    /// obtain useful AOP-like method entry information
    /// </summary>
    internal class XmlReaderDecorator : XmlReader
    {
        private readonly XmlReader _internal;

        /// <summary></summary>
        public XmlReaderDecorator(XmlReader internalReader)
        {
            _internal = internalReader;
        }

        /// <summary></summary>
        public override string ReadOuterXml()
        {
            Console.WriteLine("ReadOuterXml");
            return _internal.ReadOuterXml();
        }

        /// <summary></summary>
        public override bool ReadAttributeValue()
        {
            Console.WriteLine("ReadAttributeValue");
            return _internal.ReadAttributeValue();
        }

        /// <summary></summary>
        public override bool IsEmptyElement
        {
            get
            {
                Console.WriteLine("IsEmptyElement");
                return _internal.IsEmptyElement;
            }
        }

        /// <summary></summary>
        public override void ResolveEntity()
        {
            Console.WriteLine("ResolveEntity");
            _internal.ResolveEntity();
        }

        /// <summary></summary>
        public override string LookupNamespace(string prefix)
        {
            Console.WriteLine("LookupNamespace{0}", prefix);
            return _internal.LookupNamespace(prefix);
        }

        /// <summary></summary>
        public override XmlNameTable NameTable
        {
            get
            {
                Console.WriteLine("NameTable");
                return _internal.NameTable;
            }
        }

        /// <summary></summary>
        public override string ReadInnerXml()
        {
            Console.WriteLine("ReadInnerXml");
            return _internal.ReadInnerXml();
        }
        /// <summary></summary>
        public override string ReadString()
        {
            Console.WriteLine("ReadString");
            return _internal.ReadString();
        }

        /// <summary></summary>
        public override ReadState ReadState
        {
            get
            {
                Console.WriteLine("ReadState");
                return _internal.ReadState;
            }
        }

        /// <summary></summary>
        public override void Close()
        {
            Console.WriteLine("Close");
            _internal.Close();
        }

        /// <summary></summary>
        public override bool EOF
        {
            get
            {
                Console.WriteLine("EOF");
                return _internal.EOF;
            }
        }

        /// <summary></summary>
        public override bool Read()
        {
            Console.WriteLine("Read");
            return _internal.Read();
        }

        /// <summary></summary>
        public override bool MoveToElement()
        {
            Console.WriteLine("CloMoveToElementse");
            return _internal.MoveToElement();
        }

        /// <summary></summary>
        public override bool MoveToNextAttribute()
        {
            bool bResult = _internal.MoveToNextAttribute();
            Console.WriteLine("MoveToNextAttribute={0}", bResult);
            return bResult;
        }

        /// <summary></summary>
        public override bool MoveToFirstAttribute()
        {
            Console.WriteLine("MoveToFirstAttribute");
            return _internal.MoveToFirstAttribute();
        }

        /// <summary></summary>
        public override void MoveToAttribute(int i)
        {
            Console.WriteLine("MoveToAttribute {0}", i);
            _internal.MoveToAttribute(i);
        }

        /// <summary></summary>
        public override bool MoveToAttribute(string localName, string namespaceUri)
        {
            Console.WriteLine("MoveToAttribute {0}, {1}", localName, namespaceUri);
            return _internal.MoveToAttribute(localName, namespaceUri);
        }

        /// <summary></summary>
        public override bool MoveToAttribute(string name)
        {
            Console.WriteLine("MoveToAttribute {0}", name);
            return _internal.MoveToAttribute(name);
        }

        /// <summary></summary>
        public override string this[string name, string namespaceUri]
        {
            get
            {
                Console.WriteLine("Item {0}, {1}", name, namespaceUri);
                return _internal[name, namespaceUri];
            }
        }

        /// <summary></summary>
        public override string this[string name]
        {
            get
            {
                Console.WriteLine("Item {0}", name);
                return _internal[name];
            }
        }

        /// <summary></summary>
        public override string this[int i]
        {
            get
            {
                Console.WriteLine("Item {0}", i);
                return _internal[i];
            }
        }

        /// <summary></summary>
        public override string GetAttribute(int i)
        {
            Console.WriteLine("GetAttribute {0}", i);
            return _internal.GetAttribute(i);
        }

        /// <summary></summary>
        public override string GetAttribute(string localName, string namespaceUri)
        {
            Console.WriteLine("GetAttribute {0}, {1}", localName, namespaceUri);
            return _internal.GetAttribute(localName, namespaceUri);
        }

        /// <summary></summary>
        public override string GetAttribute(string name)
        {
            Console.WriteLine("GetAttribute {0}", name);
            return _internal.GetAttribute(name);
        }

        /// <summary></summary>
        public override int AttributeCount
        {
            get
            {
                Console.WriteLine("AttributeCount");
                return _internal.AttributeCount;
            }
        }

        /// <summary></summary>
        public override string XmlLang
        {
            get
            {
                Console.WriteLine("XmlLang");
                return _internal.XmlLang;
            }
        }

        /// <summary></summary>
        public override XmlSpace XmlSpace
        {
            get
            {
                Console.WriteLine("XmlSpace");
                return _internal.XmlSpace;
            }
        }

        /// <summary></summary>
        public override char QuoteChar
        {
            get
            {
                Console.WriteLine("QuoteChar");
                return _internal.QuoteChar;
            }
        }
        
        /// <summary></summary>
        public override bool IsDefault
        {
            get
            {
                Console.WriteLine("IsDefault");
                return _internal.IsDefault;
            }
        }

        /// <summary></summary>
        public override string BaseURI
        {
            get
            {
                Console.WriteLine("BaseURI");
                return _internal.BaseURI;
            }
        }
        
        /// <summary></summary>
        public override int Depth
        {
            get
            {
                Console.WriteLine("Depth");
                return _internal.Depth;
            }
        }
        
        /// <summary></summary>
        public override string Value
        {
            get
            {
                Console.WriteLine("Value");
                return _internal.Value;
            }
        }
        
        /// <summary></summary>
        public override bool HasValue
        {
            get
            {
                Console.WriteLine("HasValue");
                return _internal.HasValue;
            }
        }
        
        /// <summary></summary>
        public override string Prefix
        {
            get
            {
                Console.WriteLine("Close");
                return _internal.Prefix;
            }
        }
        
        /// <summary></summary>
        public override string NamespaceURI
        {
            get
            {
                Console.WriteLine(_internal.NamespaceURI + "=NamespaceURI()");
                return _internal.NamespaceURI;
            }
        }
        
        /// <summary></summary>
        public override string LocalName
        {
            get
            {
                Console.WriteLine(_internal.LocalName + "=LocalName()");
                return _internal.LocalName;
            }
        }
        
        /// <summary></summary>
        public override string Name
        {
            get
            {
                Console.WriteLine(_internal.Name + "=Name()");
                return _internal.Name;
            }
        }

        /// <summary></summary>
        public override XmlNodeType NodeType
        {
            get
            {
                Console.WriteLine(_internal.NodeType + "=NodeType()");
                return _internal.NodeType;
            }
        }
    }
}