using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	public class SubtypeDeclaration : Declaration
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
	}
}
