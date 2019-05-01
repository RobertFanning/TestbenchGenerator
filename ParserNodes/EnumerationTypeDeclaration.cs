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
			fEnumerationList = enumerationList;

		}

		readonly string fIdentifier;

		readonly List<string> fEnumerationList;
	
		public string Identifier { get { return fIdentifier; } }

		public int Left { get { return (fEnumerationList.Count-1); } }

		public int Right { get { return '0'; } }

	}
}
