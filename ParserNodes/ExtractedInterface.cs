using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a for-loop.</summary>
	public class ExtractedInterface : Clause
	{
		public ExtractedInterface(string name, string InterfaceType, List<PortInterfaceElement> portExpressions, PortInterfaceElement dataSig, PortInterfaceElement ready, PortInterfaceElement metadata, PortInterfaceElement acknowledge)
		{
			
			fExpressions = portExpressions;
			fType = InterfaceType;
			fName = name;
			freq = ready;
			fack = acknowledge;
			fdata = dataSig;
			fmetadata = metadata;
			
		}

		//string fInOut;
		public string InOut { get { return fdata.InOut; } }
 
		readonly List<PortInterfaceElement> fExpressions;
		public List<PortInterfaceElement> Expressions { get { return fExpressions; } }

		PortInterfaceElement freq;
		public PortInterfaceElement req { get { return freq; } }

		PortInterfaceElement fack;
		public PortInterfaceElement ack { get { return fack; } }

		PortInterfaceElement fdata;
		public PortInterfaceElement data { get { return fdata; } }

		PortInterfaceElement fmetadata;
		public PortInterfaceElement meta { get { return fmetadata; } }

		readonly string fType;
		public string Type { get { return fType; } }

		readonly string fName;
		public string Name { get { return fName; } }

	}
}
