// Copyright (c) ThunderMain Ltd. All rights reserved. See LICENSE in the project root for license information.
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace ThunderMain.GedcomReader
{
    /// <summary>
    /// The lexical analysis phase of the Gedcom parser.
    /// The scanner is initialized on the first character
    /// </summary>
    internal sealed class Scanner
    {
        // TODO: Implement switching of encodings

        private const string _testGedcomTransmission = "0 Items\n" +
            "1 Item Test with an entity: &foo;\n" +
            "1 Item @I34@ test with a child element \n" +
            "2 @I343@ more\n" +
            "1 Item test with a CDATA section <![CDATA[<456>]]> def\n" +
            "1 Item Test with an char entity: &#65;\n" +
            "1 Item 1234567890ABCD\n\0";

        /// <summary>No current token</summary>
        public const int None = 0;
        /// <summary>A level token</summary>
        public const int Level = 1;
        /// <summary>A GEDCOM tag</summary>
        public const int Tag = 2;
        /// <summary>A cross-reference token</summary>
        public const int Xref = 3;
        /// <summary>A text node</summary>
        public const int Text = 4;
        /// <summary>An escape section</summary>
        public const int Escape = 5;
        /// <summary>The end of a line</summary>
        public const int Eol = 6;
        /// <summary>The end of a file</summary>
        public const int Eof = 7;

        // ScannerReader is a facade onto a Stream, or any other 
        // source of characters
        private readonly StreamScannerReader _reader;

        private int _lineNum;

        // Strict conformance to the GEDCOM 5.5 spec?
        private readonly bool _strict;

        private int _currentToken = None;
        private bool _consumedTag;
        private readonly XmlNameTable _names;

        public XmlNameTable NameTable
        {
            get { return _names; }
        }

        /// <summary>
        /// Gets the line number of the current token
        /// </summary>
        public int LineNumber
        {
            get { return _lineNum; }
        }

        /// <summary>
        /// Gets the column position of the current token
        /// </summary>
        public int LinePosition
        {
            get { return _reader.CurrColumn(); }
        }

        /// <summary>
        /// Closes the scanner
        /// </summary>
        public void Close()
        {
            _reader.Close();
        }

        /// <summary>
        /// Instructs the scanner reader to decode bytes using a different character set decoder
        /// </summary>
        /// <param name="encoding"></param>
        public void SwitchEncoding(Encoding encoding)
        {
            _reader.SwitchEncoding(encoding);
        }

        /// <summary>
        /// Test method used during development
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="output"></param>
        private static void ReadAll(Scanner scanner, TextWriter output)
        {
            int token = None;

            while (token != Eof)
            {
                scanner.NextToken();
                token = scanner.CurrentToken;

#if DEBUG
                switch(token) {
                    case Eol:
                        output.WriteLine("[EOL]");
                        break;
                    case Level:
                        output.WriteLine("[LEVEL] {" + new string(scanner.Lexeme) + "}");
                        break;
                    case Tag:
                        output.WriteLine("[TAG] {" + new string(scanner.Lexeme) + "}");
                        break;
                    case Xref:
                        output.WriteLine("[XREF] {" + new string(scanner.Lexeme) + "}");
                        break;
                    case Eof:
                        output.WriteLine("[EOF]" );
                        break;
                    case Text:
                        output.WriteLine("[TEXT] {" + new string(scanner.Lexeme) + "}");
                        break;
                    default:
                        output.WriteLine("[UNKNOWN] {" + new string(scanner.Lexeme) + "}");
                        break;
                }
#endif
            }
        }

        #region Test/Debug stuff
        /// <summary>
        /// Test Main() for when we compile this as a standalone console app during
        /// development
        /// </summary>
        public static void Main()
        {
            TextWriter outputStream = Console.Out; //new StreamWriter(@"c:\output.txt");

            Stream inputStream = new MemoryStream(Encoding.ASCII.GetBytes(_testGedcomTransmission));
            var scanner = new Scanner(inputStream, false);

            ReadAll(scanner, outputStream);

            inputStream.Close();

            inputStream = new FileStream(@"D:\Software Library\Old Pendleton\2003-05-01.ged", FileMode.Open, FileAccess.Read);
            scanner = new Scanner(inputStream, false);
            DateTime timer = DateTime.Now;

            ReadAll(scanner, outputStream);

            Console.WriteLine("Full token scan took {0} seconds", DateTime.Now - timer);
            inputStream.Close();

            Console.WriteLine("Press any key to continue...");

            Console.ReadLine();
        }
        #endregion

        /// <summary>
        /// Create a new Lexical analyser, reading from the given stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="strict">true to conform strictly to the GEDCOM spec</param>
        public Scanner(Stream stream, bool strict)
        {
            _reader = new StreamScannerReader(stream);

            // Create a new Xml Name Table
            _names = new NameTable();

            _strict = strict;
        }

        /// <summary>
        /// Gets a human readable representation of the current token
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string tokenType = "";

            switch (CurrentToken)
            {
                case Level:
                    tokenType = "LEVEL";
                    break;
                case Tag:
                    tokenType = "TAG";
                    break;
                case Xref:
                    tokenType = "XREF";
                    break;
                case Text:
                    tokenType = "TEXT";
                    break;
                case Escape:
                    tokenType = "ESCAPE";
                    break;
                case Eol:
                    tokenType = "EOL";
                    break;
                case Eof:
                    tokenType = "EOF";
                    break;
                case None:
                    tokenType = "NONE";
                    break;
            }

            return Lexeme + "[" + tokenType + "]";
        }

        /// <summary>
        /// Gets the most recent token's value from the scanner
        /// </summary>
        public char[] Lexeme
        {
            get { return _reader.Lexeme; }
        }

        /// <summary>
        /// Obtains the current token value from the Xml Name Table
        /// </summary>
        public string LexemeNT
        {
            get
            {
                return _names.Add(Lexeme, 0, Lexeme.Length);
            }
        }

        /// <summary>
        /// Read the GEDCOM Level identifier
        /// </summary>
        private void ScanLevel()
        {
            _reader.MarkLexemeStart();

            while (true)
            {
                switch (_reader.CurrChar())
                {
                    case ' ':
                        // Maximum of 2 digits allowed
                        if (_reader.MarkedLength() > 2)
                        {
                            throw new GedcomException("BAD LEVEL", _reader.CurrChar(), _reader.CurrColumn(), _lineNum);
                        }

                        _reader.MarkLexemeEnd();
                        return;
                    default:
                        if (!GedcomCharType.IsLevelChar(_reader.CurrChar()))
                        {
                            throw new GedcomException("BAD LEVEL CHAR", _reader.CurrChar(), _reader.CurrColumn(), _lineNum);
                        }

                        _reader.ReadChar();
                        break;
                }
            }
        }

        /// <summary>
        /// Read the GEDCOM Tag identifier
        /// </summary>
        private void ScanTag()
        {
            _reader.MarkLexemeStart();

            for (; ; _reader.ReadChar())
            {
                // switch on the acceptable characters to find
                switch (_reader.CurrChar())
                {
                    case ' ':
                    case '\r':
                    case '\n':
                    case '\0':
                        // End of tag
                        _reader.MarkLexemeEnd();
                        return;
                    default:
                        if (GedcomCharType.IsTagChar(_reader.CurrChar()) == false)
                        {
                            throw new GedcomException("BAD TAG CHAR", _reader.CurrChar(), _reader.CurrColumn(), _lineNum);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Read a GEDCOM XREF - a string delineated by '@' symbols
        /// </summary>
        private void ScanXref()
        {
            // Advance past the first @
            _reader.ReadChar();

            _reader.MarkLexemeStart();

            for (; ; _reader.ReadChar())
            {
                switch (_reader.CurrChar())
                {
                    case ' ':
                    case '\r':
                    case '\n':
                    case '\0':
                        return;
                    case '@':
                        _reader.MarkLexemeEnd();

                        _reader.ReadChar();
                        // End of tag

                        return;
                    default:
                        if (!GedcomCharType.IsXrefChar(_reader.CurrChar()))
                        {
                            throw new GedcomException("BAD XREF CHAR", _reader.CurrChar(), _reader.CurrColumn(), _lineNum);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Read the 'Value' of the GEDCOM line
        /// </summary>
        private void ScanText()
        {
            // Good candidate for performance improvements
            _reader.MarkLexemeStart();

            for (; ; _reader.ReadChar())
            {
                switch (_reader.CurrChar())
                {
                    case '\r':
                    case '\n':
                    case '\0':
                        // End of tag
                        _reader.MarkLexemeEnd();

                        return;
                    case '@':
                        _reader.ReadChar();
                        switch (_reader.CurrChar())
                        {
                            case '#':
                                // found an escape
                                return;
                            case '@':
                                // OK
                                break;
                            default:
                                if (_strict)
                                {
                                    throw new GedcomException("BAD ESCAPE CHAR", _reader.CurrChar(), _reader.CurrColumn() - 1, _lineNum);
                                }
                                break;
                        }
                        break;
                    default:
                        if (!GedcomCharType.IsLineValueChar(_reader.CurrChar()))
                        {
                            throw new GedcomException("BAD LINE VALUE CHAR", _reader.CurrChar(), _reader.CurrColumn(), _lineNum);
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Read a GEDCOM escape sequence
        /// </summary>
        private void ScanEscape()
        {
            Debug.Assert(_reader.CurrChar() == '#');

            _reader.MarkLexemeStart();

            for (; ; _reader.ReadChar())
            {
                switch (_reader.CurrChar())
                {
                    case '@':
                    case '\r':
                    case '\n':
                    case '\0':
                        // End of tag
                        _reader.MarkLexemeEnd();

                        return;
                }
            }
        }

        /// <summary>
        /// Advance to the end of the current line, consuming all tokens
        /// </summary>
        private void ReadToEol()
        {
            for (; ; _reader.ReadChar())
            {
                switch (_reader.CurrChar())
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        return;
                }
            }
        }

        /// <summary>
        /// Gets the current token type - defined as constant values
        /// </summary>
        public int CurrentToken
        {
            get { return _currentToken; }
        }

        /// <summary>
        /// Pull the next token off the stream
        /// </summary>
        public void NextToken()
        {
            for (; ; )
            {
                switch (_reader.CurrChar())
                {
                    case ' ':
                        // Ignore whitespace delimiters
                        _reader.ReadChar();
                        break;
                    case '\r':
                    case '\n':
                        _reader.MarkLexemeStart();
                        _reader.ReadChar();
                        _reader.MarkLexemeEnd();

                        _reader.MarkLineStart();

                        if (_currentToken == Eol)
                            break;

                        _currentToken = Eol;
                        return;
                    case '\0':
                        _currentToken = Eof;
                        _reader.MarkLexemeEnd();
                        return;
                    default:
                        switch (_currentToken)
                        {
                            case None:
                            case Eol:

                                // We are yet to see a tag on this line
                                _consumedTag = false;

                                // Some transmissions have // style comments in them
                                if (_strict == false && _reader.CurrChar() == '/')
                                {
                                    _reader.ReadChar();

                                    if (_reader.CurrChar() == '/')
                                    {

                                        // Read to the end of the line
                                        ReadToEol();
                                    }
                                    else
                                    {
                                        throw new GedcomException("BAD LEVEL CHAR", _reader.CurrChar(), _reader.CurrColumn(), _lineNum);
                                    }

                                    break;
                                }

                                ScanLevel();
                                _currentToken = Level;
                                return;
                            case Level:
                                _lineNum++;

                                switch (_reader.CurrChar())
                                {
                                    case '@':
                                        ScanXref();
                                        _currentToken = Xref;
                                        return;
                                    default:
                                        ScanTag();
                                        _consumedTag = true;
                                        _currentToken = Tag;
                                        return;
                                }
                            case Tag:

                                switch (_reader.CurrChar())
                                {
                                    case '@':
                                        ScanXref();
                                        _currentToken = Xref;
                                        return;
                                    default:
                                        ScanText();
                                        _currentToken = Text;
                                        return;
                                }
                            case Xref:
                                if (_consumedTag == false)
                                {
                                    // Must be a tag
                                    ScanTag();
                                    _currentToken = Tag;
                                    return;
                                }

                                // Must be text
                                ScanText();
                                _currentToken = Text;
                                return;
                            case Text:
                                switch (_reader.CurrChar())
                                {
                                    case '@':
                                        ScanEscape();
                                        _currentToken = Escape;
                                        return;
                                    default:
                                        throw new GedcomException("UNEXPECTED TOKEN", _reader.CurrChar(), _reader.CurrColumn(), _lineNum);
                                }

                            case Escape:
                                ScanText();
                                _currentToken = Text;
                                return;
                            default:
                                throw new GedcomException("UNKNOWN STATE", ' ', _reader.CurrColumn(), _lineNum);
                        }
                        break;
                }
            }
        }
    }
}