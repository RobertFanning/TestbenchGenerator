using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents a division.</summary>
	public class Division : BinaryExpression
	{
		/// <summary>Initializes a new instance of the <see cref="Division"/> class.</summary>
		/// <param name="leftChildExpression">The left child expression.</param>
		/// <param name="rightChildExpression">The right child expression.</param>
		public Division(Expression leftChildExpression, Expression rightChildExpression)
			: base(leftChildExpression, rightChildExpression) { }
	}
}
