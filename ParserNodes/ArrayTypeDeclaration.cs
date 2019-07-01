using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	//This is a composite type definition.

	//type type_name is array (range) of element_type;
	//type NIBBLE is array (3 downto 0) of std_ulogic;
	//type RAM is array (0 to 31) of integer range 0 to 255;
	public class ArrayTypeDeclaration : SignalType
	{
		public ArrayTypeDeclaration(string identifier, int from, int to, SignalType subtype)
		{
			if (identifier == null) throw new ArgumentNullException("indentifier");
			if (from == null) throw new ArgumentNullException("ArrayType_indication");

			fIdentifier = identifier;
			fFrom = from;
			fTo = to;
			fSubtype = subtype;
		}

		readonly SignalType fSubtype;
		readonly string fIdentifier;
	
		public string Identifier { get { return fIdentifier; } }

		readonly int fFrom;

		public int From { get { return fFrom; } }

		readonly int fTo;

		public int To { get { return fTo; } }

		public override string getIdentifier()
        {
            // Just return it.  Too easy.
            return fIdentifier;
        }

		public override string getType()
        {
            // Just return it.  Too easy.
            return "ArrayType";
        }

		public override int getLeft()
        {
            // Just return it.  Too easy.
            return fFrom;
        }

		public override int getRight()
        {
            // Just return it.  Too easy.
            return fTo;
        }

		public override Boolean isUnpacked()
        {
            // Just return it.  Too easy.
            return false;
        }

		public override string PortmapDefinition()
        {
            // Just return it.  Too easy.
            return ("  logic [" + From + ":" + To + "] [" + fSubtype.getLeft() + ":" + fSubtype.getRight() + "]  ");
        }
	}
}
