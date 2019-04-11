namespace myApp
{
	/// <summary>Represents the type of a token.</summary>
	public enum TokenType
	{
		/// <summary>No token.</summary>
		None = 0,

		/// <summary>A word: starts with a letter, followed by zero or more letters or digits.</summary>
		Word,

		/// <summary>An integer: one or more digits.</summary>
		Integer,

		/// <summary>A string: a sequence of characters, enclosed in quotes.</summary>
		String,

		/// <summary>A symbol: one of the known symbols.</summary>
		Symbol,

		Comment
	}
}
