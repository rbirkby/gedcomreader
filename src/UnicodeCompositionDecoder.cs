// Copyright (c) ThunderMain Ltd. All rights reserved. See LICENSE in the project root for license information.
using System.Text;

namespace ThunderMain.GedcomReader
{
    /// <summary>
    /// Provides a decorator around another Decoder.
    /// Converts characters from the internal decoder from Unicode decomposed
    /// characters into precomposed characters
    /// </summary>
    public class UnicodeCompositionDecoder : Decoder
    {
        private readonly Decoder _internal;

        /// <summary></summary>
        public UnicodeCompositionDecoder(Decoder internalDecoder)
        {
            _internal = internalDecoder;
        }

        /// <summary></summary>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return _internal.GetCharCount(bytes, index, count);
        }

        /// <summary></summary>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            // TODO: Implement this method so it precomposes the character stream
            return _internal.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }
    }
}