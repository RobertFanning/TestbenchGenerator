using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	public class SubtypeIndication : Declaration
	{
		public SubtypeIndication(string type, Node left, Node right)
		{
			if (type == null) throw new ArgumentNullException("indentifier");

			fType = type;
			fleft = left;
			fright = right;
		}

		readonly string fType;
	
		public string Type { get { return fType; } }

		readonly Node fleft;

		public int Left { get { return fleft.Eval(); } }
	
		
		readonly Node fright;

		public int Right { get { return fright.Eval(); } }
	}
}
