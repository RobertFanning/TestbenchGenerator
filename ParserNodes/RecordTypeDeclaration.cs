using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	//This is a composite type definition.
	public class RecordTypeDeclaration : Clause
	{
		public RecordTypeDeclaration(string identifier, List<string> identifierList, List<string> subtypeIndication)
		{
			if (identifier == null) throw new ArgumentNullException("indentifier");

			fIdentifier = identifier;
		}

		readonly string fIdentifier;
	
		public string Identifier { get { return fIdentifier; } }

	}
}
