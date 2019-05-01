using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	public class LibraryClause : Clause
	{
		public LibraryClause(string library)
		{
			if (library == null) throw new ArgumentNullException("library");
	
			fLibrary = library;
		}

		readonly string fLibrary;
		
		public string Library { get { return fLibrary; } }
	}
}
