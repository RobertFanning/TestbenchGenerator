using System;
using System.Collections.Generic;
using System.Text;

namespace myApp
{
	/// <summary>Represents a token, generated from a sequence of characters.</summary>
	public class Token
	{
		/// <summary>Initializes a new instance of the <see cref="Token"/> class.</summary>
		/// <param name="type">The type of the token.</param>
		/// <param name="value">The value of the token.</param>
		public Token(TokenType type, string value)
		{
			if (value == null) throw new ArgumentNullException("value");

			fType = type;
			fValue = value;
		}

		readonly TokenType fType;
		/// <summary>Gets the type of this token.</summary>
		/// <value>The type of this token.</value>
		public TokenType Type { get { return fType; } }

		readonly string fValue;
		/// <summary>Gets the value of this token.</summary>
		/// <value>The value of this token.</value>
		public string Value { get { return fValue; } }

		/// <summary>Determines whether this token has the specified type and value.</summary>
		/// <param name="type">The type to check against.</param>
		/// <param name="value">The value to check against.</param>
		/// <returns>If this token has the specified type and value, <c>true</c>; otherwise, <c>false</c>.</returns>
		public bool Equals(TokenType type, string value)
		{
			return fType == type && fValue == value;
		}
	}
}
