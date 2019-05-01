using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace VHDLparser
{
	// Represents an error that occurred while parsing the VHDL code.
	[Serializable]
	public class ParserException : Exception
	{
		// Initializes a new instance of the "ParserException" class.
		public ParserException() { }

		// Initializes a new instance of the "ParserException" class.
		// The message describes the error.
		public ParserException(string message) : base(message) { }

		// Initializes a new instance of the "ParserException" class.
		// The message describes the error.
		// innerException is the exception that caused this exception.
		public ParserException(string message, Exception innerException) : base(message, innerException) { }

		// Initializes a new instance of the "ParserException" class.
		// SerializationInfo holds the serialized object data about the exception being thrown.
		// StreamingContext contains contextual information about the source or destination.
		protected ParserException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
