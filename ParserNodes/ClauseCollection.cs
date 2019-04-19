using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	public class ClauseCollection : ICollection<Clause>
	{
		readonly ICollection<Clause> fClauses;
		/// <summary>Initializes a new instance of the <see cref="ClauseCollection"/> class.</summary>
		/// <param name="Clauses">The Clauses of this collection.</param>
		public ClauseCollection(ICollection<Clause> Clauses)
		{
			if (Clauses == null) throw new ArgumentNullException("Clauses");

			fClauses = Clauses;
		}

		#region ICollection<Clause> Members

		void ICollection<Clause>.Add(Clause item) { throw new NotSupportedException(); }
		void ICollection<Clause>.Clear() { throw new NotSupportedException(); }
		bool ICollection<Clause>.Remove(Clause item) { throw new NotSupportedException(); }

		/// <summary>Determines whether this collection contains a specific Clause.</summary>
		/// <param name="item">The Clause to locate in the collection.</param>
		/// <returns><c>true</c> if item is found in the collection; otherwise, <c>false</c>.</returns>
		public bool Contains(Clause item) { return fClauses.Contains(item); }

		/// <summary>Copies the Clauses of this collection to an array, starting at a particular index.</summary>
		/// <param name="array">The one-dimensional array that is the destination of the elements copied from this collection. The array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
		/// <exception cref="T:System.ArgumentNullException">array is null.</exception>
		/// <exception cref="T:System.ArgumentException">array is multidimensional.-or-arrayIndex is equal to or greater than the length of array.-or-The number of elements in this collection is greater than the available space from arrayIndex to the end of the destination array.-or-Type T cannot be cast automatically to the type of the destination array.</exception>
		public void CopyTo(Clause[] array, int arrayIndex) { fClauses.CopyTo(array, arrayIndex); }

		/// <summary>Gets the number of elements contained in this collection.</summary>
		/// <returns>The number of elements contained in this collection.</returns>
		public int Count { get { return fClauses.Count; } }

		/// <summary>Gets a value indicating whether this collection is read-only.</summary>
		/// <returns><c>true</c> if this collection is read-only; otherwise, <c>false</c>.</returns>
		public bool IsReadOnly { get { return true; } }

		#endregion

		#region IEnumerable<Clause> Members

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>A <see cref="System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.</returns>
		public IEnumerator<Clause> GetEnumerator() { return fClauses.GetEnumerator(); }

		#endregion

		#region IEnumerable Members

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return fClauses.GetEnumerator(); }

		#endregion
	}
}
