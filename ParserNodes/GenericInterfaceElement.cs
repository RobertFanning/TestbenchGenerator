using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a function call.</summary>
	public class GenericInterfaceElement : InterfaceElement
	{

		public GenericInterfaceElement(string name, string type, string value)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (type == null) throw new ArgumentNullException("type");
			if (value == null) throw new ArgumentNullException("type");
			if (name.Length == 0) throw new ArgumentException("name cannot be an empty string.", "name");
			if (type.Length == 0) throw new ArgumentException("name cannot be an empty string.", "name");
			if (value.Length == 0) throw new ArgumentException("name cannot be an empty string.", "name");
			fName = name;
			fValue = value;
			fType = type;
		}

		readonly string fName;
		/// <summary>Gets the name of the function to call.</summary>
		/// <value>The name of the function to call.</value>
		public string Name { get { return fName; } }

		readonly string fValue;
		/// <summary>Gets the name of the function to call.</summary>
		/// <value>The name of the function to call.</value>
		public string Value { get { return fValue; } }

		readonly string fType;
		/// <summary>Gets the name of the function to call.</summary>
		/// <value>The name of the function to call.</value>
		public string Type { get { return fType; } }
	
	}
}
