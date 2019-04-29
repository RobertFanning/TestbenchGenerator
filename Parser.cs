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

		MyLibrary lib;
		ReflectionContext ctx;


		//Lists for storing Parameters extracted from package
		List<ConstantDeclaration>         ConstantList;

		List<SubtypeDeclaration>          SubtypeList;

		List<ArrayTypeDeclaration>        ArrayTypeList;

		List<EnumerationTypeDeclaration>  EnumerationTypeList;

		List<RecordTypeDeclaration>       RecordTypeList;



		//Initializes a new instance of the "Parser" class.
		//The source "TextReader" to read the tokens from.
		public Parser(TextReader source)
		{
			if (source == null) throw new ArgumentNullException("source");

			fTokenizer = new Tokenizer(source);

			parseComplete = false;

			//Library of known methods
			lib = new MyLibrary();

			ctx = new ReflectionContext(lib);

			ConstantList = new List<ConstantDeclaration>();

			SubtypeList = new List<SubtypeDeclaration>();

			ArrayTypeList = new List<ArrayTypeDeclaration>();

			EnumerationTypeList = new List<EnumerationTypeDeclaration>();

			RecordTypeList = new List<RecordTypeDeclaration>();

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

		public ParserNode ParseNextNode()
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


		//use_clause ::= use selected_name { , selected_name } ;
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

		//library_clause ::= library logical_name_list ;
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

        //----------------------------------24-04-2019-------------------------------------------
		//Parse Expression:
		//expressions 
		//Integer: if the first value is a number followed by ; we have an int

		//Constant: if the first value is a word followed by ; 

		//Function: first value is word followed by ( 

		//Array: first value is ( followed by strings separated by ,

		//Otheres: ( others =>

		Node ParseExpression()
		{
		   // For the moment, all we understand is add and subtract
            var expr = ParseAddSubtract();

			Console.WriteLine(fCurrentToken.Value);
            // Check if the entire expression until the end of line or a reserved word has been parsed
            if (!(fCurrentToken.Equals(TokenType.Symbol, ";") || ReservedWords.IsReservedWord(fCurrentToken.Value)))
                throw new ParserException("Expected ; and end of line");

            return expr;	

		
		}


				// Parse an sequence of add/subtract operators
		Node ParseAddSubtract()
		{
			// Parse the left hand side
			var lhs = ParseMultiplyDivide();

			while (true)
			{
				// Work out the operator
				Func<int, int, int> op = null;
				if (fCurrentToken.Equals(TokenType.Symbol, "+"))
				{
					op = (a, b) => a + b;
				}
				else if (fCurrentToken.Equals(TokenType.Symbol, "-"))
				{
					op = (a, b) => a - b;
				}

				// Binary operator found?
				if (op == null)
					return lhs;             // no
		
				// Skip the operator
				ReadNextToken();

				// Parse the right hand side of the expression
				var rhs = ParseMultiplyDivide();

				// Create a binary node and use it as the left-hand side from now on
				lhs = new NodeBinary(lhs, rhs, op);
			}
		}


        // Parse an sequence of add/subtract operators
        Node ParseMultiplyDivide()
        {
            // Parse the left hand side
            var lhs = ParseExponent();

            while (true)
            {
                // Work out the operator
                Func<int, int, int> op = null;
                if (fCurrentToken.Equals(TokenType.Symbol, "*"))
                {
                    op = (a, b) => a * b;
                }
                else if (fCurrentToken.Equals(TokenType.Symbol, "/"))
                {
                    op = (a, b) => a / b;
                }

                // Binary operator found?
                if (op == null)
                    return lhs;             // no

                // Skip the operator
                ReadNextToken();

                // Parse the right hand side of the expression
                var rhs = ParseExponent();

                // Create a binary node and use it as the left-hand side from now on
                lhs = new NodeBinary(lhs, rhs, op);
            }
        }

		Node ParseExponent()
        {
            // Parse the left hand side
            var lhs = ParseUnary();

            while (true)
            {
                // Work out the operator
                Func<int, int, int> op = null;
                if (fCurrentToken.Equals(TokenType.Symbol, "**"))
                {
                    op = (a, b) => (int) Math.Pow(a, b);
                }

                // Binary operator found?
                if (op == null)
                    return lhs;             // no

                // Skip the operator
                ReadNextToken();

                // Parse the right hand side of the expression
                var rhs = ParseUnary();

                // Create a binary node and use it as the left-hand side from now on
                lhs = new NodeBinary(lhs, rhs, op);
            }
        }

		Node ParseUnary()
		{
			// Positive operator is a no-op so just skip it
			if (fCurrentToken.Equals(TokenType.Symbol, "+"))
			{
				// Skip
				ReadNextToken();
				return ParseUnary();
			}

			// Negative operator
			if (fCurrentToken.Equals(TokenType.Symbol, "-"))
			{
				// Skip
				ReadNextToken();

				// Parse RHS 
				// Note this recurses to self to support negative of a negative
				var rhs = ParseUnary();

				// Create unary node
				return new NodeUnary(rhs, (a) => -a);
			}

			// No positive/negative operator so parse a leaf node
			return ParseLeaf();
		}


		// Parse a leaf node
		// (For the moment this is just a number)
		Node ParseLeaf()
		{
			// Is it a number?

			if (fCurrentToken.Type == TokenType.Integer)
			{
				var node = new NodeNumber(fCurrentToken.IntValue());
				ReadNextToken();
				return node;
			}

            // Parenthesis?
            if (fCurrentToken.Equals(TokenType.Symbol, "("))
            {
                // Skip '('
                ReadNextToken();

				if(fCurrentToken.Equals(TokenType.Word, "others"))
				{
					SkipOver(TokenType.Symbol, "'");
					var node = new NodeNumber(fCurrentToken.IntValue());
					SkipOver(TokenType.Symbol, ")");
					return node;
				}
				else
				{
					// Parse a top-level expression
                	var node = ParseAddSubtract();
                

                	// Check and skip ')'
					if (!fCurrentToken.Equals(TokenType.Symbol, ")"))
						throw new ParserException("Missing close parenthesis");
					ReadNextToken();

					// Return
					return node;
				}
            }

			//
            if (fCurrentToken.Equals(TokenType.Symbol, "\""))
            {
                // Skip '"'
                ReadNextToken(); //Read the number
				
				var node = new NodeNumber(fCurrentToken.IntValue());

				ReadNextToken(); //Skip the number

				// Check and skip ')'
				if (!fCurrentToken.Equals(TokenType.Symbol, "\""))
					throw new ParserException("Missing close asterisk");
				ReadNextToken();

				// Return
				return node;
				
            }

			if (fCurrentToken.Type == TokenType.Word)
            {
                // Parse a top-level expression
				var name = fCurrentToken.Value;
				ReadNextToken();

				if (!fCurrentToken.Equals(TokenType.Symbol, "("))
            	{
					ConstantDeclaration result = ConstantList.Find(x => x.Identifier == name);
					var node = new NodeNumber(result.Value);
					return node;
				}
				else
				{
					// Function call
                    // Skip parens
                    ReadNextToken();

                    // Parse arguments
                    var arguments = new List<Node>();
                    while (true)
                    {
                        // Parse argument and add to list
                        arguments.Add(ParseAddSubtract());

                        // Is there another argument?
                        if (fCurrentToken.Equals(TokenType.Symbol, ","))
                        {
                            ReadNextToken();
                            continue;
                        }

                        // Get out
                        break;
					}

					// Check and skip ')'
                    if (!fCurrentToken.Equals(TokenType.Symbol, ")"))
                        throw new ParserException("Missing Closing Parenthesis");
                    ReadNextToken();

                    // Create the function call node
                    return new NodeFunctionCall(name, arguments.ToArray());
            	}
			}
			// Don't Understand
			throw new ParserException("Expected ; and end of line");
		}
	
		//---------------------------------------------------------------------------------------
		//Subtype Indication parsing 27 - Apr - 2019

		SubtypeIndication ParseSubtypeIndication()
		{
			//SubTypeIndication: [ resolution_function_name ] type_mark [ constraint ]
			// $type($upper downto $lower) || $type range $lower to $upper
			string type = fCurrentToken.Value;
			ReadNextToken();
			//Either type X downto Y
			if(fCurrentToken.Equals(TokenType.Symbol, "("))
			{
				ReadNextToken(); //Skip parenthesis since its only opening parenthesis.
				Node upper = ParseExpression();
				SkipExpected(TokenType.Word, "downto");
				Node lower = ParseExpression();
				SubtypeIndication ParsedSubtype = new SubtypeIndication(type, upper, lower);

				return ParsedSubtype;
			}
			//Else range Y to X
			else
			{
				SkipExpected(TokenType.Word, "range");
				Node lower = ParseExpression();
				SkipExpected(TokenType.Word, "to");
				Node upper = ParseExpression();
				SubtypeIndication ParsedSubtype = new SubtypeIndication(type, upper, lower);

				return ParsedSubtype;
			}
		}

		//---------------------------------------------------------------------------------------

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
			CheckForUnexpectedEndOfSource();
			while (!fCurrentToken.Equals(TokenType.Word, "end"))
			{
			
				if (ParseNextNode() == null)
					throw new ParserException("Unexpected end of source.");	
				
			}
			
			ReadNextToken(); // skip 'end'
			SkipExpected(TokenType.Word, packageName.Name); // skip end {moduleName}
			SkipExpected(TokenType.Symbol, ";");

			parseComplete = true; //Trigger end of parsing.

			ConstantList.ForEach(Console.WriteLine);
			
			foreach(ConstantDeclaration constant in ConstantList)
			{
				Console.WriteLine(constant.Identifier);
			}

			return new PackageDeclaration(packageName);
		}

		ConstantDeclaration ParseConstantDeclaration()
		{
			ReadNextToken(); //skip constant

			string identifier = fCurrentToken.Value;

			ReadNextToken(); //skip identifier
			SkipExpected(TokenType.Symbol, ":");
			
			string subtypeIndication = fCurrentToken.Value;
			ReadNextToken();

			// In a package, a constant may be deferred. This means its value is defined in the package body. the value may be changed by re-analysing only the package body. we do not want this
			if(!fCurrentToken.Equals(TokenType.Symbol, ":="))
			{
				throw new ParserException("Constants value definition must not be deferred to body");
			}

			ReadNextToken();
			Node result = ParseExpression();
			ConstantDeclaration ParsedConstant = new ConstantDeclaration(identifier, subtypeIndication, result.Eval(ctx));
			ConstantList.Add(ParsedConstant);
			ReadNextToken();
			return ParsedConstant;

		}

		SubtypeDeclaration ParseSubtypeDeclaration()
		{
			ReadNextToken(); //skip subtype
			
			string identifier = fCurrentToken.Value;
			ReadNextToken(); //skip identifier
			SkipExpected(TokenType.Word, "is");

			SubtypeIndication subtype = ParseSubtypeIndication();


			//Read until end of line
			SkipExpected(TokenType.Symbol, ";");


			SubtypeDeclaration ParsedSubtype = new SubtypeDeclaration(identifier, subtype);
			SubtypeList.Add(ParsedSubtype);

			return ParsedSubtype;
		}

		Declaration ParseTypeDeclaration()
		{
			
			ReadNextToken(); //skip type
		
			string identifier = fCurrentToken.Value;
			SkipOver(TokenType.Word, "is");
	
			if(fCurrentToken.Equals(TokenType.Word, "array"))
			{
				SkipOver(TokenType.Symbol, "(");
				string from = fCurrentToken.Value;
				SkipOver(TokenType.Word, "to");
				string to = fCurrentToken.Value;
				SkipOver(TokenType.Word, "of");
				string subtypeIndication = fCurrentToken.Value;
				SkipOver(TokenType.Symbol, ";");
				ArrayTypeDeclaration ParsedArrayType = new ArrayTypeDeclaration(identifier, from, to, subtypeIndication);
				ArrayTypeList.Add(ParsedArrayType);
				return ParsedArrayType; 
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
				RecordTypeDeclaration ParsedRecordType = new RecordTypeDeclaration(identifier, identifierList, subtypeIndicationList);
				RecordTypeList.Add(ParsedRecordType);
				return ParsedRecordType;
			}
			else
			{
		
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
				EnumerationTypeDeclaration ParsedEnumerationType = new EnumerationTypeDeclaration(identifier, enumerationList);
				EnumerationTypeList.Add(ParsedEnumerationType);
				return ParsedEnumerationType;
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
			List<ParserNode> lParserNodes = new List<ParserNode>();
			ParserNode lParserNode;
			CheckForUnexpectedEndOfSource();
			while (!fCurrentToken.Equals(TokenType.Word, "end"))
			{
				if ((lParserNode = ParseNextNode()) != null)
					lParserNodes.Add(lParserNode);
				else throw new ParserException("Unexpected end of source.");
			}
	
			ReadNextToken(); // skip 'end'
			SkipExpected(TokenType.Word, moduleName.Name); // skip end {moduleName}
			SkipExpected(TokenType.Symbol, ";");

			parseComplete = true;

			return new EntityDeclaration(moduleName, new ParserNodeCollection(lParserNodes));
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