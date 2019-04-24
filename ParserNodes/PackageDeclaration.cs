using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a for-loop.</summary>
	public class PackageDeclaration : Declaration
	{
		public PackageDeclaration(Variable moduleName)
		{
			if (moduleName == null) throw new ArgumentNullException("moduleName");
			

			fVariable = moduleName;
		}

		readonly Variable fVariable;
		/// <summary>Gets the variable that is incremented each iteration.</summary>
		/// <value>The variable that is incremented each iteration.</value>
		public Variable Variable { get { return fVariable; } }

	}
}
