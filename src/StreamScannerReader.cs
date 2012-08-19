// Copyright (c) ThunderMain Ltd. All rights reserved. See LICENSE in the project root for license information.
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ThunderMain.GedcomReader
{
    /// <summary>
    /// Provide a facade onto a stream of characters, extending it with facilities such as Lexeme Start and 
    /// Lexeme End marking
    /// </summary>
    internal class StreamScannerReader
    {
        private int _startLinePos;
        private int _pos; // current position
        private int _start; // Lexeme start marker
        private int _end; // Lexeme end marker
        private readonly Stream _stream;
        private readonly Decoder _decoder;
        private readonly byte[] _byteBuffer;

        private readonly char[] _chars;
        private int _bufferSize;

        public void Close()
        {
            _stream.Close();
        }

        public StreamScannerReader(Stream stream)
        {
            _chars = new Char[4096];
            _byteBuffer = new byte[4096];

            _startLinePos = 0;
            _pos = 0;
            _start = 0;
            _end = 0;
            _bufferSize = 0;


            _stream = stream;

            // TODO: Only change to ANSEL if requested in the feed
            Encoding encoding = Encoding.ASCII;//new AnselEncoding();
            _decoder = encoding.GetDecoder();

            ReadChar();
            // _pos is zero based
            _pos = 0;
        }

        /// <summary>
        /// Instructs the scanner reader to decode bytes using a different character set decoder
        /// </summary>
        /// <param name="encoding"></param>
        public void SwitchEncoding(Encoding encoding)
        {
            // TODO: Implement this
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtains the current token value
        /// </summary>
        public char[] Lexeme
        {
            get
            {
                var lexeme = new char[_end - _start];

                Array.Copy(_chars, _start, lexeme, 0, _end - _start);

                return lexeme;
            }
        }

        /// <summary>
        /// Gets the current character in the stream
        /// </summary>
        /// <returns></returns>
        public char CurrChar()
        {
            return _chars[_pos];
        }

        /// <summary>
        /// Read a new character from the stream
        /// </summary>
        public void ReadChar()
        {
            Debug.Assert(_stream != null);

            // Relocate the buffer if within 1024 bytes of the end of the buffer
            // TODO: Why test _start and not _pos???
            if (_start > _bufferSize - 1024)
            {

                RelocateBuffer();
                // Read more chars
                FillBuffer();
            }

            // Advance the position in the buffer
            _pos++;
        }

        private void FillBuffer()
        {
            // Fill the buffer with more chars
            int charsToRead = _chars.Length - _bufferSize;

            int charCount = _stream.Read(_byteBuffer, 0, charsToRead);

            // Use the decoder to convert from byteBuffer to chars
            charCount = _decoder.GetChars(_byteBuffer, 0, charCount, _chars, _bufferSize);

            _bufferSize += charCount;

            // EOF if can't read charsToRead chars
            if (charCount < charsToRead)
            {
                _chars[_bufferSize] = '\0';
                _bufferSize++;
            }
        }

        private void RelocateBuffer()
        {
            _end -= _start;
            _startLinePos -= _start;
            _pos -= _start;

            // Look into using Buffer.BlockCopy - will it be faster?
            Array.Copy(_chars, _start, _chars, 0, _bufferSize - _start);
            _bufferSize -= _start;

            // No need to the clear the remaining chars
            //Array.Clear(_chars,_bufferSize,_chars.Length-_bufferSize);

            _start = 0;
        }

        /// <summary>
        /// Set a marker at the current character position to indicate the start of a token
        /// </summary>
        public void MarkLexemeStart()
        {
            _start = _pos;
        }

        /// <summary>
        /// Sets a marker at the current character position to indicate the end of a token
        /// </summary>
        public void MarkLexemeEnd()
        {
            _end = _pos;
        }

        /// <summary>
        /// Marks the start of the line in the token stream
        /// </summary>
        public void MarkLineStart()
        {
            _startLinePos = _pos;
        }

        /// <summary>
        /// Gets the number of characters marked
        /// </summary>
        /// <returns></returns>
        public int MarkedLength()
        {
            return _pos - _start;
        }

        /// <summary>
        /// Gets the horizontal position of the current character
        /// </summary>
        /// <returns></returns>
        public int CurrColumn()
        {
            return _pos - _startLinePos;
        }
    }
}