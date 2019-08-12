using System;
using System.Collections.Generic;
using System.Text;


namespace VHDLparser.ParserNodes
{
	
	public class UseClause : Clause
	{
		public UseClause(string library, string package)
		{
			if (library == null) throw new ArgumentNullException("library");
			if (package == null) throw new ArgumentNullException("package");
		

			fLibrary = library;
			fPackage = package;
		}

		readonly string fLibrary;
		public string Library { get { return fLibrary; } }

		readonly string fPackage;
		public string Package { get { return fPackage; } }

	}
}
