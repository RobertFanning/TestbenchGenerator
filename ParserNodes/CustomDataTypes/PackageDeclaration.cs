using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a for-loop.</summary>
	public class PackageDeclaration : Declaration
	{
		public PackageDeclaration(string moduleName)
		{
			if (moduleName == null) throw new ArgumentNullException("moduleName");
			

			fVariable = moduleName;
		}

		readonly string fVariable;
		/// <summary>Gets the variable that is incremented each iteration.</summary>
		/// <value>The variable that is incremented each iteration.</value>
		public string Variable { get { return fVariable; } }

	}
}
