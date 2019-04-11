using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents a comparison operator.</summary>
	public enum ComparisonOperator
	{
		/// <summary>No operator.</summary>
		None = 0,

		/// <summary>The equal operator.</summary>
		Equal,

		/// <summary>The not-equal operator.</summary>
		NotEqual,

		/// <summary>The less-than operator.</summary>
		LessThan,

		/// <summary>The greater-than operator.</summary>
		GreaterThan,

		/// <summary>The less-than-or-equal operator.</summary>
		LessThanOrEqual,

		/// <summary>The greater-than-or-equal operator.</summary>
		GreaterThanOrEqual
	}
}
