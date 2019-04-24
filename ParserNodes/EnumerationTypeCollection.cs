using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	public class EnumerationTypeCollection : ICollection<EnumerationTypeDeclaration>
	{
		readonly ICollection<EnumerationTypeDeclaration> fEnumerationTypeDeclarations;
		/// <summary>Initializes a new instance of the <see cref="TypeCollection"/> class.</summary>
		/// <param name="EnumerationTypeDeclarations">The EnumerationTypeDeclarations of this collection.</param>
		public EnumerationTypeCollection(ICollection<EnumerationTypeDeclaration> EnumerationTypeDeclarations)
		{
			if (EnumerationTypeDeclarations == null) throw new ArgumentNullException("EnumerationTypeDeclarations");

			fEnumerationTypeDeclarations = EnumerationTypeDeclarations;
		}

		#region ICollection<EnumerationTypeDeclaration> Members

		void ICollection<EnumerationTypeDeclaration>.Add(EnumerationTypeDeclaration item) { throw new NotSupportedException(); }
		void ICollection<EnumerationTypeDeclaration>.Clear() { throw new NotSupportedException(); }
		bool ICollection<EnumerationTypeDeclaration>.Remove(EnumerationTypeDeclaration item) { throw new NotSupportedException(); }

		/// <summary>Determines whether this collection contains a specific EnumerationTypeDeclaration.</summary>
		/// <param name="item">The EnumerationTypeDeclaration to locate in the collection.</param>
		/// <returns><c>true</c> if item is found in the collection; otherwise, <c>false</c>.</returns>
		public bool Contains(EnumerationTypeDeclaration item) { return fEnumerationTypeDeclarations.Contains(item); }

		/// <summary>Copies the EnumerationTypeDeclarations of this collection to an Enumeration, starting at a particular index.</summary>
		/// <param name="Enumeration">The one-dimensional Enumeration that is the destination of the elements copied from this collection. The Enumeration must have zero-based indexing.</param>
		/// <param name="EnumerationIndex">The zero-based index in Enumeration at which copying begins.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">EnumerationIndex is less than 0.</exception>
		/// <exception cref="T:System.ArgumentNullException">Enumeration is null.</exception>
		/// <exception cref="T:System.ArgumentException">Enumeration is multidimensional.-or-EnumerationIndex is equal to or greater than the length of Enumeration.-or-The number of elements in this collection is greater than the available space from EnumerationIndex to the end of the destination Enumeration.-or-Type T cannot be cast automatically to the type of the destination Enumeration.</exception>
		public void CopyTo(EnumerationTypeDeclaration[] Enumeration, int EnumerationIndex) { fEnumerationTypeDeclarations.CopyTo(Enumeration, EnumerationIndex); }

		/// <summary>Gets the number of elements contained in this collection.</summary>
		/// <returns>The number of elements contained in this collection.</returns>
		public int Count { get { return fEnumerationTypeDeclarations.Count; } }

		/// <summary>Gets a value indicating whether this collection is read-only.</summary>
		/// <returns><c>true</c> if this collection is read-only; otherwise, <c>false</c>.</returns>
		public bool IsReadOnly { get { return true; } }

		#endregion

		#region IEnumerable<EnumerationTypeDeclaration> Members

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>A <see cref="System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.</returns>
		public IEnumerator<EnumerationTypeDeclaration> GetEnumerator() { return fEnumerationTypeDeclarations.GetEnumerator(); }

		#endregion

		#region IEnumerable Members

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return fEnumerationTypeDeclarations.GetEnumerator(); }

		#endregion
	}
}
