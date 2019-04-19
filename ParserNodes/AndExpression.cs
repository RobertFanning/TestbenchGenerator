using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a logical AND-expression.</summary>
	public class AndExpression : BinaryExpression
	{
		/// <summary>Initializes a new instance of the <see cref="AndExpression"/> class.</summary>
		/// <param name="leftChildExpression">The left child expression.</param>
		/// <param name="rightChildExpression">The right child expression.</param>
		public AndExpression(Expression leftChildExpression, Expression rightChildExpression) 
			: base(leftChildExpression, rightChildExpression) { }
	}
}
