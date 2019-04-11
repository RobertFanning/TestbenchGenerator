using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using myApp.ParserNodes;

namespace myApp
{
	/// <summary>Transforms a sequence of tokens into a tree of operators and operands.</summary>
	public class Parser
	{
		Tokenizer fTokenizer; // the tokenizer to read tokens from
		Token fCurrentToken; // the current token

		/// <summary>Initializes a new instance of the <see cref="Parser"/> class.</summary>
		/// <param name="source">The source <see cref="TextReader"/> to read the tokens from.</param>
		public Parser(TextReader source)
		{
			if (source == null) throw new ArgumentNullException("source");

			fTokenizer = new Tokenizer(source);

			// read the first token
			ReadNextToken();
		}

		/// <summary>Reads the next token.</summary>
		/// <remarks>After calling this method, fCurrentToken will contain the read token.</remarks>
		void ReadNextToken() { fCurrentToken = fTokenizer.ReadNextToken(); }

		/// <summary>Gets a value indicating whether the parser is at the end of the source.</summary>
		/// <value><c>true</c> if the parser is at the end of the source; otherwise, <c>false</c>.</value>
		bool AtEndOfSource { get { return fCurrentToken == null; } }

		/// <summary>Checks for an unexpected end of the source.</summary>
		/// <exception cref="ParserException">The end of the source has been reached unexpectedly.</exception>
		void CheckForUnexpectedEndOfSource()
		{
			if (AtEndOfSource)
				throw new ParserException("Unexpected end of source.");
		}

		/// <summary>Skips the current token if it is the specified token, or throws a <see cref="ParserException"/>.</summary>
		/// <param name="type">The type of token to expect.</param>
		/// <param name="value">The value of the token to expect.</param>
		void SkipExpected(TokenType type, string value)
		{
			CheckForUnexpectedEndOfSource();
			if (!fCurrentToken.Equals(type, value))
				throw new ParserException("Expected '" + value + "'.");
			ReadNextToken();
		}

		/// <summary>Reads the next <see cref="Statement"/>.</summary>
		/// <returns>The next <see cref="Statement"/>, or <c>null</c> if the end of the source has been reached.</returns>
		/// <exception cref="ParserException">The source code is invalid.</exception>
		public Statement ReadNextStatement()
		{
			if (AtEndOfSource)
				return null;

			// all the statements start with a word
			if (fCurrentToken.Type != TokenType.Word)
				throw new ParserException("Expected a statement.");

			if (fCurrentToken.Value == "if")
				return ParseIfStatement();

			if (fCurrentToken.Value == "while")
				return ParseWhileStatement();

			if (fCurrentToken.Value == "for")
				return ParseForStatement();


			//NEWLY ADDED STATEMENTS

			if (fCurrentToken.Value == "library")
				return ParseLibraryStatement();

			if (fCurrentToken.Value == "use")
				return ParseUseStatement();

			//if (fCurrentToken.Value == "entity")
			//	return ParseEntityStatement();

			//if (fCurrentToken.Value == "generic")
			//	return ParseGenericStatement();

			//if (fCurrentToken.Value == "port")
			//	return ParsePortStatement();

			return ParseAssignmentOrFunctionCallStatement();
		}

		UseStatement ParseUseStatement()
		{
			// Use-statement:
			//   use work.intea_p.all;

			ReadNextToken(); // skip 'use'

			Expression lLibrary = ParseExpression();

			SkipExpected(TokenType.Symbol, '.'); // skip '.'

			Expression lPackage = ParseExpression();

			SkipExpected(TokenType.Symbol, '.'); // skip '.'

			SkipExpected(TokenType.Word, "all"); // skip '.'

			ReadNextToken(); // skip ';'

			return new IfStatement(lLibrary, lPackage);
		}

		IfStatement ParseIfStatement()
		{
			// if-statement:
			//   if {condition} then {true-statements} end if
			//   if {condition} then {true-statements} else {false-statements} end if

			ReadNextToken(); // skip 'if'

			Expression lCondition = ParseExpression();

			SkipExpected(TokenType.Word, "then"); // skip 'then'

			List<Statement> lTrueStatements = new List<Statement>();
			List<Statement> lFalseStatements = new List<Statement>();
			List<Statement> lStatements = lTrueStatements;
			Statement lStatement;

			CheckForUnexpectedEndOfSource();
			while (!fCurrentToken.Equals(TokenType.Word, "end"))
			{
				if (fCurrentToken.Equals(TokenType.Word, "else"))
				{
					ReadNextToken(); // skip 'else'
					CheckForUnexpectedEndOfSource();
					lStatements = lFalseStatements;
				}

				if ((lStatement = ReadNextStatement()) != null)
					lStatements.Add(lStatement);
				else throw new ParserException("Unexpected end of source.");
			}

			ReadNextToken(); // skip 'end'
			SkipExpected(TokenType.Word, "if"); // skip 'if'

			return new IfStatement(lCondition
				, new StatementCollection(lTrueStatements)
				, new StatementCollection(lFalseStatements));
		}

		WhileStatement ParseWhileStatement()
		{
			// while-statement:
			//   while {condition} do {statements} end while

			ReadNextToken(); // skip 'while'

			Expression lCondition = ParseExpression();

			SkipExpected(TokenType.Word, "do"); // skip 'do'

			List<Statement> lStatements = new List<Statement>();
			Statement lStatement;
			CheckForUnexpectedEndOfSource();
			while (!fCurrentToken.Equals(TokenType.Word, "end"))
			{
				if ((lStatement = ReadNextStatement()) != null)
					lStatements.Add(lStatement);
				else throw new ParserException("Unexpected end of source.");
			}

			ReadNextToken(); // skip 'end'
			SkipExpected(TokenType.Word, "while"); // skip 'while'

			return new WhileStatement(lCondition, new StatementCollection(lStatements));
		}

		ForStatement ParseForStatement()
		{
			// for-statement:
			//   for {variable} := {start-value} to {end-value} do {statements} end for
			//   for {variable} := {start-value} to {end-value} by {step-size} do {statements} end for

			ReadNextToken(); // skip 'for'
			CheckForUnexpectedEndOfSource();

			if (fCurrentToken.Type != TokenType.Word)
				throw new ParserException("Expected a variable.");

			Variable lVariable = new Variable(fCurrentToken.Value);
			ReadNextToken();

			SkipExpected(TokenType.Symbol, ":="); // skip ':='

			Expression lStartValue = ParseExpression();

			SkipExpected(TokenType.Word, "to"); // skip 'to'

			Expression lEndValue = ParseExpression();
			CheckForUnexpectedEndOfSource();

			Expression lStepSize;
			if (fCurrentToken.Equals(TokenType.Word, "by"))
			{
				ReadNextToken(); // skip 'by'

				lStepSize = ParseExpression();
			}
			else lStepSize = new IntegerConstant(1);

			SkipExpected(TokenType.Word, "do"); // skip 'do'

			List<Statement> lStatements = new List<Statement>();
			Statement lStatement;
			CheckForUnexpectedEndOfSource();
			while (!fCurrentToken.Equals(TokenType.Word, "end"))
			{
				if ((lStatement = ReadNextStatement()) != null)
					lStatements.Add(lStatement);
				else throw new ParserException("Unexpected end of source.");
			}

			ReadNextToken(); // skip 'end'
			SkipExpected(TokenType.Word, "for"); // skip 'for'

			return new ForStatement(lVariable, lStartValue, lEndValue, lStepSize, new StatementCollection(lStatements));
		}

		Statement ParseAssignmentOrFunctionCallStatement()
		{
			Token lToken = fCurrentToken;
			
			ReadNextToken();
			CheckForUnexpectedEndOfSource();

			if (fCurrentToken.Equals(TokenType.Symbol, ":="))
				return ParseAssignment(new Variable(lToken.Value));

			if (fCurrentToken.Equals(TokenType.Symbol, "("))
				return new FunctionCallStatement(ParseFunctionCall(lToken.Value));

			throw new ParserException("Expected a statement.");
		}

		Assignment ParseAssignment(Variable variable)
		{
			// assignment:
			//   {variable} := {value}

			ReadNextToken(); // skip ':='

			return new Assignment(variable, ParseExpression());
		}

		Expression ParseExpression()
		{
			return ParseAndExpression();
		}

		Expression ParseAndExpression()
		{
			// AND-expression:
			//   {or-expression}
			//   {or-expression} and {or-expression}
			//   {or-expression} and {or-expression} and ...

			Expression lNode = ParseOrExpression();

			while (!AtEndOfSource && fCurrentToken.Equals(TokenType.Word, "and"))
			{
				ReadNextToken(); // skip 'and'
				lNode = new AndExpression(lNode, ParseOrExpression());
			}

			return lNode;
		}

		Expression ParseOrExpression()
		{
			// OR-expression:
			//   {comparison}
			//   {comparison} or {comparison}
			//   {comparison} or {comparison} or ...

			Expression lNode = ParseComparison();

			while (!AtEndOfSource && fCurrentToken.Equals(TokenType.Word, "or"))
			{
				ReadNextToken(); // skip 'or'
				lNode = new OrExpression(lNode, ParseComparison());
			}

			return lNode;
		}

		Expression ParseComparison()
		{
			// comparison:
			//   {additive-expression}
			//   {additive-expression} == {additive-expression}
			//   {additive-expression} <> {additive-expression}
			//   {additive-expression} < {additive-expression}
			//   {additive-expression} > {additive-expression}
			//   {additive-expression} <= {additive-expression}
			//   {additive-expression} >= {additive-expression}

			Expression lNode = ParseAdditiveExpression();

			if (!AtEndOfSource && fCurrentToken.Type == TokenType.Symbol)
			{
				ComparisonOperator lOperator;
				switch (fCurrentToken.Value)
				{
					case "==": lOperator = ComparisonOperator.Equal; break;
					case "<>": lOperator = ComparisonOperator.NotEqual; break;
					case "<": lOperator = ComparisonOperator.LessThan; break;
					case ">": lOperator = ComparisonOperator.GreaterThan; break;
					case "<=": lOperator = ComparisonOperator.LessThanOrEqual; break;
					case ">=": lOperator = ComparisonOperator.GreaterThanOrEqual; break;
					default: return lNode;
				}

				ReadNextToken(); // skip comparison operator
				return new Comparison(lOperator, lNode, ParseAdditiveExpression());
			}
			else return lNode;
		}

		Expression ParseAdditiveExpression()
		{
			// additive expression:
			//   {multiplicative-expression}
			//   {multiplicative-expression} [+ or -] {multiplicative-expression}
			//   {multiplicative-expression} [+ or -] {multiplicative-expression} [+ or -] ...

			Expression lNode = ParseMultiplicativeExpression();

			while (!AtEndOfSource)
			{
				if (fCurrentToken.Equals(TokenType.Symbol, "+"))
				{
					ReadNextToken(); // skip '+'
					lNode = new Addition(lNode, ParseMultiplicativeExpression());
				}
				else if (fCurrentToken.Equals(TokenType.Symbol, "-"))
				{
					ReadNextToken(); // skip '-'
					lNode = new Subtraction(lNode, ParseMultiplicativeExpression());
				}
				else break;
			}

			return lNode;
		}

		Expression ParseMultiplicativeExpression()
		{
			// multiplicative expression:
			//   {unary-expression}
			//   {unary-expression} [* or /] {unary-expression}
			//   {unary-expression} [* or /] {unary-expression} [* or /] ...

			Expression lNode = ParseUnaryExpression();

			while (!AtEndOfSource)
			{
				if (fCurrentToken.Equals(TokenType.Symbol, "*"))
				{
					ReadNextToken(); // skip '*'
					lNode = new Multiplication(lNode, ParseUnaryExpression());
				}
				else if (fCurrentToken.Equals(TokenType.Symbol, "/"))
				{
					ReadNextToken(); // skip '/'
					lNode = new Division(lNode, ParseUnaryExpression());
				}
				else break;
			}

			return lNode;
		}

		Expression ParseUnaryExpression()
		{
			// unary expression:
			//   {base-expression}
			//   -{base-expression}
			//   not {base-expression}

			CheckForUnexpectedEndOfSource();
			if (fCurrentToken.Equals(TokenType.Symbol, "-"))
			{
				ReadNextToken(); // skip '-'
				return new Negation(ParseBaseExpression());
			}
			else if (fCurrentToken.Equals(TokenType.Word, "not"))
			{
				ReadNextToken(); // skip 'not'
				return new NotExpression(ParseBaseExpression());
			}
			else if (fCurrentToken.Equals(TokenType.Symbol, "+"))
				ReadNextToken(); // skip '+'
			
			return ParseBaseExpression();
		}

		Expression ParseBaseExpression()
		{
			CheckForUnexpectedEndOfSource();

			switch (fCurrentToken.Type)
			{
				case TokenType.Integer: return ParseIntegerConstant();
				case TokenType.String: return ParseStringConstant();
				case TokenType.Word: return ParseVariableOrFunctionCall();
				default: // TokenType.Symbol
					if (fCurrentToken.Value == "(")
						return ParseGroupExpression();
					else throw new ParserException("Expected an expression.");
			}
		}

		Expression ParseGroupExpression()
		{
			ReadNextToken(); // skip '('

			Expression lExpression = ParseExpression();
			
			SkipExpected(TokenType.Symbol, ")"); // skip ')'
			
			return lExpression;
		}

		Expression ParseVariableOrFunctionCall()
		{
			string lName = fCurrentToken.Value;

			ReadNextToken();

			if (!AtEndOfSource && fCurrentToken.Equals(TokenType.Symbol, "("))
				return ParseFunctionCall(lName);
			else return new Variable(lName);
		}

		Expression ParseStringConstant()
		{
			string lValue = fCurrentToken.Value;
			ReadNextToken(); // skip string constant
			return new StringConstant(lValue);
		}

		private Expression ParseIntegerConstant()
		{
			int lValue;
			if (int.TryParse(fCurrentToken.Value, out lValue))
			{
				ReadNextToken(); // skip integer constant
				return new IntegerConstant(lValue);
			}
			else throw new ParserException("Invalid integer constant " + fCurrentToken.Value);
		}

		FunctionCall ParseFunctionCall(string name)
		{
			// function call:
			//   {function-name} ( )
			//   {function-name} ( {expression} )
			//   {function-name} ( {expression}, {expression}, ... )

			ReadNextToken(); // skip '('
			CheckForUnexpectedEndOfSource();

			List<Expression> lArguments = new List<Expression>();
			if (!fCurrentToken.Equals(TokenType.Symbol, ")"))
			{
				lArguments.Add(ParseExpression());
				CheckForUnexpectedEndOfSource();

				while (fCurrentToken.Equals(TokenType.Symbol, ","))
				{
					ReadNextToken(); // skip ','
					lArguments.Add(ParseExpression());
					CheckForUnexpectedEndOfSource();
				}

				if (!fCurrentToken.Equals(TokenType.Symbol, ")"))
					throw new ParserException("Expected ')'.");
			}

			ReadNextToken(); // skip ')'

			return new FunctionCall(name, lArguments.ToArray());
		}
	}
}