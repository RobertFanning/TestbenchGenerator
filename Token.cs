using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser
{
	//Represents a token, generated from a sequence of characters.
	public class Token
	{
		
		public Token(TokenType type, string value)
		{
			if (value == null) throw new ArgumentNullException("value");

			fType = type;
			fValue = value;
		}

		readonly TokenType fType;
		public TokenType Type { get { return fType; } }

		readonly string fValue;
		
		public string Value { get { return fValue; } }

		public int IntValue() 
		{ 
			int x = 0;

			if(Int32.TryParse(fValue, out x))
				return x;
			else
				throw new ParserException("String not converted to int");

		}

		public bool Equals(TokenType type, string value)
		{
			return fType == type && fValue == value;
		}
	}
}
