using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents a subtraction.</summary>
	public class Subtraction : BinaryExpression
	{
		/// <summary>Initializes a new instance of the <see cref="Subtraction"/> class.</summary>
		/// <param name="leftChildExpression">The left child expression.</param>
		/// <param name="rightChildExpression">The right child expression.</param>
		public Subtraction(Expression leftChildExpression, Expression rightChildExpression)
			: base(leftChildExpression, rightChildExpression) { }
	}
}
