using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	//This is a composite type definition.

	//type type_name is array (range) of element_type;
	//type NIBBLE is array (3 downto 0) of std_ulogic;
	//type RAM is array (0 to 31) of integer range 0 to 255;
	public class ArrayTypeDeclaration : Clause
	{
		public ArrayTypeDeclaration(string identifier, string from, string to, string subtype_indication)
		{
			if (identifier == null) throw new ArgumentNullException("indentifier");
			if (from == null) throw new ArgumentNullException("ArrayType_indication");

			fIdentifier = identifier;
			fFrom = from;
		}

		readonly string fIdentifier;
	
		public string Identifier { get { return fIdentifier; } }

		readonly string fFrom;

		public string From { get { return fFrom; } }
	}
}
