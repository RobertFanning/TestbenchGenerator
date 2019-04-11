using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents a comparison of 2 values.</summary>
	public class Comparison : BinaryExpression
	{
		/// <summary>Initializes a new instance of the <see cref="Comparison"/> class.</summary>
		/// <param name="comparisonOperator">The comparison operator.</param>
		/// <param name="leftChildExpression">The left child expression.</param>
		/// <param name="rightChildExpression">The right child expression.</param>
		public Comparison(ComparisonOperator comparisonOperator, Expression leftChildExpression, Expression rightChildExpression)
			: base(leftChildExpression, rightChildExpression)
		{
			fOperator = comparisonOperator;
		}

		readonly ComparisonOperator fOperator;
		/// <summary>Gets the operator.</summary>
		/// <value>The operator.</value>
		public ComparisonOperator Operator { get { return fOperator; } }
	}
}
