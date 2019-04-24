using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	public class ClauseCollection : ICollection<Clause>
	{
		readonly ICollection<Clause> fClauses;

		public ClauseCollection(ICollection<Clause> Clauses)
		{
			if (Clauses == null) throw new ArgumentNullException("Clauses");

			fClauses = Clauses;
		}

		#region ICollection<Clause> Members

		void ICollection<Clause>.Add(Clause item) { throw new NotSupportedException(); }
		void ICollection<Clause>.Clear() { throw new NotSupportedException(); }
		bool ICollection<Clause>.Remove(Clause item) { throw new NotSupportedException(); }
		public bool Contains(Clause item) { return fClauses.Contains(item); }

		public void CopyTo(Clause[] array, int arrayIndex) { fClauses.CopyTo(array, arrayIndex); }

	
		public int Count { get { return fClauses.Count; } }

	
		public bool IsReadOnly { get { return true; } }

		#endregion

		#region IEnumerable<Clause> Members

		public IEnumerator<Clause> GetEnumerator() { return fClauses.GetEnumerator(); }

		#endregion

		#region IEnumerable Members


		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return fClauses.GetEnumerator(); }

		#endregion
	}
}
