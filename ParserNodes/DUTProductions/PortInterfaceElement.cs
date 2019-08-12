using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	public class PortInterfaceElement : InterfaceElement
	{
		public PortInterfaceElement(string name, string inout, string type, SignalType SigType, Boolean Array)
		{
			fName = name;
			fInOut = inout;
			fType = type;
		    TypeSig = SigType;
			fInputOutput = fInOut + "put";
			arraySignal = Array;
			fRole = "";
		}

		string fInputOutput;
		public string InputOutput { get { return fInputOutput; } }
		string fRole;
		public string Role { get { return fRole; } }
		readonly string fName;
		public string Name { get { return fName; } }

		readonly string fInOut;
		public string InOut { get { return fInOut; } }

		readonly string fType;
		public string Type { get { return fType; } }

		readonly SignalType TypeSig;

		public SignalType SignalType { get { return TypeSig; } }

		Boolean arraySignal;
		public Boolean isArray { get { return arraySignal; } }
		public Boolean isUnpacked { get { return (TypeSig.isUnpacked() || arraySignal); } }

		public string PortMapType ()
		{
			if (isUnpacked)
				return fType;
			else
				return "logic";
		}
		//For asigning the role of the signal e.g. data, req, ack, metadata
		public void setRole (string use)
		{
			fRole = use;
		}

	}
}
