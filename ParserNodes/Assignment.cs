using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents an assignment of a value to a variable.</summary>
	public class Assignment : Statement
	{
		/// <summary>Initializes a new instance of the <see cref="Assignment"/> class.</summary>
		/// <param name="variable">The variable to assign to.</param>
		/// <param name="value">The value to assign.</param>
		public Assignment(Variable variable, Expression value)
		{
			if (variable == null) throw new ArgumentNullException("variable");
			if (value == null) throw new ArgumentNullException("value");

			fVariable = variable;
			fValue = value;
		}

		readonly Variable fVariable;
		/// <summary>Gets the variable to assign to.</summary>
		/// <value>The variable to assign to.</value>
		public Variable Variable { get { return fVariable; } }

		readonly Expression fValue;
		/// <summary>Gets the expression to assign.</summary>
		/// <value>The expression to assign.</value>
		public Expression Value { get { return fValue; } }
	}
}
