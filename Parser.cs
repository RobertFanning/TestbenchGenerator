using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using VHDLparser.ParserNodes;

namespace VHDLparser 
{
	// The Parser class contains the vast functionality of this VHDLparser.
	public class Parser 
	{
		//----------------------------------------------------------------------------------------
		//                            Class Objects and Constructor
		
		// Tokenizer is required for the deconstruction of the input code into discrete tokens.
		Tokenizer fTokenizer; 
		// Current token created to store the current token that has been read.
		Token fCurrentToken;
		// lib of type MyLibrary contains the operations of all possible predefined VHDL functions.
		MyLibrary lib;

		SubtypeDeclaration[] PredefinedTypes = { new SubtypeDeclaration ("std_ulogic", new SubtypeIndication ("std_ulogic", 0, 0))};

		PortClause fPortmap;
		public PortClause Portmap { get { return fPortmap; } }

		// Below are a series of lists for storing declarations parsed from a package that may need to be referenced later.
		// ConstantList, is a list of constant declarations.
		List<ConstantDeclaration> ConstantList;
		public List<ConstantDeclaration> FetchConstant { get { return ConstantList; } }

		// SubtypeList, is a list of subtype declarations.
		List<SubtypeDeclaration> SubtypeList;
		// There are multiple versions of type declarations each using different definition structures, therefore requiring separated classes and lists.
		List<ArrayTypeDeclaration> ArrayTypeList;

		List<EnumerationTypeDeclaration> EnumerationTypeList;

		string entityName;

		public string Entity  { get { return entityName; } }

		List<RecordTypeDeclaration> RecordTypeList;

		public List<RecordTypeDeclaration> RecordType { get { return RecordTypeList; } }
		// Constructor requires a source to be parsed.
		// All objects declared above are created.

		public Parser (TextReader source) 
		{
			if (source == null) throw new ArgumentNullException ("source");

			fTokenizer = new Tokenizer (source);

			fPortmap = new PortClause(null);

			lib = new MyLibrary ();

			ConstantList = new List<ConstantDeclaration> ();

			SubtypeList = new List<SubtypeDeclaration> ();

			ArrayTypeList = new List<ArrayTypeDeclaration> ();

			EnumerationTypeList = new List<EnumerationTypeDeclaration> ();

			RecordTypeList = new List<RecordTypeDeclaration> ();

			// First token is read to commence the parsing processs.
			ReadNextToken ();
		}

		//----------------------------------------------------------------------------------------
		//                                   Helper Methods

		// Reads the next token.
		// After calling this method, fCurrentToken will contain the read token.
		void ReadNextToken () { fCurrentToken = fTokenizer.ReadNextToken (); }

		// If true the parser is at the end of the source.
		bool AtEndOfSource { get { return fCurrentToken == null; } }

		// Checks for an unexpected end of the source.
		// Throws "ParserException" The end of the source has been reached unexpectedly.
		void CheckForUnexpectedEndOfSource () 
		{
			if (AtEndOfSource)
				throw new ParserException ("Unexpected end of source.");
		}

		
		// This method is for determining the specified attribute for a type.
		// Currently only 3 attributes have been defined (left, right and length).
		// Attributes can be called on types or subtypes, therefore cases for each are specified below.
		int DetermineAttribute (string Attribute, string Type) 
		{
			// It is first determined whether the subtype passed exists in the list of subtypes.
			if (SubtypeList.Exists (x => x.Identifier == Type)) 
			{
				// The subtype is found and stored in chosenType.
				SubtypeDeclaration chosenType = SubtypeList.Find (x => x.Identifier == Type);
				// The correct attribute is fetched depending on the passed string.
				if (Attribute == "left")
					return chosenType.Left;

				if (Attribute == "right")
					return chosenType.Right;

				if (Attribute == "length")
					return (Math.Abs (chosenType.Left - chosenType.Right) + 1);

				throw new ParserException ("Attribute Provided is not recognised");
			} else if (EnumerationTypeList.Exists (x => x.Identifier == Type)) {
				EnumerationTypeDeclaration chosenType = EnumerationTypeList.Find (x => x.Identifier == Type);
				if (Attribute == "left")
					return chosenType.Left;

				if (Attribute == "right")
					return chosenType.Right;

				if (Attribute == "length")
					return (Math.Abs (chosenType.Left - chosenType.Right) + 1);

				throw new ParserException ("Attribute Provided is not recognised");
			}
			// If the type passed is neither a subtype or a type an exception is thrown. 
			else 
			{
				throw new ParserException ("Cannot determine attribute, Type/Subtype not recognized.");
			}
		}

		// ParseNextNode parses the input source determining if a node is present.
		// The nodes represent the different VHDL constructs. The nodes are identified by their starting token.
		// Below 10 nodes have been difined, the tokens used to identify these should be the first token of each new node.
		public ParserNode ParseNextNode () 
		{
			Console.WriteLine(fCurrentToken.Value);

			if (AtEndOfSource)
				return null;
			
			// all the Clauses start with a word
			if (fCurrentToken.Type != TokenType.Word)
				throw new ParserException ("Expected a Clause.");

			if (fCurrentToken.Value == "library")
				return ParseLibraryClause ();

			if (fCurrentToken.Value == "use")
				return ParseUseClause ();

			if (fCurrentToken.Value == "package")
				return ParsePackageDeclaration ();

			if (fCurrentToken.Value == "constant")
				return ParseConstantDeclaration ();

			if (fCurrentToken.Value == "type")
				return ParseTypeDeclaration ();

			if (fCurrentToken.Value == "subtype")
				return ParseSubtypeDeclaration ();

			if (fCurrentToken.Value == "function")
				return ParseFunctionDeclaration ();

			if (fCurrentToken.Value == "entity")
				return ParseEntityDeclaration ();

			if (fCurrentToken.Value == "generic")
				return ParseGenericClause ();

			if (fCurrentToken.Value == "port")
				return ParsePortClause ();

			return null;
		}

		// Use clauses have a strict structure in VHDL, and are defined as:
		//                       use prefix.suffix
		// The prefix contains the library where the package is located and,
		// the packages name. Following that is the suffix, which in most 
		// cases is all.
		UseClause ParseUseClause () 
		{
			// The "use" node is skipped.
			ReadNextToken (); 
			// The library is extracted.
			string lLibrary = fCurrentToken.Value;
			// The library token is skipped along with the neighbouring '.'
			fCurrentToken = fCurrentToken = fTokenizer.SkipOver (TokenType.Symbol, "."); 
			// The package is extracted.
			string lPackage = fCurrentToken.Value;
			// The package token is skipped along with the neighbouring '.'
			fCurrentToken = fTokenizer.SkipOver (TokenType.Symbol, "."); 
			// The "all" typically present is skipped, this may need to be adjusted.
			fCurrentToken = fTokenizer.SkipExpected (TokenType.Word, "all"); // skip '.'

			fCurrentToken = fTokenizer.SkipExpected (TokenType.Symbol, ";"); // skip '.'
			// The parsed use clause is created and returned.
			return new UseClause (lLibrary, lPackage);
		}

		// Library clauses too have a strict structure they obey, this is:
		//                      library logical_name
		// The logical_name is just a simple identifier of the library.
		LibraryClause ParseLibraryClause () 
		{
			// The "library" node is skipped.
			ReadNextToken ();
			// The "library" identifier is extracted.
			string lLibrary = fCurrentToken.Value;
			// The rest of the line is read until the end.
			fCurrentToken = fTokenizer.SkipOver (TokenType.Symbol, ";"); 
			// The parsed library clause is created and returned.
			return new LibraryClause (lLibrary);
		}

		
		//---------------------------------------------------------------------------------------
		//Subtype Indication parsing 27 - Apr - 2019

		SubtypeIndication ParseSubtypeIndication () {
			//SubTypeIndication: [ resolution_function_name ] type_mark [ constraint ]
			// $type($upper downto $lower) || $type range $lower to $upper
			string type = fCurrentToken.Value;
			ReadNextToken ();
			//Either type X downto Y
			if (fCurrentToken.Equals (TokenType.Symbol, "(")) {
				ReadNextToken (); //Skip parenthesis since its only opening parenthesis.
				Node left = ParseExpression ();
				fCurrentToken = fTokenizer.SkipExpected (TokenType.Word, "downto");
				Node right = ParseExpression ();
				SubtypeIndication ParsedSubtype = new SubtypeIndication (type, left.Eval(), right.Eval());

				return ParsedSubtype;
			}
			//Else range Y to X
			else {
				fCurrentToken = fTokenizer.SkipExpected (TokenType.Word, "range");
				Node left = ParseExpression ();
				fCurrentToken = fTokenizer.SkipExpected (TokenType.Word, "to");
				Node right = ParseExpression ();
				SubtypeIndication ParsedSubtype = new SubtypeIndication (type, left.Eval(), right.Eval());

				return ParsedSubtype;
			}
		}

		//---------------------------------------------------------------------------------------

		PackageDeclaration ParsePackageDeclaration () {

			ReadNextToken (); // skip 'package'
			CheckForUnexpectedEndOfSource ();

			if (fCurrentToken.Type != TokenType.Word)
				throw new ParserException ("Expected a module name.");

			Variable packageName = new Variable (fCurrentToken.Value);

			ReadNextToken ();
			fCurrentToken = fTokenizer.SkipExpected (TokenType.Word, "is"); // skip 'is'

			// PARSE GENERICS IF THERE ARE ANY
			CheckForUnexpectedEndOfSource ();
			while (!fCurrentToken.Equals (TokenType.Word, "end")) {

				if (ParseNextNode () == null)
					throw new ParserException ("Unexpected end of source.");

			}

			ReadNextToken (); // skip 'end'
			fCurrentToken = fTokenizer.SkipExpected (TokenType.Word, packageName.Name); // skip end {moduleName}
			fCurrentToken = fTokenizer.SkipExpected (TokenType.Symbol, ";");

			ConstantList.ForEach (Console.WriteLine);
			// Package has been parsed, now the next file can be parsed
			fCurrentToken = fTokenizer.SkipOver(TokenType.Word, "EndOfFileIdentifier");
			foreach (ConstantDeclaration constant in ConstantList) {
				Console.WriteLine (constant.Identifier);
			}

			return new PackageDeclaration (packageName);
		}

		ConstantDeclaration ParseConstantDeclaration () {
			ReadNextToken (); //skip constant

			string identifier = fCurrentToken.Value;

			ReadNextToken (); //skip identifier
			fCurrentToken = fTokenizer.SkipExpected (TokenType.Symbol, ":");

			string subtypeIndication = fCurrentToken.Value;
			ReadNextToken ();

			// In a package, a constant may be deferred. This means its value is defined in the package body. the value may be changed by re-analysing only the package body. we do not want this
			if (!fCurrentToken.Equals (TokenType.Symbol, ":=")) {
				throw new ParserException ("Constants value definition must not be deferred to body");
			}

			ReadNextToken ();
			Node result = ParseExpression ();
			ConstantDeclaration ParsedConstant = new ConstantDeclaration (identifier, subtypeIndication, result.Eval ());
			ConstantList.Add (ParsedConstant);
			ReadNextToken ();
			return ParsedConstant;

		}

		SubtypeDeclaration ParseSubtypeDeclaration () {
			ReadNextToken (); //skip subtype

			string identifier = fCurrentToken.Value;
			ReadNextToken (); //skip identifier
			fCurrentToken = fTokenizer.SkipExpected (TokenType.Word, "is");

			SubtypeIndication subtype = ParseSubtypeIndication ();

			//Read until end of line
			fCurrentToken = fTokenizer.SkipExpected (TokenType.Symbol, ";");

			SubtypeDeclaration ParsedSubtype = new SubtypeDeclaration (identifier, subtype);
			SubtypeList.Add (ParsedSubtype);

			return ParsedSubtype;
		}

		SignalType ParseTypeDeclaration () {

			ReadNextToken (); //skip type

			string identifier = fCurrentToken.Value;
			fCurrentToken = fTokenizer.SkipOver (TokenType.Word, "is");

			if (fCurrentToken.Equals (TokenType.Word, "array")) {
				fCurrentToken = fTokenizer.SkipOver (TokenType.Symbol, "(");
				string from = fCurrentToken.Value;
				fCurrentToken = fTokenizer.SkipOver (TokenType.Word, "to");
				string to = fCurrentToken.Value;
				fCurrentToken = fTokenizer.SkipOver (TokenType.Word, "of");
				string subtypeIndication = fCurrentToken.Value;
				fCurrentToken = fTokenizer.SkipOver (TokenType.Symbol, ";");
				ArrayTypeDeclaration ParsedArrayType = new ArrayTypeDeclaration (identifier, from, to, subtypeIndication);
				ArrayTypeList.Add (ParsedArrayType);
				return ParsedArrayType;
			} else if (fCurrentToken.Equals (TokenType.Word, "record")) {

				List<string> identifierList = new List<string> ();
				List<string> subtypeIndicationList = new List<string> ();

				ReadNextToken (); //skip "record"

				CheckForUnexpectedEndOfSource ();

				while (!fCurrentToken.Equals (TokenType.Word, "end")) {
					identifierList.Add (fCurrentToken.Value);
					fCurrentToken = fTokenizer.SkipOver (TokenType.Symbol, ":");
					subtypeIndicationList.Add (fCurrentToken.Value);
					fCurrentToken = fTokenizer.SkipOver (TokenType.Symbol, ";");
				}
				//Skip Over: end record;
				fCurrentToken = fTokenizer.SkipOver (TokenType.Symbol, ";");
				RecordTypeDeclaration ParsedRecordType = new RecordTypeDeclaration (identifier, identifierList, subtypeIndicationList);
				RecordTypeList.Add (ParsedRecordType);
				return ParsedRecordType;
			} else {

				List<string> enumerationList = new List<string> ();
				fCurrentToken = fTokenizer.SkipOver (TokenType.Symbol, "(");

				while (!fCurrentToken.Equals (TokenType.Symbol, ";")) {
					enumerationList.Add (fCurrentToken.Value);
					//Can't use skip over "," since no "," after last entry
					ReadNextToken ();
					ReadNextToken ();
				}

				ReadNextToken ();
				EnumerationTypeDeclaration ParsedEnumerationType = new EnumerationTypeDeclaration (identifier, enumerationList);
				EnumerationTypeList.Add (ParsedEnumerationType);
				return ParsedEnumerationType;
			}
		}

		// NEED TO FIX
		UseClause ParseFunctionDeclaration () {
			ReadNextToken (); //skip function

			Token lName = fCurrentToken;
			ReadNextToken (); //Skip the function name
			fCurrentToken = fTokenizer.SkipExpected (TokenType.Symbol, "(");

			//Waiting for the bracket to be closed again.
			//This is because function declarations can contain ';'.
			while (!fCurrentToken.Equals (TokenType.Symbol, ")")) {
			ReadNextToken ();
			}
			//Waiting for the end of line.
			while (!fCurrentToken.Equals (TokenType.Symbol, ";")) {
				ReadNextToken ();
			}

			ReadNextToken (); // skip ;

			return new UseClause (lName.Value, lName.Value);
		}

		EntityDeclaration ParseEntityDeclaration () {

			// entity-Clause:
			//		entity {name} is 
			//      	generic(
			//             {gen_name}  : {type} := {value}
			//          );
			//		    port (
			//	           {variable}	: {in/out} {type};
			//          );
			//          end {name};

			ReadNextToken (); // skip 'entity'
			CheckForUnexpectedEndOfSource ();

			if (fCurrentToken.Type != TokenType.Word)
				throw new ParserException ("Expected a module name.");

			string moduleName = new string (fCurrentToken.Value);

			entityName = moduleName;

			ReadNextToken ();
			fCurrentToken = fTokenizer.SkipExpected (TokenType.Word, "is"); // skip 'is'

			// PARSE GENERICS IF THERE ARE ANY
			List<ParserNode> lParserNodes = new List<ParserNode> ();
			ParserNode lParserNode;
			CheckForUnexpectedEndOfSource ();
			while (!fCurrentToken.Equals (TokenType.Word, "end")) {
				if ((lParserNode = ParseNextNode ()) != null)
					lParserNodes.Add (lParserNode);
				else throw new ParserException ("Unexpected end of source.");
			}

			ReadNextToken (); // skip 'end'
			fCurrentToken = fTokenizer.SkipExpected (TokenType.Word, moduleName); // skip end {moduleName}
			fCurrentToken = fTokenizer.SkipExpected (TokenType.Symbol, ";");

			// Package has been parsed, now the next file can be parsed
			fCurrentToken = fTokenizer.SkipOver(TokenType.Word, "EndOfFileIdentifier");

			return new EntityDeclaration (moduleName, new ParserNodeCollection (lParserNodes));
		}

		GenericClause ParseGenericClause () {
			ReadNextToken (); // skip 'port'
			ReadNextToken (); // skip '('
			List<InterfaceElement> lInterfaceElements = new List<InterfaceElement> ();
			InterfaceElement lInterfaceElement;
			CheckForUnexpectedEndOfSource ();
			while (!fCurrentToken.Equals (TokenType.Symbol, ")")) {
				if ((lInterfaceElement = ParseGenericInterfaceElement ()) != null)
					lInterfaceElements.Add (lInterfaceElement);
				else throw new ParserException ("Unexpected end of source.");
			}

			ReadNextToken (); // skip ')'
			ReadNextToken (); // skip ';'

			InterfaceList GenericList = new InterfaceList (lInterfaceElements);

			return new GenericClause (GenericList);
		}

		GenericInterfaceElement ParseGenericInterfaceElement () {

			string genericName = fCurrentToken.Value;

			ReadNextToken ();
			fCurrentToken = fTokenizer.SkipExpected (TokenType.Symbol, ":");

			string type = fCurrentToken.Value;

			ReadNextToken ();
			fCurrentToken = fTokenizer.SkipExpected (TokenType.Symbol, ":=");

			string value = fCurrentToken.Value;

			ReadNextToken (); // skip VALUE

			if (fCurrentToken.Equals (TokenType.Symbol, ";"))
				ReadNextToken (); // skip ';'

			return new GenericInterfaceElement (genericName, type, value);
		}

		PortClause ParsePortClause () {

			ReadNextToken (); // skip 'port'
			ReadNextToken (); // skip '('
			List<PortInterfaceElement> lPortInterfaceElements = new List<PortInterfaceElement> ();
			PortInterfaceElement lPortInterfaceElement; 
			CheckForUnexpectedEndOfSource ();
			while (!fCurrentToken.Equals (TokenType.Symbol, ")")) {
				if ((lPortInterfaceElement = ParsePortInterfaceElement ()) != null)
					lPortInterfaceElements.Add (lPortInterfaceElement);
				else throw new ParserException ("Unexpected end of source.");
			}

			ReadNextToken (); // skip ')'
			ReadNextToken (); // skip ';'

			fPortmap = new PortClause (lPortInterfaceElements);
			fPortmap.ExtractInterfaces ();
			fPortmap.ExtractClockReset ();

			return fPortmap;
		}

		PortInterfaceElement ParsePortInterfaceElement () {

			string signalName = fCurrentToken.Value;

			ReadNextToken ();
			fCurrentToken = fTokenizer.SkipExpected (TokenType.Symbol, ":");

			string inOut = fCurrentToken.Value;

			ReadNextToken ();

			string type = fCurrentToken.Value;
			
			ReadNextToken (); // skip TYPE
			if (fCurrentToken.Equals (TokenType.Symbol, ";"))
				ReadNextToken (); // skip ';'
			

			SignalType fType;

			if ((fType = RecordTypeList.Find(item => item.Identifier == type)) != null)
				return new PortInterfaceElement (signalName, inOut, type, fType);
			else if ((fType = SubtypeList.Find(item => item.Identifier == type)) != null)
				return new PortInterfaceElement (signalName, inOut, type, fType);
			else if ((fType = ArrayTypeList.Find(item => item.Identifier == type)) != null)
				return new PortInterfaceElement (signalName, inOut, type, fType);
			else if ((fType = EnumerationTypeList.Find(item => item.Identifier == type)) != null)
				return new PortInterfaceElement (signalName, inOut, type, fType);
			else if ((fType = Array.Find(PredefinedTypes, item => item.Identifier == type)) != null)
				return new PortInterfaceElement (signalName, inOut, type, fType);
			else
				throw new ParserException ("Signal Type is unknown. It exists neither in the Package file or in the Predifined types.");

			

			
		}

		//----------------------------------------------------------------------------------------
		//                                   Expression Parsing
		//
		// The following methods are required for expression parsing. Expressions appear in both
		// constant declarations and value definitions for types and subtypes. Anywhere where
		// the user may put a value in VHDL, they can instead describe that value as an 
		// expression. Expressions require a different pasing approach since they do not follow
		// the left to right structure of other nodes such as clauses and type declarations. 
		// Instead expressions have a prioritized order of execution. Operations are not computed
		// from left to right but instead obeying this priority.

		// ParseExpression is the first method called when an expression requires parsing.
		// This method initializes the parsing and returns the computed expression value.
		Node ParseExpression () 
		{
			// Addition and subtraction are the operations with the least priority, therefore
			// process of parsing and expression is commenced with those operations.
			var expr = ParseAddSubtract ();
			// The closing parenthesis is skipped if it is present at the end of an expression.
			// This may occur in instances such as an array of expression. 
			fCurrentToken = fTokenizer.SkipIfPresent (TokenType.Symbol, ")");
			// Check if the entire expression until the end of line or a reserved word has been parsed
			// Expressions may be separated either by a line termination or a reserved word.
			if (!(fCurrentToken.Equals (TokenType.Symbol, ";") || ReservedWords.IsReservedWord (fCurrentToken.Value)))
				throw new ParserException ("Expected ; and end of line");

			return expr;

		}

		// ParseAddSubtract is the least prioritized and thererfore called first.
		Node ParseAddSubtract () 
		{
			// The lefthand side of the operation is parsed with the next higher prioritized operation.
			// Multiplication and division is a priority level above addition and subtraction.
			var lhs = ParseMultiplyDivide ();
			// The while loop below contiues infinitely until no more operations are to be parsed.
			while (true) 
			{
				// The operation (add/sub) is determined by the symbol.
				Func<int, int, int> op = null;
				if (fCurrentToken.Equals (TokenType.Symbol, "+")) 
				{
					op = (a, b) => a + b;
				} else if (fCurrentToken.Equals (TokenType.Symbol, "-")) 
				{
					op = (a, b) => a - b;
				}

				// If there are no further operations the lefthand side is returned.
				if (op == null)
					return lhs;

				// The operator is skipped.
				ReadNextToken ();
				// Now the righthand side of the operation is parsed for the operation of the higher priority level.
				var rhs = ParseMultiplyDivide ();

				// The lefthand side value is updated with the computed operation value.
				lhs = new NodeBinary (lhs, rhs, op);
			}
		}

		// ParseMultiplyDivide is of the next level of priority.
		// The general structure of this method is the same as for addition and subtraction,
		// except the method now calls the next priority level above which are exponents.
		Node ParseMultiplyDivide () 
		{
			var lhs = ParseExponent ();

			while (true) 
			{
				Func<int, int, int> op = null;
				if (fCurrentToken.Equals (TokenType.Symbol, "*")) 
				{
					op = (a, b) => a * b;
				} 
				else if (fCurrentToken.Equals (TokenType.Symbol, "/")) 
				{
					op = (a, b) => a / b;
				}

				if (op == null)
					return lhs; 

				ReadNextToken ();

				// The righthandside is now parsed of any exponents.
				var rhs = ParseExponent ();

				lhs = new NodeBinary (lhs, rhs, op);
			}
		}

		// ParseExponent is of the next level of priority above ParseMultiplyDivide.
		Node ParseExponent () 
		{
			var lhs = ParseUnary ();

			while (true) 
			{
				Func<int, int, int> op = null;
				if (fCurrentToken.Equals (TokenType.Symbol, "**")) 
				{
					op = (a, b) => (int) Math.Pow (a, b);
				}

				if (op == null)
					return lhs; 

				ReadNextToken ();

				var rhs = ParseUnary ();

				lhs = new NodeBinary (lhs, rhs, op);
			}
		}

		// Unary operations are the operation with the highest priority.
		// These must be computed before any other operations. 
		// Unary operations are when values are first negated or made positive.
		Node ParseUnary () 
		{
			// If values are preceded by a '+' we do not change the value.
			// Therefore the token is skipped and parse unary is called again.
			if (fCurrentToken.Equals (TokenType.Symbol, "+")) 
			{
				ReadNextToken ();
				return ParseUnary ();
			}
			// If values are preceded by a '-' we negate the value.
			if (fCurrentToken.Equals (TokenType.Symbol, "-")) 
			{
				ReadNextToken ();

				var rhs = ParseUnary ();

				return new NodeUnary (rhs, (a) => -a);
			}

			// At this stage we have parsed for all operations and therefore only need
			// to extract the values to be used in these operations. This is done using
			// ParseLeaf.
			return ParseLeaf ();
		}

		// ParseLeaf is the lowest level of the expression parsers.
		// It is used to extract the fundamental elements of an expression.
		// These elements are numbers, function calls and arrays.
		Node ParseLeaf () 
		{
			// NodeNumber:
			// If current token is a number we simply extract it as a NodeNumber.
			if (fCurrentToken.Type == TokenType.Integer) 
			{
				var node = new NodeNumber (fCurrentToken.IntValue ());
				ReadNextToken ();
				return node;
			}

			// Parenthesis:
			// If an open parenthesis is read, it must be skipped and the expression
			// it encloses must be parsed. We must also check again at the end of the
			// expression if the parenthesis is correctly closed otherwise an exception
			// is thrown.		
			if (fCurrentToken.Equals (TokenType.Symbol, "(")) 
			{
				// Skip '('
				ReadNextToken ();

				// Check for "others" statement that is commonly used in VHDL.
				if (fCurrentToken.Equals (TokenType.Word, "others")) 
				{
					fCurrentToken = fTokenizer.SkipOver (TokenType.Symbol, "'");
					var node = new NodeNumber (fCurrentToken.IntValue ());
					fCurrentToken = fTokenizer.SkipOver (TokenType.Symbol, ")");
					return node;
				} 
				// Else we have an expression inside of parenthesis. 
				else 
				{
					// Parse a top-level expression
					var arguments = new List<Node> ();
					while (true) 
					{
						// Parse argument and add to list
						arguments.Add (ParseAddSubtract ());

						// Is there another argument?
						if (fCurrentToken.Equals (TokenType.Symbol, ",")) 
						{
							ReadNextToken ();
							continue;
						}

						// Get out
						break;
					}

					// Check and skip ')'
					if (!fCurrentToken.Equals (TokenType.Symbol, ")"))
						throw new ParserException ("Missing close parenthesis");
					ReadNextToken ();

					// Return
					return new NodeArray (arguments.ToArray ());
				}
			}

			// Single/Double Quote Literals:
			// In vhdl we can have single digit values enclosed in signle quote literals and longer values
			// enclosed in double quote literals. We must therefore extract the number from within these quotation literals. 
			if (fCurrentToken.Equals (TokenType.Symbol, "\"")  || fCurrentToken.Equals (TokenType.Symbol, "\'")) 
			{
				// Skip '"'
				ReadNextToken (); //Read the number

				var node = new NodeNumber (fCurrentToken.IntValue ());

				ReadNextToken (); //Skip the number

				// Check and skip ')'
				if (!(fCurrentToken.Equals (TokenType.Symbol, "\"") || fCurrentToken.Equals (TokenType.Symbol, "\'")))
					throw new ParserException ("Missing closing quote literals");
				ReadNextToken ();

				// Return
				return node;

			}

			if (fCurrentToken.Type == TokenType.Word) 
			{
				// Parse a top-level expression
				var name = fCurrentToken.Value;
				ReadNextToken ();
				if (fCurrentToken.Equals (TokenType.Symbol, "\'")) 
				{
					//Type Attribute has been requested
					ReadNextToken ();
					var attribute = DetermineAttribute (fCurrentToken.Value, name);
					var node = new NodeNumber (attribute);
					ReadNextToken (); //Skip the found attribute
					return node;

				} 
				else if (!fCurrentToken.Equals (TokenType.Symbol, "(")) 
				{
					ConstantDeclaration result = ConstantList.Find (x => x.Identifier == name);
					var node = new NodeNumber (result.Value);
					return node;

				} 
				else 
				{
					// Function call
					// Skip parens
					ReadNextToken ();

					// Parse arguments
					var arguments = new List<Node> ();
					while (true) 
					{
						// Parse argument and add to list
						arguments.Add (ParseAddSubtract ());

						// Is there another argument?
						if (fCurrentToken.Equals (TokenType.Symbol, ",")) 
						{
							ReadNextToken ();
							continue;
						}

						// Get out
						break;
					}
					// Check and skip ')'
					if (!fCurrentToken.Equals (TokenType.Symbol, ")"))
						throw new ParserException ("Missing Closing Parenthesis");

					ReadNextToken ();

					// Create the function call node
					return new NodeFunctionCall (name, arguments.ToArray ());
				}
			}
			// Don't Understand
			throw new ParserException ("Expected ; and end of line");
		}
	}
}


