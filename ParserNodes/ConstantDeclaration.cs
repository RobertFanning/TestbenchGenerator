using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{

	public class ConstantDeclaration : Clause
	{
		public ConstantDeclaration(string identifier, string subtype_indication, string expression)
		{
			if (identifier == null) throw new ArgumentNullException("identifier");
			if (subtype_indication == null) throw new ArgumentNullException("subtype_indication");

			fIdentifier = identifier;
			fSubtype = subtype_indication;

		}

		readonly string fIdentifier;
		/// <summary>Gets the Identifier that is incremented each iteration.</summary>
		/// <value>The Identifier that is incremented each iteration.</value>
		public string Identifier { get { return fIdentifier; } }

		readonly string fSubtype;
		/// <summary>Gets the Identifier that is incremented each iteration.</summary>
		/// <value>The Identifier that is incremented each iteration.</value>
		public string Subtype { get { return fSubtype; } }

		
	}
}
