using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a for-loop.</summary>
	public class PackageDeclaration : Clause
	{
		public PackageDeclaration(Variable moduleName, ClauseCollection block)
		{
			if (moduleName == null) throw new ArgumentNullException("moduleName");
			if (block == null) throw new ArgumentNullException("block");

			fVariable = moduleName;
			fBlock = block;
		}

		readonly Variable fVariable;
		/// <summary>Gets the variable that is incremented each iteration.</summary>
		/// <value>The variable that is incremented each iteration.</value>
		public Variable Variable { get { return fVariable; } }

		readonly ClauseCollection fBlock;
		/// <summary>Gets the block that is executed each iteration.</summary>
		/// <value>The block that is executed each iteration.</value>
		public ClauseCollection Block { get { return fBlock; } }
	}
}
