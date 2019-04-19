using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents an expression with 2 child expressions.</summary>
	public abstract class BinaryExpression : Expression
	{
		/// <summary>Initializes a new instance of the <see cref="BinaryExpression"/> class.</summary>
		/// <param name="leftChildExpression">The left child expression.</param>
		/// <param name="rightChildExpression">The right child expression.</param>
		protected BinaryExpression(Expression leftChildExpression, Expression rightChildExpression)
		{
			if (leftChildExpression == null) throw new ArgumentNullException("leftChildExpression");
			if (rightChildExpression == null) throw new ArgumentNullException("rightChildExpression");

			fLeftChildExpression = leftChildExpression;
			fRightChildExpression = rightChildExpression;
		}

		readonly Expression fLeftChildExpression;
		/// <summary>Gets the left child expression.</summary>
		/// <value>The left child expression.</value>
		public Expression LeftChildExpression { get { return fLeftChildExpression; } }

		readonly Expression fRightChildExpression;
		/// <summary>Gets the right child expression.</summary>
		/// <value>The right child expression.</value>
		public Expression RightChildExpression { get { return fRightChildExpression; } }
	}
}
