using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a for-loop.</summary>
	public class PortClause : Clause
	{
		public PortClause(InterfaceList portExpressions)
		{
			if (portExpressions == null) throw new ArgumentNullException("portExpressions");
			fExpressions = portExpressions;
		}

		readonly InterfaceList fExpressions;
		/// <summary>Gets the block that is executed when the condition evaluates to true.</summary>
		/// <value>The block that is executed when the condition evaluates to true.</value>
		public InterfaceList Expressions { get { return fExpressions; } }

	}
}
