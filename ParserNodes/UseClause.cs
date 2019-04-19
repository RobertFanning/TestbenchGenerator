using System;
using System.Collections.Generic;
using System.Text;


namespace VHDLparser.ParserNodes
{
	/// <summary>Represents an if-Clause.</summary>
	public class UseClause : Clause
	{
		public UseClause(Token library, Token package)
		{
			if (library == null) throw new ArgumentNullException("library");
			if (package == null) throw new ArgumentNullException("package");
		

			fLibrary = library;
			fPackage = package;
		}

		readonly Token fLibrary;
		public Token Library { get { return fLibrary; } }

		readonly Token fPackage;
		public Token Package { get { return fPackage; } }

	}
}
