// Copyright (c) ThunderMain Ltd. All rights reserved. See LICENSE in the project root for license information.
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace ThunderMain.GedcomReader
{
    /// <summary>
    /// Writes GEDCOM format to the provided output location
    /// </summary>
    public class GedcomWriter : XmlWriter
    {
        // TODO: reorder the attribute value and attribute name if it's an ID attribute
        private readonly TextWriter _writer;
        private int _level; // The GEDCOM level or Xml Depth in the hierarchy
        private WriteState _writeState = WriteState.Start;

        /// <summary>
        /// 
        /// </summary>
        public GedcomWriter(TextWriter w)
        {
            _writer = w;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="encoding"></param>
        public GedcomWriter(string filename, Encoding encoding) :
            this(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None), encoding)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="encoding"></param>
        public GedcomWriter(Stream w, Encoding encoding)
        {
            _writer = encoding == null ? new StreamWriter(w) : new StreamWriter(w, encoding);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Close()
        {
            if (_writer != null)
                _writer.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Flush()
        {
            if (_writer != null)
                _writer.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public override string LookupPrefix(string ns)
        {
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public override void WriteBinHex(byte[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public override void WriteCData(string text)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        public override void WriteCharEntity(char ch)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public override void WriteChars(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public override void WriteComment(string text)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pubid"></param>
        /// <param name="sysid"></param>
        /// <param name="subset"></param>
        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void WriteEndAttribute()
        {
            _writer.Write("@");
            _writeState = WriteState.Element;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void WriteEndDocument()
        {
            _level--;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void WriteEndElement()
        {
            // Decrement the level counter
            _level--;
            _writeState = WriteState.Content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public override void WriteEntityRef(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void WriteFullEndElement()
        {
            if (_level > 0)
            {
                _level--;
            }
            else
            {
                _writer.Write('\n');
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public override void WriteName(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public override void WriteNmToken(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        public override void WriteProcessingInstruction(string name, string text)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localName"></param>
        /// <param name="ns"></param>
        public override void WriteQualifiedName(string localName, string ns)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public override void WriteRaw(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override void WriteRaw(string data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="localName"></param>
        /// <param name="ns"></param>
        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            _writer.Write(" @");
            _writeState = WriteState.Attribute;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void WriteStartDocument()
        {
            WriteStartDocument(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="standalone"></param>
        public override void WriteStartDocument(bool standalone)
        {
            // Do Nothing
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="localName"></param>
        /// <param name="ns"></param>
        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            if (localName != "GED")
            {
                Console.WriteLine(WriteState);
                if (WriteState != WriteState.Start)
                {
                    _writer.Write("\n");    
                }

                _writer.Write("{0:d} {1}", _level, localName);
                _level++;

                _writeState = WriteState.Element;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override WriteState WriteState
        {
            get { return _writeState; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public override void WriteString(string text)
        {
            if(WriteState == WriteState.Element) _writer.Write(' ');
            _writer.Write(text);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lowChar"></param>
        /// <param name="highChar"></param>
        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ws"></param>
        public override void WriteWhitespace(string ws)
        {
            _writer.Write(ws);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override string XmlLang
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override XmlSpace XmlSpace
        {
            get { return XmlSpace.None; }
        }
    }
}