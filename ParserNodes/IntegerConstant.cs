using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents an integer constant.</summary>
	public class IntegerConstant : Expression
	{
		/// <summary>Initializes a new instance of the <see cref="IntegerConstant"/> class.</summary>
		/// <param name="value">The value of this constant.</param>
		public IntegerConstant(int value) { fValue = value; }

		readonly int fValue;
		/// <summary>Gets the value of this constant.</summary>
		/// <value>The value of this constant.</value>
		public int Value { get { return fValue; } }
	}
}
