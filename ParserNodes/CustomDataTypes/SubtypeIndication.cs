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

		public override string getIdentifier()
        {
            return fType;
        }

		public override string getType()
        {
            return "SubtypeIndication";
        }

		public override int getLeft()
        {
            return Left;
        }

		public override int getRight()
        {
            return Right;
        }

		public override Boolean isUnpacked()
        {
            return false;
        }

		public override SignalType getSignalType()
        {
            throw new ParserException ("Error: Attempting to obtain signal type from enumeration signal. Enumeration signals have no subtype.");
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
