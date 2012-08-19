// Copyright (c) ThunderMain Ltd. All rights reserved. See LICENSE in the project root for license information.
using System;
using System.Xml;

namespace ThunderMain.GedcomReader
{
    /// <summary>
    /// Defaults (ie changes from abstract to virtual) many methods in XmlReader.
    /// </summary>
    public abstract class DefaultedXmlReader : XmlReader
    {
        /// <summary>
        /// Returns an empty string
        /// </summary>
        public override string this[string name, string namespaceUri]
        {
            get { return String.Empty; }
        }

        /// <summary>
        /// Returns an empty string
        /// </summary>
        public override string this[string name]
        {
            get { return this[name, String.Empty]; }
        }

        /// <summary>
        /// Returns an empty string
        /// </summary>
        public override string this[int i]
        {
            get { return String.Empty; }
        }

        /// <summary>
        /// Always returns false
        /// </summary>
        public override bool EOF
        {
            get { return false; }
        }

        /// <summary>
        /// Returns the value of the current node
        /// </summary>
        /// <returns></returns>
        public override string ReadString()
        {
            return Value;
        }

        /// <summary>
        /// Always returns ReadState.Initial
        /// </summary>
        public override ReadState ReadState
        {
            get { return ReadState.Initial; }
        }

        /// <summary>
        /// Throws a not implemented exception
        /// </summary>
        /// <returns></returns>
        public override string ReadInnerXml()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Always returns an empty string
        /// </summary>
        public override string Prefix
        {
            get { return String.Empty; }
        }
        
        /// <summary>
        /// Always returns an empty string
        /// </summary>
        public override string NamespaceURI
        {
            get { return String.Empty; }
        }
        
        /// <summary>
        /// Returns the local name of the current node
        /// </summary>
        public override string Name
        {
            get { return LocalName; }
        }
        
        /// <summary>
        /// Always returns an empty string
        /// </summary>
        public override string XmlLang
        {
            get { return String.Empty; }
        }
        
        /// <summary>
        /// Always returns XmlSpace.None
        /// </summary>
        public override XmlSpace XmlSpace
        {
            get { return XmlSpace.None; }
        }
        
        /// <summary>
        /// Always returns a single quote
        /// </summary>
        public override char QuoteChar
        {
            get { return Char.Parse("'"); }
        }
        
        /// <summary>
        /// Always returns false
        /// </summary>
        public override bool IsDefault
        {
            get { return false; }
        }

        /// <summary>
        /// Always returns false
        /// </summary>
        public override bool IsEmptyElement
        {
            get { return false; }
        }
        
        /// <summary>
        /// Always returns an empty string
        /// </summary>
        public override string BaseURI
        {
            get { return String.Empty; }
        }
        
        /// <summary>
        /// Always throws an invalid operation exception
        /// </summary>
        public override void ResolveEntity()
        {
            throw new InvalidOperationException();
        }
        
        /// <summary>
        /// Always throws a not implemented exception
        /// </summary>
        public override string LookupNamespace(string prefix)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Always throws a not implemented exception
        /// </summary>
        public override string ReadOuterXml()
        {
            throw new NotImplementedException();
        }
    }
}