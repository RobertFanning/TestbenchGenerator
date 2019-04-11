using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents a function call, as a statement.</summary>
	public class FunctionCallStatement : Statement
	{
		/// <summary>Initializes a new instance of the <see cref="FunctionCallStatement"/> class.</summary>
		/// <param name="functionCall">The function call of this statement.</param>
		public FunctionCallStatement(FunctionCall functionCall)
		{
			if (functionCall == null) throw new ArgumentNullException("functionCall");

			fFunctionCall = functionCall;
		}

		readonly FunctionCall fFunctionCall;
		/// <summary>Gets the function call of this statement.</summary>
		/// <value>The function call of this statement.</value>
		public FunctionCall FunctionCall { get { return fFunctionCall; } }
	}
}
