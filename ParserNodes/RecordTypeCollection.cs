using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	public class RecordTypeCollection : ICollection<RecordTypeDeclaration>
	{
		readonly ICollection<RecordTypeDeclaration> fRecordTypeDeclarations;
		/// <summary>Initializes a new instance of the <see cref="TypeCollection"/> class.</summary>
		/// <param name="RecordTypeDeclarations">The RecordTypeDeclarations of this collection.</param>
		public RecordTypeCollection(ICollection<RecordTypeDeclaration> RecordTypeDeclarations)
		{
			if (RecordTypeDeclarations == null) throw new ArgumentNullException("RecordTypeDeclarations");

			fRecordTypeDeclarations = RecordTypeDeclarations;
		}

		#region ICollection<RecordTypeDeclaration> Members

		void ICollection<RecordTypeDeclaration>.Add(RecordTypeDeclaration item) { throw new NotSupportedException(); }
		void ICollection<RecordTypeDeclaration>.Clear() { throw new NotSupportedException(); }
		bool ICollection<RecordTypeDeclaration>.Remove(RecordTypeDeclaration item) { throw new NotSupportedException(); }

		/// <summary>Determines whether this collection contains a specific RecordTypeDeclaration.</summary>
		/// <param name="item">The RecordTypeDeclaration to locate in the collection.</param>
		/// <returns><c>true</c> if item is found in the collection; otherwise, <c>false</c>.</returns>
		public bool Contains(RecordTypeDeclaration item) { return fRecordTypeDeclarations.Contains(item); }

		/// <summary>Copies the RecordTypeDeclarations of this collection to an Record, starting at a particular index.</summary>
		/// <param name="Record">The one-dimensional Record that is the destination of the elements copied from this collection. The Record must have zero-based indexing.</param>
		/// <param name="RecordIndex">The zero-based index in Record at which copying begins.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">RecordIndex is less than 0.</exception>
		/// <exception cref="T:System.ArgumentNullException">Record is null.</exception>
		/// <exception cref="T:System.ArgumentException">Record is multidimensional.-or-RecordIndex is equal to or greater than the length of Record.-or-The number of elements in this collection is greater than the available space from RecordIndex to the end of the destination Record.-or-Type T cannot be cast automatically to the type of the destination Record.</exception>
		public void CopyTo(RecordTypeDeclaration[] Record, int RecordIndex) { fRecordTypeDeclarations.CopyTo(Record, RecordIndex); }

		/// <summary>Gets the number of elements contained in this collection.</summary>
		/// <returns>The number of elements contained in this collection.</returns>
		public int Count { get { return fRecordTypeDeclarations.Count; } }

		/// <summary>Gets a value indicating whether this collection is read-only.</summary>
		/// <returns><c>true</c> if this collection is read-only; otherwise, <c>false</c>.</returns>
		public bool IsReadOnly { get { return true; } }

		#endregion

		#region IEnumerable<RecordTypeDeclaration> Members

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>A <see cref="System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.</returns>
		public IEnumerator<RecordTypeDeclaration> GetEnumerator() { return fRecordTypeDeclarations.GetEnumerator(); }

		#endregion

		#region IEnumerable Members

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return fRecordTypeDeclarations.GetEnumerator(); }

		#endregion
	}
}
