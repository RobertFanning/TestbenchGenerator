using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using VHDLparser.ParserNodes;

namespace VHDLparser
{
	//Transforms a sequence of tokens into a tree of operators and operands.
	public class Parser
	{
		Tokenizer fTokenizer; // the tokenizer to read tokens from
		Token fCurrentToken; // the current token

		bool parseComplete;

		//Initializes a new instance of the "Parser" class.
		//The source "TextReader" to read the tokens from.
		public Parser(TextReader source)
		{
			if (source == null) throw new ArgumentNullException("source");

			fTokenizer = new Tokenizer(source);

			parseComplete = false;

			// read the first token
			ReadNextToken();
		}

		//Reads the next token.
		//After calling this method, fCurrentToken will contain the read token.
		void ReadNextToken() { fCurrentToken = fTokenizer.ReadNextToken(); }

		//Gets a value indicating whether the parser is at the end of the source.
		/// If true the parser is at the end of the source.
		bool AtEndOfSource { get { return fCurrentToken == null; } }

		//Checks for an unexpected end of the source.
		/// Throws "ParserException" The end of the source has been reached unexpectedly.
		void CheckForUnexpectedEndOfSource()
		{
			if (AtEndOfSource)
				throw new ParserException("Unexpected end of source.");
		}

		//Skips the current token if it is the specified token, or throws a "ParserException"
		//"type" is the type of token to expect.
		//"value" is the value of the token to expect.
		void SkipExpected(TokenType type, string value)
		{
			CheckForUnexpectedEndOfSource();
			if (!fCurrentToken.Equals(type, value))
				throw new ParserException("Expected '" + value + "'. But instead got '" + fCurrentToken.Value + "'.");
			ReadNextToken();
		}

		//Reads until specified token and then skips it.
		void SkipOver(TokenType type, string value)
		{
			CheckForUnexpectedEndOfSource();
			while (!fCurrentToken.Equals(type, value))
			{
				ReadNextToken();
			}
		    ReadNextToken(); //Skip specified token
		}

		//Reads the next Clause.
		//The next Clause, or null if the end of the source has been reached.
		//"ParserException" the source code is invalid.
		public Clause ReadNextClause()
		{
			
			if (AtEndOfSource || parseComplete)
				return null;

			// all the Clauses start with a word
			if (fCurrentToken.Type != TokenType.Word)
				throw new ParserException("Expected a Clause.");

			if (fCurrentToken.Value == "library")
				return ParseLibraryClause();

			if (fCurrentToken.Value == "use")
				return ParseUseClause();
			
			if (fCurrentToken.Value == "package")
				return ParsePackageDeclaration();

			if (fCurrentToken.Value == "constant")
				return ParseConstantDeclaration();

			if (fCurrentToken.Value == "type")
				return ParseTypeDeclaration();

			if (fCurrentToken.Value == "subtype")
				return ParseSubtypeDeclaration();

			if (fCurrentToken.Value == "function")
				return ParseFunctionDeclaration();

			if (fCurrentToken.Value == "entity")
				return ParseEntityDeclaration();

			if (fCurrentToken.Value == "generic")
				return ParseGenericClause();

			if (fCurrentToken.Value == "port")
				return ParsePortClause();

			return null;
		}

		UseClause ParseUseClause()
		{
			// Use-Clause:
			//   use work.intea_p.all;

			ReadNextToken(); // skip 'use'

			ReadNextToken();
			Token lLibrary = fCurrentToken;

			SkipExpected(TokenType.Symbol, "."); // skip '.'

			ReadNextToken();
			Token lPackage = fCurrentToken;

			SkipExpected(TokenType.Symbol, "."); // skip '.'

			SkipExpected(TokenType.Word, "all"); // skip '.'

			SkipExpected(TokenType.Symbol, ";"); // skip '.'
			
			return new UseClause(lLibrary, lPackage);
		}

		LibraryClause ParseLibraryClause()
		{
			// Use-Clause:
			//   use work.intea_p.all;

			ReadNextToken(); // skip 'Library'

			ReadNextToken();
			Token lLibrary = fCurrentToken;

			ReadNextToken(); // skip ';'
			
			return new LibraryClause(lLibrary);
		}

		PackageDeclaration ParsePackageDeclaration()
		{

			ReadNextToken(); // skip 'package'
			CheckForUnexpectedEndOfSource();

			if (fCurrentToken.Type != TokenType.Word)
				throw new ParserException("Expected a module name.");

			Variable packageName = new Variable(fCurrentToken.Value);

			ReadNextToken();
			SkipExpected(TokenType.Word, "is"); // skip 'is'
	

			// PARSE GENERICS IF THERE ARE ANY
			List<Clause> lClauses = new List<Clause>();
			Clause lClause;
			CheckForUnexpectedEndOfSource();
			while (!fCurrentToken.Equals(TokenType.Word, "end"))
			{
				Console.WriteLine(fCurrentToken.Value);
				if ((lClause = ReadNextClause()) != null)
					lClauses.Add(lClause);
				else throw new ParserException("Unexpected end of source.");	
				
			}
	
			ReadNextToken(); // skip 'end'
			SkipExpected(TokenType.Word, packageName.Name); // skip end {moduleName}
			SkipExpected(TokenType.Symbol, ";");

			parseComplete = true; //Trigger end of parsing.

			return new PackageDeclaration(packageName, new ClauseCollection(lClauses));
		}

		ConstantDeclaration ParseConstantDeclaration()
		{
			ReadNextToken(); //skip constant

			string identifier = fCurrentToken.Value;

			ReadNextToken(); //skip identifier
			SkipExpected(TokenType.Symbol, ":");
			
			string subtypeIndication = fCurrentToken.Value;
			ReadNextToken();

			if(fCurrentToken.Equals(TokenType.Symbol, ":="))
			{
				ReadNextToken();
				//SHOULD HAVE FUNCTION HERE THAT EXTRACTS INTEGER OR OTHERS DOWNTO 0

			}	

			//Read until end of line	-- MOVE THIS TO FUNCTION	
			while (!fCurrentToken.Equals(TokenType.Symbol, ";")) 
			{
				ReadNextToken();
			}

			ReadNextToken(); // skip ;
			
			Console.WriteLine("CONSTANT" + identifier);

			return new ConstantDeclaration(identifier, subtypeIndication, "EXPRESSION CONTAINING VALUE SHOULD GO HERE");
		}

		SubtypeDeclaration ParseSubtypeDeclaration()
		{
			ReadNextToken(); //skip subtype
			
			string identifier = fCurrentToken.Value;
			ReadNextToken(); //skip identifier
			SkipExpected(TokenType.Word, "is");

			string subtypeIndication = fCurrentToken.Value;

			//Read until end of line
			while (!fCurrentToken.Equals(TokenType.Symbol, ";")) 
			{
				ReadNextToken();
			}

			ReadNextToken(); // skip ;
			

			return new SubtypeDeclaration(identifier, subtypeIndication);
		}

		Clause ParseTypeDeclaration()
		{
			Console.WriteLine("CURRENT VALUE: " + fCurrentToken.Value);
			ReadNextToken(); //skip type
			Console.WriteLine("CURRENT VALUE: " + fCurrentToken.Value);
			string identifier = fCurrentToken.Value;
			SkipOver(TokenType.Word, "is");
			Console.WriteLine("CURRENT VALUE: " + fCurrentToken.Value);
			if(fCurrentToken.Equals(TokenType.Word, "array"))
			{
				SkipOver(TokenType.Symbol, "(");
				string from = fCurrentToken.Value;
				SkipOver(TokenType.Word, "to");
				string to = fCurrentToken.Value;
				SkipOver(TokenType.Word, "of");
				string subtypeIndication = fCurrentToken.Value;
				SkipOver(TokenType.Symbol, ";");
				return new ArrayTypeDeclaration(identifier, from, to, subtypeIndication); 
			}
			else if (fCurrentToken.Equals(TokenType.Word, "record"))
			{
				
				List<string> identifierList = new List<string>();
				List<string> subtypeIndicationList = new List<string>();

				ReadNextToken(); //skip "record"

				CheckForUnexpectedEndOfSource();

				while (!fCurrentToken.Equals(TokenType.Word, "end"))
				{
					identifierList.Add(fCurrentToken.Value);
					SkipOver(TokenType.Symbol, ":");
					subtypeIndicationList.Add(fCurrentToken.Value);
					SkipOver(TokenType.Symbol, ";");
				}
				//Skip Over: end record;
				SkipOver(TokenType.Symbol, ";");

				return new RecordTypeDeclaration(identifier, identifierList, subtypeIndicationList);
			}
			else
			{
				Console.WriteLine("ELSE Entered");
				List<string> enumerationList = new List<string>();
				SkipOver(TokenType.Symbol, "(");
				
				while (!fCurrentToken.Equals(TokenType.Symbol, ";"))
				{
					enumerationList.Add(fCurrentToken.Value);
					//Can't use skip over "," since no "," after last entry
					ReadNextToken();
					ReadNextToken();
				}

				ReadNextToken();

				return new EnumerationTypeDeclaration(identifier, enumerationList);
			}
		}

		UseClause ParseFunctionDeclaration()
		{
			ReadNextToken(); //skip function

			Token lName = fCurrentToken;
			ReadNextToken(); //Skip the function name
			SkipExpected(TokenType.Symbol, "(");

			//Waiting for the bracket to be closed again.
			//This is because function declarations can contain ';'.
			while (!fCurrentToken.Equals(TokenType.Symbol, ")")) 
			{
				ReadNextToken();
			}
			//Waiting for the end of line.
			while (!fCurrentToken.Equals(TokenType.Symbol, ";")) 
			{
				ReadNextToken();
			}

			ReadNextToken(); // skip ;
			

			return new UseClause(lName, lName);
		}

		EntityDeclaration ParseEntityDeclaration()
		{

			// entity-Clause:
			//		entity {name} is 
			//      	generic(
		    //             {gen_name}  : {type} := {value}
			//          );
			//		    port (
		    //	           {variable}	: {in/out} {type};
		    //          );
			//          end {name};
			

			ReadNextToken(); // skip 'entity'
			CheckForUnexpectedEndOfSource();

			if (fCurrentToken.Type != TokenType.Word)
				throw new ParserException("Expected a module name.");

			Variable moduleName = new Variable(fCurrentToken.Value);

			ReadNextToken();
			SkipExpected(TokenType.Word, "is"); // skip 'is'

			// PARSE GENERICS IF THERE ARE ANY
			List<Clause> lClauses = new List<Clause>();
			Clause lClause;
			CheckForUnexpectedEndOfSource();
			while (!fCurrentToken.Equals(TokenType.Word, "end"))
			{
				if ((lClause = ReadNextClause()) != null)
					lClauses.Add(lClause);
				else throw new ParserException("Unexpected end of source.");
			}
	
			ReadNextToken(); // skip 'end'
			SkipExpected(TokenType.Word, moduleName.Name); // skip end {moduleName}
			SkipExpected(TokenType.Symbol, ";");

			parseComplete = true;

			return new EntityDeclaration(moduleName, new ClauseCollection(lClauses));
		}

		GenericClause ParseGenericClause()
		{
			ReadNextToken(); // skip 'port'
			ReadNextToken(); // skip '('
			List<InterfaceElement> lInterfaceElements = new List<InterfaceElement>();
			InterfaceElement lInterfaceElement;
			CheckForUnexpectedEndOfSource();
			while (!fCurrentToken.Equals(TokenType.Symbol, ")"))
			{
				if ((lInterfaceElement = ParseGenericInterfaceElement()) != null)
					lInterfaceElements.Add(lInterfaceElement);
				else throw new ParserException("Unexpected end of source.");
			}

			ReadNextToken(); // skip ')'
			ReadNextToken(); // skip ';'

			InterfaceList GenericList = new InterfaceList(lInterfaceElements);
			//Console.WriteLine(GenericList.fElements);
			return new GenericClause(GenericList);
		}

		GenericInterfaceElement ParseGenericInterfaceElement()
		{
			
			string genericName = fCurrentToken.Value;

			ReadNextToken();
			SkipExpected(TokenType.Symbol, ":");
			

			string type = fCurrentToken.Value;

			ReadNextToken();
			SkipExpected(TokenType.Symbol, ":=");

			string value = fCurrentToken.Value;

			ReadNextToken(); // skip VALUE

			if(fCurrentToken.Equals(TokenType.Symbol, ";"))
				ReadNextToken(); // skip ';'

			return new GenericInterfaceElement(genericName, type, value);
		}


		PortClause ParsePortClause()
		{
			

			ReadNextToken(); // skip 'port'
			ReadNextToken(); // skip '('
			List<InterfaceElement> lInterfaceElements = new List<InterfaceElement>();
			InterfaceElement lInterfaceElement;
			CheckForUnexpectedEndOfSource();
			while (!fCurrentToken.Equals(TokenType.Symbol, ")"))
			{
				if ((lInterfaceElement = ParsePortInterfaceElement()) != null)
					lInterfaceElements.Add(lInterfaceElement);
				else throw new ParserException("Unexpected end of source.");
			}

			ReadNextToken(); // skip ')'
			ReadNextToken(); // skip ';'

			return new PortClause(new InterfaceList(lInterfaceElements));
		}

		PortInterfaceElement ParsePortInterfaceElement()
		{
			
			string signalName = fCurrentToken.Value;

			ReadNextToken();
			SkipExpected(TokenType.Symbol, ":");
			

			string inOut = fCurrentToken.Value;

			ReadNextToken();
			
			string type = fCurrentToken.Value;

			ReadNextToken(); // skip TYPE
			if(fCurrentToken.Equals(TokenType.Symbol, ";"))
				ReadNextToken(); // skip ';'
			
			return new PortInterfaceElement(signalName, inOut, type);
		}

	}
}