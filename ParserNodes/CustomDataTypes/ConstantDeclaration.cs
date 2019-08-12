using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{

	public class ConstantDeclaration : Declaration
	{
		public ConstantDeclaration(string identifier, string subtype_indication, int expression)
		{
			if (identifier == null) throw new ArgumentNullException("identifier");

			fIdentifier = identifier;
			fSubtype = subtype_indication;
			fValue = expression;

		}

		readonly string fIdentifier;

		public string Identifier { get { return fIdentifier; } }

		readonly int fValue;
		public int Value { get { return fValue; } }

		readonly string fSubtype;
	
		public string Subtype { get { return fSubtype; } }

		
	}
}
