using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VHDLparser {

	// Tokenizer is utilized for the deconstruction of the input into discrete identifiable tokens.
	public class Tokenizer {
		// Input source to be parsed.
		TextReader fSource;
		// Current Character being processed by tokenizer.
		char fCurrentChar;
		// Buffer for building the value of a token when multiple characters required.
		StringBuilder fTokenValueBuffer;

		// Constructor requires source to be parsed else exception is thrown. 
		// The parsing process is commenced by the triggering of ReadNextChar().
		public Tokenizer (TextReader source) {
			if (source == null) throw new ArgumentNullException ("source");

			fSource = source;
			fTokenValueBuffer = new StringBuilder ();

			// First character is read.
			ReadNextChar ();
		}

		// Next character in source is read, if >0 it is cast to char else null character.
		void ReadNextChar () {
			int lChar = fSource.Read ();
			if (lChar > 0)
				fCurrentChar = (char) lChar;
			else fCurrentChar = '\0';
		}
		
		// IsWhiteSpace tests for spaces and newlines, these are skipped using ReadNextToken.
		void SkipWhitespace () {
			while (char.IsWhiteSpace (fCurrentChar))
				ReadNextChar ();
		}

		// Trivial method for determining if the end of source has been reached.
		bool AtEndOfSource { get { return fCurrentChar == '\0'; } }

		// Current character is appended to buffer.
		void StoreCurrentCharAndReadNext () {
			fTokenValueBuffer.Append (fCurrentChar);
			ReadNextChar ();
		}

		// Characters stored in buffer are extracted and buffer is reset.
		string ExtractStoredChars () {
			string lValue = fTokenValueBuffer.ToString ();
			fTokenValueBuffer.Length = 0;
			return lValue;
		}

		// Exception thrown for the unexpected end of source.
		void CheckForUnexpectedEndOfSource () {
			if (AtEndOfSource)
				throw new ParserException ("Unexpected end of source.");
		}

		// Exception thrown when a symbol that is not recognised is read. 
		// Recognised symbols are specified in the ReadSymbol mehtod.
		void ThrowInvalidCharException () {
			if (fTokenValueBuffer.Length == 0)
				throw new ParserException ("Invalid character '" + fCurrentChar.ToString () + "'.");
			else {
				throw new ParserException ("Invalid character '" +
					fCurrentChar.ToString () + "' after '" +
					fTokenValueBuffer.ToString () + "'.");
			}
		}

		// White space is skipped (if present) before the tokenization process begins.
		public Token ReadNextToken () {
			SkipWhitespace ();

			if (AtEndOfSource)
				return null;

			// if the first character is a letter, the token is a word
			if (char.IsLetter (fCurrentChar))
				return ReadWord ();

			// if the first character is a digit, the token is an integer constant
			if (char.IsDigit (fCurrentChar))
				return ReadIntegerConstant ();

			// in all other cases, the token should be a symbol
			return ReadSymbol ();
		}

		// Method for extracting each character until the end of the word has ben reached.
		// Words can contain letters, digits or underscores.
		Token ReadWord () {
			do {
				StoreCurrentCharAndReadNext ();
			}
			while (char.IsLetterOrDigit (fCurrentChar) || fCurrentChar.Equals ('_'));

			return new Token (TokenType.Word, ExtractStoredChars ());
		}

		// This method is triggered from ReadSymbol method, when the symbol -- has been read.
		// The comment is read and ignored until the end of line.
		void ReadComment () {
			do {
				ReadNextChar ();
			}
			while (!fCurrentChar.Equals ('\n'));
			//Clears the buffer after comment is read.
			fTokenValueBuffer.Length = 0;
		}

		// Integers may only contain numbers. This method reads characters until non digit is reached.
		Token ReadIntegerConstant () {
			do {
				StoreCurrentCharAndReadNext ();
			}
			while (char.IsDigit (fCurrentChar));

			return new Token (TokenType.Integer, ExtractStoredChars ());
		}

		// Method for extacting and bundling read symbols.
		// VHDL contains symbols that require more than a single character,
		// these must therefore be stored as a token together since their individual function differs.
		Token ReadSymbol () {
			switch (fCurrentChar) {
				// The symbols below do not appear joined with other symbols and are therefore directly stored as tokens
				case '+':
				case '(':
				case ')':
				case '"':
				case ';':
				case '.':
				case ',':
				case '\'':
					StoreCurrentCharAndReadNext ();
					return new Token (TokenType.Symbol, ExtractStoredChars ());
				// Both ':' and ':=' are possible symbols
				case ':':
					StoreCurrentCharAndReadNext ();
					if (fCurrentChar == '=') {
						StoreCurrentCharAndReadNext ();
					}
					return new Token (TokenType.Symbol, ExtractStoredChars ());

				// *  denotes multiplication.
				// ** denotes exponents.
				case '*':
					StoreCurrentCharAndReadNext ();
					if (fCurrentChar == '*') {
						StoreCurrentCharAndReadNext ();
					}
					return new Token (TokenType.Symbol, ExtractStoredChars ());

				// =, ==. => are all valid symbols
				case '=':
					StoreCurrentCharAndReadNext ();
					if (fCurrentChar == '=' || fCurrentChar == '>') {
						StoreCurrentCharAndReadNext ();
					}
					return new Token (TokenType.Symbol, ExtractStoredChars ());

				// -  denotes subtraction.
				// -- denotes a comment.
				case '-':
					StoreCurrentCharAndReadNext ();
					if (fCurrentChar == '-') {
						StoreCurrentCharAndReadNext ();
						ReadComment ();
						return ReadNextToken ();
					}
					return new Token (TokenType.Symbol, ExtractStoredChars ());

				// If none of the above cases are satisfied then an invalid character has been read.
				default:
					CheckForUnexpectedEndOfSource ();
					ThrowInvalidCharException ();
					break;
			}

			return null;
		}
	}
}