using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace VHDLparser
{
	/// <summary>Converts a sequence of characters to a sequence of tokens.</summary>
	public class Tokenizer
	{
		TextReader fSource; // the source to read characters from
		char fCurrentChar; // the current character
		StringBuilder fTokenValueBuffer; // a buffer for building the value of a token

		/// <summary>Initializes a new instance of the <see cref="Tokenizer"/> class.</summary>
		/// <param name="source">The source <see cref="TextReader"/> to read the characters from.</param>
		public Tokenizer(TextReader source)
		{
			if (source == null) throw new ArgumentNullException("source");

			fSource = source;
			fTokenValueBuffer = new StringBuilder();

			// read the first character
			ReadNextChar();
		}

		/// <summary>Reads the next character.</summary>
		/// <remarks>After calling this method, fCurrentChar will contain the read character.</remarks>
		void ReadNextChar()
		{
			int lChar = fSource.Read();
			if (lChar > 0)
				fCurrentChar = (char)lChar;
			else fCurrentChar = '\0';
		}

		/// <summary>Skips the white-space characters.</summary>
		/// <remarks>Every character that is less than or equal to the space character is considered white-space.</remarks>
		void SkipWhitespace()
		{
			while (char.IsWhiteSpace(fCurrentChar))
				ReadNextChar();
		}

		/// <summary>Gets a value indicating whether the tokenizer is at the end of the source.</summary>
		/// <value><c>true</c> if the tokenizer is at the end of the source; otherwise, <c>false</c>.</value>
		bool AtEndOfSource { get { return fCurrentChar == '\0'; } }

		/// <summary>Appends the current character to the token value buffer and reads the next character.</summary>
		void StoreCurrentCharAndReadNext()
		{
			fTokenValueBuffer.Append(fCurrentChar);
			ReadNextChar();
		}

		/// <summary>Extracts the value from the token value buffer and clears the buffer.</summary>
		/// <returns>The value from the token value buffer.</returns>
		string ExtractStoredChars()
		{
			string lValue = fTokenValueBuffer.ToString();
			fTokenValueBuffer.Length = 0;
			return lValue;
		}

		/// <summary>Checks for an unexpected end of the source.</summary>
		/// <exception cref="ParserException">The end of the source has been reached unexpectedly.</exception>
		void CheckForUnexpectedEndOfSource()
		{
			if (AtEndOfSource)
				throw new ParserException("Unexpected end of source.");
		}

		/// <summary>Throws a <see cref="ParserException"/> because the current character is invalid.</summary>
		/// <exception cref="ParserException">The current character is invalid.</exception>
		void ThrowInvalidCharException()
		{
			if (fTokenValueBuffer.Length == 0)
				throw new ParserException("Invalid character '" + fCurrentChar.ToString() + "'.");
			else
			{
				throw new ParserException("Invalid character '" 
					+ fCurrentChar.ToString() + "' after '" 
					+ fTokenValueBuffer.ToString() + "'.");
			}
		}

		/// <summary>Reads the next <see cref="Token"/>.</summary>
		/// <returns>The next <see cref="Token"/>, or <c>null</c> if the end of the source has been reached.</returns>
		/// <exception cref="ParserException">The source contains invalid characters.</exception>
		public Token ReadNextToken()
		{
			SkipWhitespace();

			if (AtEndOfSource)
				return null;

			// if the first character is a letter, the token is a word
			if (char.IsLetter(fCurrentChar))
				return ReadWord();

			// if the first character is a digit, the token is an integer constant
			if (char.IsDigit(fCurrentChar))
				return ReadIntegerConstant();

			// if the first character is a quote, the token is a string constant
			if (fCurrentChar == '"')
				return ReadStringConstant();

			// in all other cases, the token should be a symbol
			return ReadSymbol();
		}

		Token ReadWord()
		{
			//do while: word can be composed of letters or digits or underscores
			do
			{
				StoreCurrentCharAndReadNext();
			}
			while (char.IsLetterOrDigit(fCurrentChar) || fCurrentChar.Equals('_'));

			return new Token(TokenType.Word, ExtractStoredChars());
		}

		//This will be triggered from read symbol and will only complete at the termination of a line.
		void ReadComment()
		{
			//do while: word can be composed of letters or digits or underscores
			//Want to discard comments so no values are stored.
			do
			{
				ReadNextChar();
			}
			while (!fCurrentChar.Equals('\n'));
			//Clears the buffer after any comment is parsed.
			fTokenValueBuffer.Length = 0;
		}

		Token ReadIntegerConstant()
		{
			do
			{
				StoreCurrentCharAndReadNext();
			}
			while (char.IsDigit(fCurrentChar));

			return new Token(TokenType.Integer, ExtractStoredChars());
		}

		Token ReadStringConstant()
		{
			ReadNextChar();
			while (!AtEndOfSource && fCurrentChar != '"')
			{
				StoreCurrentCharAndReadNext();
			}

			CheckForUnexpectedEndOfSource();
			ReadNextChar();

			return new Token(TokenType.String, ExtractStoredChars());
		}

		Token ReadSymbol()
		{
			switch (fCurrentChar)
			{
				// the symbols + - * / ( ) ,
				case '+': 
				case '(':
				case ')':
				case ';':
				case '.':
				case ',':
				case '\'':
					StoreCurrentCharAndReadNext();
					return new Token(TokenType.Symbol, ExtractStoredChars());
				// the symbols := ==
				case ':':
					StoreCurrentCharAndReadNext();
					if (fCurrentChar == '=')
					{
						StoreCurrentCharAndReadNext();
					}
					return new Token(TokenType.Symbol, ExtractStoredChars());

				//** is needed for exponential numbers.
				case '*':
					StoreCurrentCharAndReadNext();
					if (fCurrentChar == '*')
					{
						StoreCurrentCharAndReadNext();
					}
					return new Token(TokenType.Symbol, ExtractStoredChars());

				case '=':
					StoreCurrentCharAndReadNext();
					if (fCurrentChar == '=' || fCurrentChar == '>')
					{
						StoreCurrentCharAndReadNext();
					}
					return new Token(TokenType.Symbol, ExtractStoredChars());

				// for comments --
				case '-':
					StoreCurrentCharAndReadNext();
					if (fCurrentChar == '-')
					{
						StoreCurrentCharAndReadNext();
						ReadComment();
						return ReadNextToken();
					}
					return new Token(TokenType.Symbol, ExtractStoredChars());
			
				default:
					CheckForUnexpectedEndOfSource();
					ThrowInvalidCharException();
					break;
			}

			return null;
		}
	}
}
