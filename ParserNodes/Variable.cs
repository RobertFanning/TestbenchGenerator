using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents a variable.</summary>
	public class Variable : Expression
	{
		/// <summary>Initializes a new instance of the <see cref="Variable"/> class.</summary>
		/// <param name="name">The name of the variable.</param>
		public Variable(string name)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (name.Length == 0) throw new ArgumentException("name cannot be an empty string.", "name");

			fName = name;
		}

		readonly string fName;
		/// <summary>Gets the name of this variable.</summary>
		/// <value>The name of this variable.</value>
		public string Name { get { return fName; } }
	}
}
