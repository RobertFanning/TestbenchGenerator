using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	public class nullType : SignalType
	{
		public nullType()
		{
		}
		public override string getIdentifier()
        {
            return "nullType";
        }
		public override string getType()
        {
            return "nullType";
        }

		public override int getLeft()
        {
            return 0;
        }

		public override int getRight()
        {
            return 0;
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
            return "nullType";
        }
	}
}
