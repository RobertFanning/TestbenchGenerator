using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a for-loop.</summary>
	public class ExtractedInterface : Clause
	{
		public ExtractedInterface(string name, string InterfaceType, List<PortInterfaceElement> portExpressions)
		{
			
			fExpressions = portExpressions;
			fType = InterfaceType;
			fName = name;
			freq = new PortInterfaceElement(null, null, null, null);
			fack = new PortInterfaceElement(null, null, null, null);
			fdata = new PortInterfaceElement(null, null, null, null);
			fmetadata = new PortInterfaceElement(null, null, null, null);
			
		}

		string fInOut;
		public string InOut { get { return fInOut; } }
 
		readonly List<PortInterfaceElement> fExpressions;
		public List<PortInterfaceElement> Expressions { get { return fExpressions; } }

		PortInterfaceElement freq;
		public PortInterfaceElement req { get { return freq; } }

		PortInterfaceElement fack;
		public PortInterfaceElement ack { get { return fack; } }

		PortInterfaceElement fdata;
		public PortInterfaceElement data { get { return fdata; } }

		PortInterfaceElement fmetadata;
		public PortInterfaceElement metadata { get { return fmetadata; } }

		readonly string fType;
		public string Type { get { return fType; } }

		readonly string fName;
		public string Name { get { return fName; } }

		public void ExtractSignals() {
			foreach (PortInterfaceElement element in fExpressions){
				if (element.Name.Contains ("req") || element.Name.Contains ("rdy"))
					freq = element;
				if (element.Name.Contains ("ack"))
					fack = element;
				if (element.Type == "audio_rxtx_t")
				{
					fdata = element;
					fInOut = element.InOut;
				}
				if (element.isUnpacked)
					fmetadata = element;
					
			}
		}


	}
}
