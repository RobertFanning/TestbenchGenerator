using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents a logical NOT-expression.</summary>
	public class NotExpression : UnaryExpression
	{
		/// <summary>Initializes a new instance of the <see cref="NotExpression"/> class.</summary>
		/// <param name="childExpression">The child expression to inverse.</param>
		public NotExpression(Expression childExpression) 
			: base(childExpression) { }
	}
}
