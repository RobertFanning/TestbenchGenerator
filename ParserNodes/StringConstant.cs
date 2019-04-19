using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a string constant.</summary>
	public class StringConstant : Expression
	{
		/// <summary>Initializes a new instance of the <see cref="StringExpression"/> class.</summary>
		/// <param name="value">The value of this constant.</param>
		public StringConstant(string value)
		{
			if (value == null) throw new ArgumentNullException("value");
			fValue = value;
		}

		readonly string fValue;
		/// <summary>Gets the value of this constant.</summary>
		/// <value>The value of this constant.</value>
		public string Value { get { return fValue; } }
	}
}
