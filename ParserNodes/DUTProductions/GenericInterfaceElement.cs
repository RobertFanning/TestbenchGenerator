using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	public class GenericInterfaceElement : InterfaceElement
	{

		public GenericInterfaceElement(string name, string type, int value)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (type == null) throw new ArgumentNullException("type");
			if (name.Length == 0) throw new ArgumentException("name cannot be an empty string.", "name");
			if (type.Length == 0) throw new ArgumentException("name cannot be an empty string.", "name");
			fName = name;
			fValue = value;
			fType = type;
		}

		readonly string fName;
		public string Name { get { return fName; } }

		readonly int fValue;
		public int Value { get { return fValue; } }

		readonly string fType;
		public string Type { get { return fType; } }
	
	}
}
