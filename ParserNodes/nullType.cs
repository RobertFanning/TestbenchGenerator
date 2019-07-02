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
            // Just return it.  Too easy.
            return "nullType";
        }
		public override string getType()
        {
            // Just return it.  Too easy.
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
            // Just return it.  Too easy.
            return false;
        }

		public override SignalType getSignalType()
        {
            // Just return it.  Too easy.
            throw new ParserException ("Error: Attempting to obtain signal type from enumeration signal. Enumeration signals have no subtype.");
        }

		public override string PortmapDefinition()
        {
            return "nullType";
        }
	}
}
