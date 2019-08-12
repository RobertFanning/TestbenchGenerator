using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a function call.</summary>
	public class PortInterfaceElement : InterfaceElement
	{
		public PortInterfaceElement(string name, string inout, string type, SignalType SigType, Boolean Array)
		{
		//	if (name == null) throw new ArgumentNullException("name");
		//	if (inout == null) throw new ArgumentNullException("inout");
		//	if (type == null) throw new ArgumentNullException("type");
		//	if (name.Length == 0) throw new ArgumentException("name cannot be an empty string.", "name");
		//	if (inout.Length == 0) throw new ArgumentException("name cannot be an empty string.", "name");
		//	if (type.Length == 0) throw new ArgumentException("name cannot be an empty string.", "name");
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


		// THIS IS WHERE I FINISHED TODAY
		// NEED TO GET OTHER TYPES FOR 0:-17,
		// THEY ARE STORED IN THE PARSERS LIST OF TYPES BUT NOT IN THE INTERFACE WITH THE ELEMENT
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
