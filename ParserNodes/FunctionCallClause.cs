using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a function call, as a Clause.</summary>
	public class FunctionCallClause : Clause
	{
		/// <summary>Initializes a new instance of the <see cref="FunctionCallClause"/> class.</summary>
		/// <param name="functionCall">The function call of this Clause.</param>
		public FunctionCallClause(FunctionCall functionCall)
		{
			if (functionCall == null) throw new ArgumentNullException("functionCall");

			fFunctionCall = functionCall;
		}

		readonly FunctionCall fFunctionCall;
		/// <summary>Gets the function call of this Clause.</summary>
		/// <value>The function call of this Clause.</value>
		public FunctionCall FunctionCall { get { return fFunctionCall; } }
	}
}
