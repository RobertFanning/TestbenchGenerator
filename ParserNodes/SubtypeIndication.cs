using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	public class SubtypeIndication : SignalType
	{
		public SubtypeIndication(string type, int left, int right)
		{
			if (type == null) throw new ArgumentNullException("indentifier");

			fType = type;
			fleft = left;
			fright = right;
		}

		readonly string fType;
	
		public string Type { get { return fType; } }

		readonly int fleft;

		public int Left { get { return fleft; } }
	
		
		readonly int fright;

		public int Right { get { return fright; } }

		public override string getType()
        {
            // Just return it.  Too easy.
            return "SubtypeIndication";
        }

		public override int getLeft()
        {
            // Just return it.  Too easy.
            return Left;
        }

		public override int getRight()
        {
            // Just return it.  Too easy.
            return Right;
        }

		public override Boolean isUnpacked()
        {
            // Just return it.  Too easy.
            return false;
        }

		public override string PortmapDefinition()
        {
            if (Left != Right)
            	return ("  logic [" + Left + ":" + Right + "]  ");
			else 
				return ("  logic  ");
		}
	}
}
