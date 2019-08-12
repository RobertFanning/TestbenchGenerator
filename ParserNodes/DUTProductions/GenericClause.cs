using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	public class GenericClause : Clause
	{
		public GenericClause(List<InterfaceElement> genericExpressions)
		{
			if (genericExpressions == null) throw new ArgumentNullException("portExpressions");
			fGenerics = genericExpressions;
		}

		readonly List<InterfaceElement> fGenerics;
		public List<InterfaceElement> Generics { get { return fGenerics; } }

	}

}