// Copyright (c) ThunderMain Ltd. All rights reserved. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace ThunderMain.GedcomReader
{
    /// <summary>
    /// Singleton providing utility methods to validate GEDCOM characters
    /// </summary>
    internal static class GedcomCharType
    {
        // Could use bitfields to store the char type, but it's probably not
        // worth it. For an extra 128K we get faster read access

        // Allow 16 bits to cope with all Unicode characters
        private readonly static bool[] XrefChar = new bool[0x10000]; // attribute
        private readonly static bool[] TagChar = new bool[0x10000]; // element name
        private readonly static bool[] LineValueChar = new bool[0x10000]; // text node

        private const int TypeXref = 1;
        private const int TypeTag = 2;
        private const int TypeLinevalue = 3;

        static GedcomCharType()
        {
            // The top-bit set characters only apply to the ANSEL character set
            SetCharType(new[] {
                            '@', /* to */ '@',
                            'A', /* to */ 'Z',
                            'a', /* to */ 'z',
                            '_', /* to */ '_',
                            '0', /* to */ '9',
                            ' ', /* to */ ' ',
                            '#', /* to */ '#',
                            '!', /* to */ '"',
                            '$', /* to */ '/',
                            ':', /* to */ '?',
                            '[', /* to */ '^',
                            '`', /* to */ '`',
                            '{', /* to */ '~',
                            '\u0080', /* to */ '\u00FE',
                          /* non-standard, but include unicode chars too */ 
                          '\u00FF', /* to */ '\uFFFE'}, TypeXref);

            SetCharType(new[] {
                         'A', /* to */ 'Z',
                         'a', /* to */ 'z',
                         '0', /* to */ '9',
                         '_', /* to */ '_'    }, TypeTag);

            SetCharType(new[] {
                          '@', /* to */ '@', 
                          'A', /* to */ 'Z',
                          'a', /* to */ 'z',
                          '_', /* to */ '_',
                          '0', /* to */ '9',
                          ' ', /* to */ ' ',
                          '#', /* to */ '#',
                          '!', /* to */ '"',
                          '$', /* to */ '/',
                          ':', /* to */ '?',
                          '[', /* to */ '^',
                          '`', /* to */ '`',
                          '{', /* to */ '~',
                          '\t', /* to */ '\t', /* NOTE: This is not strict GEDCOM 5.5 */
                          '\u0080', /* to */ '\u00FE',
                          /* non-standard, but include unicode chars too */ 
                          '\u00FF', /* to */ '\uFFFE'}, TypeLinevalue);
        }

        private static void SetCharType(IList<char> ranges, int type)
        {
            // For each range
            for (int i = 0; i < ranges.Count; i += 2)
            {
                // For each character in that range
                for (char index = ranges[i]; index <= ranges[i + 1]; index++)
                {
                    // Set it's corresponding attribute
                    switch (type)
                    {
                        case TypeLinevalue:
                            LineValueChar[index] = true;
                            break;
                        case TypeTag:
                            TagChar[index] = true;
                            break;
                        case TypeXref:
                            XrefChar[index] = true;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the given character is an allowed character for a level tag.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsLevelChar(char ch)
        {
            // Produces smallest possible IL code
            return (ch >= '0' && ch <= '9');
        }

        /// <summary>
        /// Determines whether the given character is an allowed character for a Tag.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsTagChar(char ch)
        {
            // alpha | digit
            // alpha = 'A' - 'Z', 'a'-'z', '_'
            // digit = '0' - '9'

            return TagChar[ch];
        }

        /// <summary>
        /// Determines whether the given character is an allowed character for an XREF.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsXrefChar(char ch)
        {
            // '@' | alpha | digit | non_at
            // alpha = 'A' - 'Z', 'a'-'z', '_'
            // digit = '0' - '9'
            // non_at = otherchar | ' ' | '#'
            // otherchar = '!' - '"', '$' - '/', ':' - '?', '[' - '^', '`',
            //			   '{' - '~', 0x80 - 0xFE

            return XrefChar[ch];
        }
 
        /// <summary>
        /// Determines whether the given character is an allowed character for a Line Value.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsLineValueChar(char ch)
        {
            // '@' | alpha | digit | non_at | any_char
            // alpha = 'A' - 'Z', 'a'-'z', '_'
            // digit = '0' - '9'
            // non_at = otherchar | ' ' | '#'
            // otherchar = '!' - '"', '$' - '/', ':' - '?', '[' - '^', '`',
            //			   '{' - '~', 0x80 - 0xFE

            return LineValueChar[ch];
        }

        /// <summary>
        /// Determines whether the given character is an allowed character for a delimited.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsDelimChar(char ch)
        {
            return (ch == ' ');
        }

        /// <summary>
        /// Determines whether the given character is an allowed character for a terminator.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        [Obsolete]
        public static bool IsTerminatorChar(char ch)
        {
            return (ch == '\r' || ch == 'f');
        }
    }
}