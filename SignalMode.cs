namespace VHDLparser
{
	/// <summary>Represents the type of a token.</summary>
	public enum SignalMode
	{
		/// <summary>No token.</summary>
		None = 0,

		/// <summary>A word: starts with a letter, followed by zero or more letters or digits.</summary>
		In,

		/// <summary>An integer: one or more digits.</summary>
		Out,

		/// <summary>A string: a sequence of characters, enclosed in quotes.</summary>
		InOut,

		/// <summary>A symbol: one of the known symbols.</summary>
		Buffer,

		Linkage
	}
}
