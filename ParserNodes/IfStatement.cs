using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents an if-statement.</summary>
	public class IfStatement : Statement
	{
		/// <summary>Initializes a new instance of the <see cref="IfStatement"/> class.</summary>
		/// <param name="condition">The condition of this if-statement.</param>
		/// <param name="trueBlock">The block that is executed when the condition evaluates to true.</param>
		/// <param name="falseBlock">The block that is executed when the condition evaluates to false.</param>
		public IfStatement(Expression condition, StatementCollection trueBlock, StatementCollection falseBlock)
		{
			if (condition == null) throw new ArgumentNullException("condition");
			if (trueBlock == null) throw new ArgumentNullException("trueBlock");
			if (falseBlock == null) throw new ArgumentNullException("falseBlock");

			fCondition = condition;
			fTrueBlock = trueBlock;
			fFalseBlock = falseBlock;
		}

		readonly Expression fCondition;
		/// <summary>Gets the condition of this if-statement.</summary>
		/// <value>The condition of this if-statement.</value>
		public Expression Condition { get { return fCondition; } }

		readonly StatementCollection fTrueBlock;
		/// <summary>Gets the block that is executed when the condition evaluates to true.</summary>
		/// <value>The block that is executed when the condition evaluates to true.</value>
		public StatementCollection TrueBlock { get { return fTrueBlock; } }

		readonly StatementCollection fFalseBlock;
		/// <summary>Gets the block that is executed when the condition evaluates to false.</summary>
		/// <value>The block that is executed when the condition evaluates to false.</value>
		public StatementCollection FalseBlock { get { return fFalseBlock; } }
	}
}
