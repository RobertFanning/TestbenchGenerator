using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{

	public abstract class SignalType : ParserNode
	{
		public abstract string getType();
		public abstract int getLeft();
		public abstract int getRight();

		public abstract string getIdentifier();
		public abstract Boolean isUnpacked();
		public abstract string PortmapDefinition();
		public abstract SignalType getSignalType();
	}
}
