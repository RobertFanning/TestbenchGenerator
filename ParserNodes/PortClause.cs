using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a for-loop.</summary>
	public class PortClause : Clause
	{
		public PortClause(List<PortInterfaceElement> portExpressions)
		{
			
			fExpressions = portExpressions;
			
		}

		readonly List<PortInterfaceElement> fExpressions;
		public List<PortInterfaceElement> Expressions { get { return fExpressions; } }

		public void ExtractInterfaces () {
			string[] indentifiers = null;
			List<string> interfaceName = new List<string> ();
	
			foreach (PortInterfaceElement element in fExpressions){
				indentifiers = element.Name.Split('_');
				Console.WriteLine(indentifiers[0]+indentifiers[1]);

				interfaceName.Add (String.Concat(indentifiers[0] + '_' +indentifiers[1]));
				elementLength +=1;
			}
			//int[] indexes = 
			
		}

	}
}
