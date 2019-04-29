using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	public class SubtypeIndication : Declaration
	{
		public SubtypeIndication(string type, Node upper, Node lower)
		{
			if (type == null) throw new ArgumentNullException("indentifier");

			fType = type;
			fUpper = upper;
			fLower = lower;
		}

		readonly string fType;
	
		public string Type { get { return fType; } }

		readonly Node fUpper;

		public Node Upper { get { return fUpper; } }
	
		
		readonly Node fLower;

		public Node Lower { get { return fLower; } }
	}
}
