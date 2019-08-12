using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	public class PackageDeclaration : Declaration
	{
		public PackageDeclaration(string moduleName)
		{
			if (moduleName == null) throw new ArgumentNullException("moduleName");
			

			fVariable = moduleName;
		}

		readonly string fVariable;
		public string Variable { get { return fVariable; } }

	}
}
