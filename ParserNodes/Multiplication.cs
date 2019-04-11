using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents a multiplication.</summary>
	public class Multiplication : BinaryExpression
	{
		/// <summary>Initializes a new instance of the <see cref="Multiplication"/> class.</summary>
		/// <param name="leftChildExpression">The left child expression.</param>
		/// <param name="rightChildExpression">The right child expression.</param>
		public Multiplication(Expression leftChildExpression, Expression rightChildExpression)
			: base(leftChildExpression, rightChildExpression) { }
	}
}
