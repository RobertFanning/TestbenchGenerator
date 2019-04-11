using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents a negation.</summary>
	public class Negation : UnaryExpression
	{
		/// <summary>Initializes a new instance of the <see cref="Negation"/> class.</summary>
		/// <param name="childExpression">The child expression to negate.</param>
		public Negation(Expression childExpression) 
			: base(childExpression) { }
	}
}
