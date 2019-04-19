using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a logical OR-expression.</summary>
	public class OrExpression : BinaryExpression
	{
		/// <summary>Initializes a new instance of the <see cref="OrExpression"/> class.</summary>
		/// <param name="leftChildExpression">The left child expression.</param>
		/// <param name="rightChildExpression">The right child expression.</param>
		public OrExpression(Expression leftChildExpression, Expression rightChildExpression)
			: base(leftChildExpression, rightChildExpression) { }
	}
}
