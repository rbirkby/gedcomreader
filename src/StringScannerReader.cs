using System;

namespace GedComReader
{
	/// <summary>
	/// Provides a facade onto a string of characters
	/// </summary>
	internal class StringScannerReader : ScannerReader
	{
		private int ifStartLinePos=0;
		private int ifPos=0;
		private int ifStart=0;
		private int ifEnd=0;

		private string zfChars;

		public StringScannerReader(string zChars) {
			zfChars=zChars + "\0";
		}

		public override string Lexeme {
			get {return zfChars.Substring(ifStart, ifEnd-ifStart); }
		}
		public override char CurrChar() {
			return zfChars[ifPos];
		}
		public override void ReadChar() {
			ifPos++;
		}
		public override void MarkLexemaStart() {
			ifStart=ifPos;
		}
		public override void MarkLexemaEnd() {
			ifEnd=ifPos;
		}
		public override void MarkLineStart() {
			ifStartLinePos=ifPos;
		}
		public override int MarkedLength() {
			return ifPos-ifStart;
		}
		public override int CurrColumn() {
			return ifPos-ifStartLinePos;
		}
	}
}
