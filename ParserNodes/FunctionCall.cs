using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a function call.</summary>
	public class FunctionCall : Expression
	{
		/// <summary>Initializes a new instance of the <see cref="FunctionCall"/> class.</summary>
		/// <param name="name">The name of the function to call.</param>
		/// <param name="arguments">The arguments of the function call.</param>
		public FunctionCall(string name, Expression[] arguments)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (arguments == null) throw new ArgumentNullException("arguments");
			if (name.Length == 0) throw new ArgumentException("name cannot be an empty string.", "name");

			fName = name;
			fArguments = arguments;
		}

		readonly string fName;
		/// <summary>Gets the name of the function to call.</summary>
		/// <value>The name of the function to call.</value>
		public string Name { get { return fName; } }

		readonly Expression[] fArguments;
		/// <summary>Gets the arguments of the function call.</summary>
		/// <value>The arguments of the function call.</value>
		public ICollection<Expression> Arguments { get { return fArguments; } }
	}
}
