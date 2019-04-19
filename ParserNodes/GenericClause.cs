using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a for-loop.</summary>
	public class GenericClause : Clause
	{
		public GenericClause(InterfaceList genericExpressions)
		{
			if (genericExpressions == null) throw new ArgumentNullException("portExpressions");
			fGenerics = genericExpressions;
		}

		readonly InterfaceList fGenerics;
		/// <summary>Gets the block that is executed when the condition evaluates to true.</summary>
		/// <value>The block that is executed when the condition evaluates to true.</value>
		public InterfaceList Generics { get { return fGenerics; } }

	}

}
