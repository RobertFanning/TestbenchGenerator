using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a for-loop.</summary>
	public class EntityDeclaration : Declaration
	{
		public EntityDeclaration(string moduleName, List<ParserNode> block)
		{
			if (moduleName == null) throw new ArgumentNullException("moduleName");
			if (block == null) throw new ArgumentNullException("block");

			fName = moduleName;
			fBlock = block;
		}

		readonly string fName;
		/// <summary>Gets the variable that is incremented each iteration.</summary>
		/// <value>The variable that is incremented each iteration.</value>
		public string Name { get { return fName; } }

		readonly List<ParserNode> fBlock;
		/// <summary>Gets the block that is executed each iteration.</summary>
		/// <value>The block that is executed each iteration.</value>
		public List<ParserNode> Block { get { return fBlock; } }


		
	}
}
