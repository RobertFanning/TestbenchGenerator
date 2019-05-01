using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser {
	// Token class stores read character or sequence of characters.
	public class Token {

		// Constructor takes the type of toke and value.
		public Token (TokenType type, string value) {
			if (value == null) throw new ArgumentNullException ("value");

			fType = type;
			fValue = value;
		}

		readonly TokenType fType;
		public TokenType Type { get { return fType; } }

		readonly string fValue;

		public string Value { get { return fValue; } }

		// Since all token values are stored as strings the value can not be directly used in a numerical expression.
		// This method returns the integer value for integer tokens.
		public int IntValue () {
			int x = 0;
			if (fType != TokenType.Integer)
				throw new ParserException ("Can only convert integer tokens to ints");

			if (Int32.TryParse (fValue, out x))
				return x;
			else
				throw new ParserException ("String not converted to int");

		}

		// Comparitive method for testing the value and type of a token.
		public bool Equals (TokenType type, string value) {
			return fType == type && fValue == value;
		}
	}
}