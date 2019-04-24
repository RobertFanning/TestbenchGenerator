using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	public class EnumerationTypeDeclaration : Declaration
	{
		public EnumerationTypeDeclaration(string identifier, List<string> enumerationList)
		{
			if (identifier == null) throw new ArgumentNullException("indentifier");


			fIdentifier = identifier;

		}

		readonly string fIdentifier;
	
		public string Identifier { get { return fIdentifier; } }

	}
}
