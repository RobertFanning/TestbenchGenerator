using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	public class ArrayTypeCollection : ICollection<ArrayTypeDeclaration>
	{
		readonly ICollection<ArrayTypeDeclaration> fArrayTypeDeclarations;
		/// <summary>Initializes a new instance of the <see cref="TypeCollection"/> class.</summary>
		/// <param name="ArrayTypeDeclarations">The ArrayTypeDeclarations of this collection.</param>
		public ArrayTypeCollection(ICollection<ArrayTypeDeclaration> ArrayTypeDeclarations)
		{
			if (ArrayTypeDeclarations == null) throw new ArgumentNullException("ArrayTypeDeclarations");

			fArrayTypeDeclarations = ArrayTypeDeclarations;
		}

		#region ICollection<ArrayTypeDeclaration> Members

		void ICollection<ArrayTypeDeclaration>.Add(ArrayTypeDeclaration item) { throw new NotSupportedException(); }
		void ICollection<ArrayTypeDeclaration>.Clear() { throw new NotSupportedException(); }
		bool ICollection<ArrayTypeDeclaration>.Remove(ArrayTypeDeclaration item) { throw new NotSupportedException(); }

		/// <summary>Determines whether this collection contains a specific ArrayTypeDeclaration.</summary>
		/// <param name="item">The ArrayTypeDeclaration to locate in the collection.</param>
		/// <returns><c>true</c> if item is found in the collection; otherwise, <c>false</c>.</returns>
		public bool Contains(ArrayTypeDeclaration item) { return fArrayTypeDeclarations.Contains(item); }

		/// <summary>Copies the ArrayTypeDeclarations of this collection to an array, starting at a particular index.</summary>
		/// <param name="array">The one-dimensional array that is the destination of the elements copied from this collection. The array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
		/// <exception cref="T:System.ArgumentNullException">array is null.</exception>
		/// <exception cref="T:System.ArgumentException">array is multidimensional.-or-arrayIndex is equal to or greater than the length of array.-or-The number of elements in this collection is greater than the available space from arrayIndex to the end of the destination array.-or-Type T cannot be cast automatically to the type of the destination array.</exception>
		public void CopyTo(ArrayTypeDeclaration[] array, int arrayIndex) { fArrayTypeDeclarations.CopyTo(array, arrayIndex); }

		/// <summary>Gets the number of elements contained in this collection.</summary>
		/// <returns>The number of elements contained in this collection.</returns>
		public int Count { get { return fArrayTypeDeclarations.Count; } }

		/// <summary>Gets a value indicating whether this collection is read-only.</summary>
		/// <returns><c>true</c> if this collection is read-only; otherwise, <c>false</c>.</returns>
		public bool IsReadOnly { get { return true; } }

		#endregion

		#region IEnumerable<ArrayTypeDeclaration> Members

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>A <see cref="System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.</returns>
		public IEnumerator<ArrayTypeDeclaration> GetEnumerator() { return fArrayTypeDeclarations.GetEnumerator(); }

		#endregion

		#region IEnumerable Members

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return fArrayTypeDeclarations.GetEnumerator(); }

		#endregion
	}
}
