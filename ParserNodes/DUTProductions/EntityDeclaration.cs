using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
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
		public string Name { get { return fName; } }

		readonly List<ParserNode> fBlock;
		public List<ParserNode> Block { get { return fBlock; } }


		
	}
}
