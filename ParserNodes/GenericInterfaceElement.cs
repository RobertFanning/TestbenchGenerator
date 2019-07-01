using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a function call.</summary>
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
		/// <summary>Gets the name of the function to call.</summary>
		/// <value>The name of the function to call.</value>
		public string Name { get { return fName; } }

		readonly int fValue;
		/// <summary>Gets the name of the function to call.</summary>
		/// <value>The name of the function to call.</value>
		public int Value { get { return fValue; } }

		readonly string fType;
		/// <summary>Gets the name of the function to call.</summary>
		/// <value>The name of the function to call.</value>
		public string Type { get { return fType; } }
	
	}
}
