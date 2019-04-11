using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents an addition.</summary>
	public class Addition : BinaryExpression
	{
		/// <summary>Initializes a new instance of the <see cref="Addition"/> class.</summary>
		/// <param name="leftChildExpression">The left child expression.</param>
		/// <param name="rightChildExpression">The right child expression.</param>
		public Addition(Expression leftChildExpression, Expression rightChildExpression)
			: base(leftChildExpression, rightChildExpression) { }
	}
}
