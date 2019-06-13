using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	public class EnumerationTypeDeclaration : SignalType
	{
		public EnumerationTypeDeclaration(string identifier, List<string> enumerationList)
		{
			if (identifier == null) throw new ArgumentNullException("indentifier");


			fIdentifier = identifier;
			fEnumerationList = enumerationList;

		}

		readonly string fIdentifier;

		readonly List<string> fEnumerationList;
	
		public string Identifier { get { return fIdentifier; } }

		public int Left { get { return (fEnumerationList.Count-1); } }

		public int Right { get { return '0'; } }

		public override string getType()
        {
            // Just return it.  Too easy.
            return "EnumerationType";
        }

		public override int getLeft()
        {
            // Just return it.  Too easy.
            return Left;
        }

		public override int getRight()
        {
            // Just return it.  Too easy.
            return Right;
        }

		public override Boolean isUnpacked()
        {
            // Just return it.  Too easy.
            return false;
        }

		public override string PortmapDefinition()
        {
            // Just return it.  Too easy.
            return ("  " + Identifier + "  ");
        }

	}
}
