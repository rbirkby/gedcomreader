// Copyright (c) ThunderMain Ltd. All rights reserved. See LICENSE in the project root for license information.
using System;
using System.Xml;

namespace ThunderMain.GedcomReader.Test
{
    /// <summary>
    /// 
    /// </summary>
    internal class XmlWriterDecorator : XmlWriter
    {
        private readonly XmlWriter _internal;

        /// <summary>
        /// 
        /// </summary>
        public XmlWriterDecorator(XmlWriter internalWriter)
        {
            _internal = internalWriter;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Close()
        {
            Console.WriteLine("Close");
            _internal.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Flush()
        {
            Console.WriteLine("Flush");
            _internal.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public override string LookupPrefix(string ns)
        {
            Console.WriteLine("LookupPrefix");
            return _internal.LookupPrefix(ns);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            Console.WriteLine("WriteBase64");
            _internal.WriteBase64(buffer, index, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public override void WriteBinHex(byte[] buffer, int index, int count)
        {
            Console.WriteLine("WriteBinHex");
            _internal.WriteBinHex(buffer, index, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public override void WriteCData(string text)
        {
            Console.WriteLine("WriteCData");
            _internal.WriteCData(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        public override void WriteCharEntity(char ch)
        {
            Console.WriteLine("WriteCharEntity");
            _internal.WriteCharEntity(ch);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public override void WriteChars(char[] buffer, int index, int count)
        {
            Console.WriteLine("WriteChars");
            _internal.WriteChars(buffer, index, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public override void WriteComment(string text)
        {
            Console.WriteLine("WriteComment");
            _internal.WriteComment(text);
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
            Console.WriteLine("WriteDocType");
            _internal.WriteDocType(name, pubid, sysid, subset);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void WriteEndAttribute()
        {
            Console.WriteLine("WriteEndAttribute");
            _internal.WriteEndAttribute();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void WriteEndDocument()
        {
            Console.WriteLine("WriteEndDocument");
            _internal.WriteEndDocument();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void WriteEndElement()
        {
            Console.WriteLine("WriteEndElement");
            _internal.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public override void WriteEntityRef(string name)
        {
            Console.WriteLine("WriteEntityRef");
            _internal.WriteEntityRef(name);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void WriteFullEndElement()
        {
            Console.WriteLine("WriteFullEndElement");
            _internal.WriteFullEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public override void WriteName(string name)
        {
            Console.WriteLine("WriteName");
            _internal.WriteName(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public override void WriteNmToken(string name)
        {
            Console.WriteLine("WriteNmToken");
            _internal.WriteNmToken(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        public override void WriteProcessingInstruction(string name, string text)
        {
            Console.WriteLine("WriteProcessingInstruction");
            _internal.WriteProcessingInstruction(name, text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localName"></param>
        /// <param name="ns"></param>
        public override void WriteQualifiedName(string localName, string ns)
        {
            Console.WriteLine("WriteQualifiedName");
            _internal.WriteQualifiedName(localName, ns);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public override void WriteRaw(char[] buffer, int index, int count)
        {
            Console.WriteLine("WriteRaw");
            _internal.WriteRaw(buffer, index, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override void WriteRaw(string data)
        {
            Console.WriteLine("WriteRaw");
            _internal.WriteRaw(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="localName"></param>
        /// <param name="ns"></param>
        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            Console.WriteLine("WriteStartAttribute");
            _internal.WriteStartAttribute(prefix, localName, ns);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void WriteStartDocument()
        {
            Console.WriteLine("WriteStartDocument");
            _internal.WriteStartDocument();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="standalone"></param>
        public override void WriteStartDocument(bool standalone)
        {
            Console.WriteLine("WriteStartDocument");
            _internal.WriteStartDocument(standalone);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="localName"></param>
        /// <param name="ns"></param>
        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            Console.WriteLine("WriteStartElement");
            _internal.WriteStartElement(prefix, localName, ns);
        }

        /// <summary>
        /// 
        /// </summary>
        public override WriteState WriteState
        {
            get
            {
                Console.WriteLine("WriteState");
                return _internal.WriteState;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public override void WriteString(string text)
        {
            Console.WriteLine("WriteString");
            _internal.WriteString(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lowChar"></param>
        /// <param name="highChar"></param>
        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            Console.WriteLine("WriteSurrogateCharEntity");
            _internal.WriteSurrogateCharEntity(lowChar, highChar);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ws"></param>
        public override void WriteWhitespace(string ws)
        {
            Console.WriteLine("WriteWhitespace");
            _internal.WriteWhitespace(ws);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override string XmlLang
        {
            get
            {
                Console.WriteLine("XmlLang");

                return _internal.XmlLang;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override XmlSpace XmlSpace
        {
            get
            {
                Console.WriteLine("XmlSpace");
                return _internal.XmlSpace;
            }
        }
    }
}