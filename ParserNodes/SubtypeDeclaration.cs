using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	public class SubtypeDeclaration : Clause
	{
		public SubtypeDeclaration(string identifier, string subtype_indication)
		{
			if (identifier == null) throw new ArgumentNullException("indentifier");
			if (subtype_indication == null) throw new ArgumentNullException("subtype_indication");

			fIdentifier = identifier;
			fsubtype_indication = subtype_indication;
		}

		readonly string fIdentifier;
	
		public string Identifier { get { return fIdentifier; } }

		readonly string fsubtype_indication;

		public string subtype_indication { get { return fsubtype_indication; } }
	}
}
