using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	public class ParserNodeCollection : ICollection<ParserNode>
	{
		readonly ICollection<ParserNode> fParserNodes;
		/// <summary>Initializes a new instance of the <see cref="ParserNodeCollection"/> class.</summary>
		/// <param name="ParserNodes">The ParserNodes of this collection.</param>
		public ParserNodeCollection(ICollection<ParserNode> ParserNodes)
		{
			if (ParserNodes == null) throw new ArgumentNullException("ParserNodes");

			fParserNodes = ParserNodes;
		}

		#region ICollection<ParserNode> Members

		void ICollection<ParserNode>.Add(ParserNode item) { throw new NotSupportedException(); }
		void ICollection<ParserNode>.Clear() { throw new NotSupportedException(); }
		bool ICollection<ParserNode>.Remove(ParserNode item) { throw new NotSupportedException(); }

		/// <summary>Determines whether this collection contains a specific ParserNode.</summary>
		/// <param name="item">The ParserNode to locate in the collection.</param>
		/// <returns><c>true</c> if item is found in the collection; otherwise, <c>false</c>.</returns>
		public bool Contains(ParserNode item) { return fParserNodes.Contains(item); }

		/// <summary>Copies the ParserNodes of this collection to an array, starting at a particular index.</summary>
		/// <param name="array">The one-dimensional array that is the destination of the elements copied from this collection. The array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
		/// <exception cref="T:System.ArgumentNullException">array is null.</exception>
		/// <exception cref="T:System.ArgumentException">array is multidimensional.-or-arrayIndex is equal to or greater than the length of array.-or-The number of elements in this collection is greater than the available space from arrayIndex to the end of the destination array.-or-Type T cannot be cast automatically to the type of the destination array.</exception>
		public void CopyTo(ParserNode[] array, int arrayIndex) { fParserNodes.CopyTo(array, arrayIndex); }

		/// <summary>Gets the number of elements contained in this collection.</summary>
		/// <returns>The number of elements contained in this collection.</returns>
		public int Count { get { return fParserNodes.Count; } }

		/// <summary>Gets a value indicating whether this collection is read-only.</summary>
		/// <returns><c>true</c> if this collection is read-only; otherwise, <c>false</c>.</returns>
		public bool IsReadOnly { get { return true; } }

		#endregion

		#region IEnumerable<ParserNode> Members

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>A <see cref="System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.</returns>
		public IEnumerator<ParserNode> GetEnumerator() { return fParserNodes.GetEnumerator(); }

		#endregion

		#region IEnumerable Members

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return fParserNodes.GetEnumerator(); }

		#endregion
	}
}
