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
			//if (portExpressions == null) throw new ArgumentNullException("portExpressions");
			fExpressions = portExpressions;
		}

		readonly List<PortInterfaceElement> fExpressions;
		/// <summary>Gets the block that is executed when the condition evaluates to true.</summary>
		/// <value>The block that is executed when the condition evaluates to true.</value>
		public List<PortInterfaceElement> Expressions { get { return fExpressions; } }

	}
}
