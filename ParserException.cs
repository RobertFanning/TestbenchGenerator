using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace VHDLparser
{
	/// <summary>Represents an error that occurred while parsing ADL code.</summary>
	[Serializable]
	public class ParserException : Exception
	{
		/// <summary>Initializes a new instance of the <see cref="ParserException"/> class.</summary>
		public ParserException() { }

		/// <summary>Initializes a new instance of the <see cref="ParserException"/> class.</summary>
		/// <param name="message">The message that describes the error.</param>
		public ParserException(string message) : base(message) { }

		/// <summary>Initializes a new instance of the <see cref="ParserException"/> class.</summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="innerException">The inner exception that caused this exception.</param>
		public ParserException(string message, Exception innerException) : base(message, innerException) { }

		/// <summary>Initializes a new instance of the <see cref="ParserException"/> class.</summary>
		/// <param name="info">The <see cref="T:SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:SerializationException">The class name is null or <see cref="P:HResult"/> is zero (0). </exception>
		/// <exception cref="T:ArgumentNullException">The info parameter is null.</exception>
		protected ParserException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
