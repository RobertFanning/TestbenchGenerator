using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents an if-statement.</summary>
	public class UseStatement : Statement
	{
		public UseStatement(Expression library, Expression package)
		{
			if (library == null) throw new ArgumentNullException("library");
			if (package == null) throw new ArgumentNullException("package");
		

			fLibrary = library;
			fPackage = package;
		}

		readonly Expression fLibrary;
		/// <summary>Gets the condition of this if-statement.</summary>
		/// <value>The condition of this if-statement.</value>
		public Expression Library { get { return fLibrary; } }

		readonly Expression fPackage;
		/// <summary>Gets the condition of this if-statement.</summary>
		/// <value>The condition of this if-statement.</value>
		public Expression Package { get { return fPackage; } }

	}
}
