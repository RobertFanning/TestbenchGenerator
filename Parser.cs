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
		
		// LexicalAnalyser is required for the deconstruction of the input code into discrete tokens.
		LexicalAnalyser fLexicalAnalyser; 
		// Current token created to store the current token that has been read.
		Token fCurrentToken;
		// lib of type MyLibrary contains the operations of all possible predefined VHDL functions.
		MyLibrary lib;

		SubtypeDeclaration[] PredefinedTypes = { new SubtypeDeclaration ("std_ulogic", new SubtypeIndication ("std_ulogic", 0, 0)), new SubtypeDeclaration ("natural", new SubtypeIndication ("natural", 0, 0))};

		public SubtypeDeclaration[] Predefined { get { return PredefinedTypes; } }

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
		public List<ArrayTypeDeclaration> ArrayType { get { return ArrayTypeList; } }

		List<EnumerationTypeDeclaration> EnumerationTypeList;

		string entityName;

		public string Entity  { get { return entityName; } }

		List<RecordTypeDeclaration> RecordTypeList;
		List<RecordTypeDeclaration> ConstantRecordTypeList;

		public List<RecordTypeDeclaration> RecordType { get { return RecordTypeList; } }
		// Constructor requires a source to be parsed.
		// All objects declared above are created.

		public Parser (TextReader source) 
		{
			if (source == null) throw new ArgumentNullException ("source");

			fLexicalAnalyser = new LexicalAnalyser (source);

			fPortmap = new PortClause(null, null);

			lib = new MyLibrary ();

			ConstantList = new List<ConstantDeclaration> ();

			SubtypeList = new List<SubtypeDeclaration> ();

			ArrayTypeList = new List<ArrayTypeDeclaration> ();

			EnumerationTypeList = new List<EnumerationTypeDeclaration> ();

			RecordTypeList = new List<RecordTypeDeclaration> ();

			ConstantRecordTypeList = new List<RecordTypeDeclaration> ();

			// First token is read to commence the parsing processs.
			ReadNextToken ();
		}

		//----------------------------------------------------------------------------------------
		//                                   Helper Methods

		// Reads the next token.
		// After calling this method, fCurrentToken will contain the read token.
		void ReadNextToken () { fCurrentToken = fLexicalAnalyser.ReadNextToken (); }

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

				if (Attribute == "high")
					return ((chosenType.Right >= chosenType.Left) ? chosenType.Right : chosenType.Left);
					
				if (Attribute == "low")
					return (chosenType.Right >= chosenType.Left ? chosenType.Left : chosenType.Right);

				if (Attribute == "length")
					return (Math.Abs (chosenType.Left - chosenType.Right) + 1);

				throw new ParserException ("Attribute Provided is not recognised:  " + Attribute);
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


		SignalType FindDefinedType () 
		{
			SignalType fType;
			do
			{
				if ((fType = RecordTypeList.Find(item => item.Identifier == fCurrentToken.Value)) != null) break;

				if ((fType = SubtypeList.Find(item => item.Identifier == fCurrentToken.Value)) != null) break;

				if ((fType = ArrayTypeList.Find(item => item.Identifier == fCurrentToken.Value)) != null) break;

				if ((fType = EnumerationTypeList.Find(item => item.Identifier == fCurrentToken.Value)) != null) break;

				if ((fType = Array.Find(PredefinedTypes, item => item.Identifier == fCurrentToken.Value)) != null) break;

			} while (false);

			return fType;
		}

		// ParseNextNode parses the input source determining if a node is present.
		// The nodes represent the different VHDL constructs. The nodes are identified by their starting token.
		// Below 10 nodes have been difined, the tokens used to identify these should be the first token of each new node.
		public ParserNode ParseNextNode () 
		{

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

			if (fCurrentToken.Value == "signal")
				return ParseSignal ();

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
			fCurrentToken = fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Symbol, "."); 
			// The package is extracted.
			string lPackage = fCurrentToken.Value;
			// The package token is skipped along with the neighbouring '.'
			fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Symbol, "."); 
			// The "all" typically present is skipped, this may need to be adjusted.
			fCurrentToken = fLexicalAnalyser.SkipExpected (TokenType.Word, "all"); // skip '.'

			fCurrentToken = fLexicalAnalyser.SkipExpected (TokenType.Symbol, ";"); // skip '.'
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
			fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Symbol, ";"); 
			// The parsed library clause is created and returned.
			return new LibraryClause (lLibrary);
		}

		//This is a temporary method for handling signal definitions in packages.
		// e.g. signal db_param: intea_pif_in_t;
		LibraryClause ParseSignal () 
		{
			// The "library" node is skipped.
			ReadNextToken ();
			// The "library" identifier is extracted.
			string lLibrary = fCurrentToken.Value;
			// The rest of the line is read until the end.
			fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Symbol, ";"); 
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
				fCurrentToken = fLexicalAnalyser.SkipExpected (TokenType.Word, "downto");
				Node right = ParseExpression ();
				SubtypeIndication ParsedSubtype = new SubtypeIndication (type, left.Eval(), right.Eval());
				return ParsedSubtype;
			}
			//Else range Y to X
			else {
				fCurrentToken = fLexicalAnalyser.SkipExpected (TokenType.Word, "range");
				Node left = ParseExpression ();
				fCurrentToken = fLexicalAnalyser.SkipExpected (TokenType.Word, "to");
				Node right = ParseExpression ();
				var bitsRight = Math.Log((right.Eval()+1), 2);
				var bitsLeft = Math.Log((left.Eval()+1), 2);
				SubtypeIndication ParsedSubtype = new SubtypeIndication (type, (int)Math.Ceiling(bitsLeft), 31);

				return ParsedSubtype;
			}
		}

		//---------------------------------------------------------------------------------------

		PackageDeclaration ParsePackageDeclaration () {

			ReadNextToken (); // skip 'package'
			CheckForUnexpectedEndOfSource ();

			if (fCurrentToken.Type != TokenType.Word)
				throw new ParserException ("Expected a module name.");

			string packageName = fCurrentToken.Value;

			ReadNextToken ();
			fCurrentToken = fLexicalAnalyser.SkipExpected (TokenType.Word, "is"); // skip 'is'

			// PARSE GENERICS IF THERE ARE ANY
			CheckForUnexpectedEndOfSource ();
			while (!fCurrentToken.Equals (TokenType.Word, "end")) {

				if (ParseNextNode () == null){
					
					throw new ParserException ("Unexpected end of source.");
				}

			}

			// I have removed the two lines belows since sometimes the package definition will end with end package name; and othertimes just end name;
			ReadNextToken (); // skip 'end'
			
			fCurrentToken = fLexicalAnalyser.SkipOver(TokenType.Word, "EndOfFileIdentifier");

			// Package has been parsed, now the next file can be parsed
			return new PackageDeclaration (packageName);
		}
		ConstantDeclaration ParseConstantDeclaration () {
			ReadNextToken (); //skip constant

			string identifier = fCurrentToken.Value;
			ReadNextToken (); //skip identifier
			fCurrentToken = fLexicalAnalyser.SkipExpected (TokenType.Symbol, ":");

			string subtypeIndication = fCurrentToken.Value;
			ReadNextToken ();


			
			//FIX 25.06.19: This is a fix to allow for constants of record type. This issue was realised when parsing intea_pif_p.vhd
			//Need to make class for record type constant
			if (RecordTypeList.Any (x => x.Identifier == subtypeIndication)){
				RecordTypeDeclaration recordConstant = RecordTypeList.Find (x => x.Identifier == subtypeIndication);
				ConstantRecordTypeList.Add (recordConstant);
				ConstantDeclaration ParsedConstant = new ConstantDeclaration (identifier, null, 0);
				fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Symbol, ";");
				return ParsedConstant;
			}
			else {
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

		}

		SubtypeDeclaration ParseSubtypeDeclaration () {
			ReadNextToken (); //skip subtype
			string identifier = fCurrentToken.Value;
			ReadNextToken (); //skip identifier
			fCurrentToken = fLexicalAnalyser.SkipExpected (TokenType.Word, "is");

			SubtypeIndication subtype = ParseSubtypeIndication ();

			//Read until end of line
			fCurrentToken = fLexicalAnalyser.SkipExpected (TokenType.Symbol, ";");

			SubtypeDeclaration ParsedSubtype = new SubtypeDeclaration (identifier, subtype);
			SubtypeList.Add (ParsedSubtype);

			return ParsedSubtype;
		}

		SignalType ParseTypeDeclaration () {

			ReadNextToken (); //skip type
			SignalType fType;

			string identifier = fCurrentToken.Value;
			fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Word, "is");

			if (fCurrentToken.Equals (TokenType.Word, "array")) {
				fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Symbol, "(");
				Node from = ParseExpression ();
				//Used to have skip over "to" here but it can also be "downto"
				ReadNextToken ();
			    Node to = ParseExpression ();
				fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Word, "of");
				string subtypeIndication = fCurrentToken.Value;
				fType = FindDefinedType ();
				fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Symbol, ";");

				ArrayTypeDeclaration ParsedArrayType = new ArrayTypeDeclaration (identifier, from.Eval(), to.Eval(), fType);
				ArrayTypeList.Add (ParsedArrayType);
				return ParsedArrayType;
			} else if (fCurrentToken.Equals (TokenType.Word, "record")) {

				List<string> identifierList = new List<string> ();
				List<SignalType> subtypeIndicationList = new List<SignalType> ();
				ReadNextToken (); //skip "record"

				CheckForUnexpectedEndOfSource ();

				while (!fCurrentToken.Equals (TokenType.Word, "end")) {
					identifierList.Add (fCurrentToken.Value);
					fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Symbol, ":");
					//Searches for the type of each record entry.
					fType = FindDefinedType (); 
					subtypeIndicationList.Add (fType);
					fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Symbol, ";");
				}
				//Skip Over: end record;
				fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Symbol, ";");

				

				RecordTypeDeclaration ParsedRecordType = new RecordTypeDeclaration (identifier, identifierList, subtypeIndicationList);
				RecordTypeList.Add (ParsedRecordType);
				return ParsedRecordType;
			} else {

				List<string> enumerationList = new List<string> ();
				fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Symbol, "(");

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

		UseClause ParseFunctionDeclaration () {
			ReadNextToken (); //skip function

			Token lName = fCurrentToken;
			ReadNextToken (); //Skip the function name
			fCurrentToken = fLexicalAnalyser.SkipExpected (TokenType.Symbol, "(");

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

			ReadNextToken (); // skip 'entity'
			CheckForUnexpectedEndOfSource ();

			if (fCurrentToken.Type != TokenType.Word)
				throw new ParserException ("Expected a module name.");

			string moduleName = new string (fCurrentToken.Value);

			entityName = moduleName;

			ReadNextToken ();
			fCurrentToken = fLexicalAnalyser.SkipExpected (TokenType.Word, "is"); // skip 'is'

			// PARSE GENERICS IF THERE ARE ANY
			List<ParserNode> lParserNodes = new List<ParserNode> ();
			ParserNode lParserNode;
			CheckForUnexpectedEndOfSource ();
			while (!fCurrentToken.Equals (TokenType.Word, "end")) {
				if ((lParserNode = ParseNextNode ()) != null)
					lParserNodes.Add (lParserNode);
				else throw new ParserException ("Unexpected end of source.");
			}
		
			fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Symbol, ";");

			// Package has been parsed, now the next file can be parsed
			fCurrentToken = fLexicalAnalyser.SkipOver(TokenType.Word, "EndOfFileIdentifier");

			return new EntityDeclaration (moduleName, lParserNodes);
		}

		GenericClause ParseGenericClause () {
			ReadNextToken (); // skip 'port'
			ReadNextToken (); // skip '('
			List<InterfaceElement> lInterfaceElements = new List<InterfaceElement> ();
			InterfaceElement lInterfaceElement;
			CheckForUnexpectedEndOfSource ();
			while (!fCurrentToken.Equals (TokenType.Word, "port")) {
				if ((lInterfaceElement = ParseGenericInterfaceElement ()) != null)
				{
					lInterfaceElements.Add (lInterfaceElement);
					fCurrentToken = fLexicalAnalyser.SkipIfPresent(TokenType.Symbol, ";");
				}
				else throw new ParserException ("Unexpected end of source.");
			}

			return new GenericClause (lInterfaceElements);
		}

		GenericInterfaceElement ParseGenericInterfaceElement () 
		{

			string genericName = fCurrentToken.Value;
			ReadNextToken ();
			fCurrentToken = fLexicalAnalyser.SkipExpected (TokenType.Symbol, ":");

			string type = fCurrentToken.Value;

			ReadNextToken ();

			if (fCurrentToken.Equals (TokenType.Word, "range")) 
			{
				ReadNextToken();
				Node left = ParseExpression ();
				fCurrentToken = fLexicalAnalyser.SkipExpected (TokenType.Word, "to");
				Node right = ParseExpression ();
				var bitsRight = Math.Log((right.Eval()+1), 2);
				var bitsLeft = Math.Log((left.Eval()+1), 2);
				return new GenericInterfaceElement (genericName, type,  (int)Math.Ceiling(bitsRight) - (int)Math.Ceiling(bitsLeft));
			}
			else if (fCurrentToken.Equals (TokenType.Symbol, ":=")) 
			{
				ReadNextToken();
				Node left = ParseExpression ();
				
				return new GenericInterfaceElement (genericName, type, left.Eval());

			}
			else
			{
				throw new ParserException ("Constants value definition must not be deferred to body");
			}
		}
		
		PortClause ParsePortClause () {

			ReadNextToken (); // skip 'port'
			ReadNextToken (); // skip '('
			RecordTypeDeclaration fType;
			List<PortInterfaceElement> lPortInterfaceElements = new List<PortInterfaceElement> ();
			List<RecordTypeDeclaration> UnpackedList = new List<RecordTypeDeclaration> ();
			PortInterfaceElement lPortInterfaceElement; 
			CheckForUnexpectedEndOfSource ();
			while (!fCurrentToken.Equals (TokenType.Symbol, ")")) {
				if ((lPortInterfaceElement = ParsePortInterfaceElement ()) != null){
					lPortInterfaceElements.Add (lPortInterfaceElement);
					//Create list of all unpacked types so the methods specific to record types can be utilized.
					if ((fType = RecordTypeList.Find(item => item.Identifier == lPortInterfaceElement.Type)) != null)
						UnpackedList.Add (fType);
				}
				else throw new ParserException ("Unexpected end of source.");
			}

			ReadNextToken (); // skip ')'
			ReadNextToken (); // skip ';'

			//Remove all duplicated from list of Unpacked types.
			UnpackedList = UnpackedList.Distinct().ToList();

			fPortmap = new PortClause (lPortInterfaceElements, UnpackedList);
			fPortmap.ExtractInterfaces ();

			return fPortmap;
		}

		PortInterfaceElement ParsePortInterfaceElement () {
			Boolean ArrayElement = false;
			string signalName = "";
			string inOut = "";
			string type = "";
			SignalType fType;
			
			signalName = fCurrentToken.Value;
			ReadNextToken ();
			fCurrentToken = fLexicalAnalyser.SkipExpected (TokenType.Symbol, ":");
			inOut = fCurrentToken.Value;
			ReadNextToken ();
			type = fCurrentToken.Value;
			if (type.Contains("_arr"))
				ArrayElement = true;

			ReadNextToken (); // skip TYPE
			if (fCurrentToken.Equals (TokenType.Symbol, ";"))
				ReadNextToken (); // skip ';'

			if ((fType = RecordTypeList.Find(item => item.Identifier == type)) != null)
				return new PortInterfaceElement (signalName, inOut, type, fType, ArrayElement);
			else if ((fType = SubtypeList.Find(item => item.Identifier == type)) != null)
				return new PortInterfaceElement (signalName, inOut, type, fType, ArrayElement);
			else if ((fType = ArrayTypeList.Find(item => item.Identifier == type)) != null)
				return new PortInterfaceElement (signalName, inOut, type, fType, ArrayElement);
			else if ((fType = EnumerationTypeList.Find(item => item.Identifier == type)) != null)
				return new PortInterfaceElement (signalName, inOut, type, fType, ArrayElement);
			else if ((fType = Array.Find(PredefinedTypes, item => item.Identifier == type)) != null)
				return new PortInterfaceElement (signalName, inOut, type, fType, ArrayElement);
			else
				throw new ParserException ("Signal Type is unknown. It exists neither in the Package file or in the Predifined types." + type);
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
			fCurrentToken = fLexicalAnalyser.SkipIfPresent (TokenType.Symbol, ")");
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
			var lhs = ParseUnary ();
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
				var rhs = ParseUnary ();

				// The lefthand side value is updated with the computed operation value.
				lhs = new NodeBinary (lhs, rhs, op);
			}
		}

		// Unary operations are the operation with the highest priority.
		// These must be computed before any other operations. 
		// Unary operations are when values are first negated or made positive.
		Node ParseUnary () 
		{
			
			while (true) 
			{
				// If values are preceded by a '+' we do not change the value.
				// Therefore the token is skipped and parse unary is called again.
				if (fCurrentToken.Equals (TokenType.Symbol, "+")) 
				{
					ReadNextToken ();

					var rhs = ParseMultiplyDivide ();

					return new NodeUnary (rhs, (a) => +a);
				}
				// If values are preceded by a '-' we negate the value.
				else if (fCurrentToken.Equals (TokenType.Symbol, "-")) 
				{
					ReadNextToken ();

					var rhs = ParseMultiplyDivide ();

					return new NodeUnary (rhs, (a) => -a);
				}
				else
				{
					var lhs = ParseMultiplyDivide ();
					return lhs;
				}		
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
			var lhs = ParseLeaf ();
			Func<int, int, int> op = null;
			if (fCurrentToken.Equals (TokenType.Symbol, "**")) 
			{
				op = (a, b) => (int) Math.Pow (a, b);
			}

			if (op == null)
			{
				// At this stage we have parsed for all operations and therefore only need
				// to extract the values to be used in these operations. This is done using
				// ParseLeaf.
				return lhs;
			}

			ReadNextToken ();

			var rhs = ParseExponent ();

			return new NodeBinary (lhs, rhs, op);
		
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
					fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Symbol, "'");
					var node = new NodeNumber (fCurrentToken.IntValue ());
					fCurrentToken = fLexicalAnalyser.SkipOver (TokenType.Symbol, ")");
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
					if (ConstantList.Any (x => x.Identifier == name)){
						ConstantDeclaration result = ConstantList.Find (x => x.Identifier == name);
						var node = new NodeNumber (result.Value);
						return node;
					}
					else if (ConstantList.Any (x => x.Identifier == name.ToUpper())){
						ConstantDeclaration result = ConstantList.Find (x => x.Identifier == name.ToUpper());
						var node = new NodeNumber (result.Value);
						return node;
					}
					else if(ConstantList.Any (x => x.Identifier == name.ToLower())){
						ConstantDeclaration result = ConstantList.Find (x => x.Identifier.ToUpper() == name.ToLower());
						var node = new NodeNumber (result.Value);
						return node;
					}
					else
						throw new ParserException ("Constant value is unknown.");
						
					

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
			throw new ParserException ("Expected ; and end of line but instead got: " + fCurrentToken.Value);
		}
	}
}


