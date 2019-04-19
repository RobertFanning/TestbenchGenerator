using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	public class InterfaceList : ICollection<InterfaceElement>
	{
		readonly ICollection<InterfaceElement> fElements;
		//Initializes a new instance of the InterfaceList class.
		//"Elements" are the entries of this collection.
		public InterfaceList(ICollection<InterfaceElement> Elements)
		{
			if (Elements == null) throw new ArgumentNullException("Elements");

			fElements = Elements;
		}

		#region ICollection<InterfaceElement> Members

		void ICollection<InterfaceElement>.Add(InterfaceElement item) { throw new NotSupportedException(); }
		void ICollection<InterfaceElement>.Clear() { throw new NotSupportedException(); }
		bool ICollection<InterfaceElement>.Remove(InterfaceElement item) { throw new NotSupportedException(); }

		//Determines whether this collection contains a specific InterfaceElement.
		//"item" is the InterfaceElement to locate in the collection.
		//it returns true if item is found in the collection; otherwise, false.
		public bool Contains(InterfaceElement item) { return fElements.Contains(item); }

		//Copies the Elements of this collection to an array, starting at a particular index.
		//"array" is the one-dimensional array that is the destination of the elements copied from this collection. The array must have zero-based indexing.
		//"arrayIndex" is the zero-based index in array at which copying begins.
		//"T:System.ArgumentOutOfRangeException" arrayIndex is less than 0.
		//"T:System.ArgumentNullException" array is null.
		//"T:System.ArgumentException" array is multidimensional.-or-arrayIndex is equal to or greater than the length of array.-or-The number of elements in this collection is greater than the available space from arrayIndex to the end of the destination array.-or-Type T cannot be cast automatically to the type of the destination array.
		public void CopyTo(InterfaceElement[] array, int arrayIndex) { fElements.CopyTo(array, arrayIndex); }

		//Gets the number of elements contained in this collection.
		//returns the number of elements contained in this collection.
		public int Count { get { return fElements.Count; } }

		//Gets a value indicating whether this collection is read-only.
		//returns true if this collection is read-only; otherwise, false.
		public bool IsReadOnly { get { return true; } }

		#endregion

		#region IEnumerable<InterfaceElement> Members

		//Returns an enumerator that iterates through the collection.
		//returns "System.Collections.Generic.IEnumerator`1" that can be used to iterate through the collection.
		public IEnumerator<InterfaceElement> GetEnumerator() { return fElements.GetEnumerator(); }

		#endregion

		#region IEnumerable Members

		//Returns an enumerator that iterates through a collection.
		//returns an "T:System.Collections.IEnumerator" object that can be used to iterate through the collection.
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return fElements.GetEnumerator(); }

		#endregion
	}
}
