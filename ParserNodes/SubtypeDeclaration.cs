using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	public class SubtypeDeclaration : SignalType
	{
		public SubtypeDeclaration(string identifier, SubtypeIndication subtype)
		{
			if (identifier == null) throw new ArgumentNullException("indentifier");

			fIdentifier = identifier;
			fSubtype = subtype;
		}

		readonly string fIdentifier;
	
		public string Identifier { get { return fIdentifier; } }

		readonly SubtypeIndication fSubtype;

		public SubtypeIndication Subtype { get { return fSubtype; } }

		public int Right { get { return fSubtype.Right; } }

		public int Left { get { return fSubtype.Left; } }

		public override string getType()
        {
            // Just return it.  Too easy.
            return "Subtype";
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
            if (Left != Right)
            	return ("  logic [" + Left + ":" + Right + "]  ");
			else 
				return ("  logic  ");
        }
	}
}
