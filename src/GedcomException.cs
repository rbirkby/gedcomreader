// Copyright (c) ThunderMain Ltd. All rights reserved. See LICENSE in the project root for license information.
using System;

namespace ThunderMain.GedcomReader
{
    /// <summary>
    /// GEDCOM Reader throws exceptions when:
    /// *
    /// *
    /// *
    /// </summary>
    public class GedcomException : ApplicationException
    {
        /// <summary></summary>
        public int LinePos { get; set; }
        /// <summary></summary>
        public int LineNum { get; set; }
        /// <summary></summary>
        public char Character { get; set; }
        /// <summary></summary>
        public string Exception { get; set; }

        /// <summary></summary>
        public const int BadLevelChar = 1;
        /// <summary></summary>
        public const int BadTagChar = 2;
        /// <summary></summary>
        public const int BadXrefChar = 3;
        /// <summary></summary>
        public const int BadEscapeChar = 4;
        /// <summary></summary>
        public const int BadLinevalueChar = 5;
        /// <summary></summary>
        public const int UnexpectedEof = 6;
        /// <summary></summary>
        public const int UnexpectedToken = 7;
        /// <summary></summary>
        public const int BadLevel = 8;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomException" /> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="ch">The ch.</param>
        /// <param name="linePos">The line pos.</param>
        /// <param name="lineNum">The line num.</param>
        public GedcomException(string exception, char ch, int linePos, int lineNum)
            : base(exception)
        {
            Character = ch;
            LinePos = linePos;
            LineNum = lineNum;
            Exception = exception;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*" /></PermissionSet>
        public override string ToString()
        {
            return Exception + " at line " + LineNum + " and position " + LinePos + " - '" + Character + "'";
        }
    }
}