using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents an expression with 1 child expression.</summary>
	public abstract class UnaryExpression : Expression
	{
		/// <summary>Initializes a new instance of the <see cref="UnaryExpression"/> class.</summary>
		/// <param name="childExpression">The child expression.</param>
		protected UnaryExpression(Expression childExpression)
		{
			if (childExpression == null) throw new ArgumentNullException("childExpression");

			fChildExpression = childExpression;
		}

		readonly Expression fChildExpression;
		/// <summary>Gets the child expression to inverse.</summary>
		/// <value>The child expression to inverse.</value>
		public Expression ChildExpression { get { return fChildExpression; } }
	}
}
