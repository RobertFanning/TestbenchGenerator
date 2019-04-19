using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents an if-Clause.</summary>
	public class LibraryClause : Clause
	{
		public LibraryClause(Token library)
		{
			if (library == null) throw new ArgumentNullException("library");
	
			fLibrary = library;
		}

		readonly Token fLibrary;
		/// <summary>Gets the condition of this if-Clause.</summary>
		/// <value>The condition of this if-Clause.</value>
		public Token Library { get { return fLibrary; } }
	}
}
