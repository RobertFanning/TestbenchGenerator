using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a function call.</summary>
	public class PortInterfaceElement : InterfaceElement
	{
		public PortInterfaceElement(string name, string inout, string type, Boolean unpacked)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (inout == null) throw new ArgumentNullException("inout");
			if (type == null) throw new ArgumentNullException("type");
			if (name.Length == 0) throw new ArgumentException("name cannot be an empty string.", "name");
			if (inout.Length == 0) throw new ArgumentException("name cannot be an empty string.", "name");
			if (type.Length == 0) throw new ArgumentException("name cannot be an empty string.", "name");
			fName = name;
			fInOut = inout;
			fType = type;
			fUnpacked = unpacked;
		}

		readonly string fName;
		public string Name { get { return fName; } }

		readonly string fInOut;
		public string InOut { get { return fInOut; } }

		readonly string fType;
		public string Type { get { return fType; } }

		readonly Boolean fUnpacked;
		public Boolean isUnpacked { get { return fUnpacked; } }


	}
}
