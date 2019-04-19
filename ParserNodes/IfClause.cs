using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents an if-Clause.</summary>
	public class IfClause : Clause
	{
		/// <summary>Initializes a new instance of the <see cref="IfClause"/> class.</summary>
		/// <param name="condition">The condition of this if-Clause.</param>
		/// <param name="trueBlock">The block that is executed when the condition evaluates to true.</param>
		/// <param name="falseBlock">The block that is executed when the condition evaluates to false.</param>
		public IfClause(Expression condition, ClauseCollection trueBlock, ClauseCollection falseBlock)
		{
			if (condition == null) throw new ArgumentNullException("condition");
			if (trueBlock == null) throw new ArgumentNullException("trueBlock");
			if (falseBlock == null) throw new ArgumentNullException("falseBlock");

			fCondition = condition;
			fTrueBlock = trueBlock;
			fFalseBlock = falseBlock;
		}

		readonly Expression fCondition;
		/// <summary>Gets the condition of this if-Clause.</summary>
		/// <value>The condition of this if-Clause.</value>
		public Expression Condition { get { return fCondition; } }

		readonly ClauseCollection fTrueBlock;
		/// <summary>Gets the block that is executed when the condition evaluates to true.</summary>
		/// <value>The block that is executed when the condition evaluates to true.</value>
		public ClauseCollection TrueBlock { get { return fTrueBlock; } }

		readonly ClauseCollection fFalseBlock;
		/// <summary>Gets the block that is executed when the condition evaluates to false.</summary>
		/// <value>The block that is executed when the condition evaluates to false.</value>
		public ClauseCollection FalseBlock { get { return fFalseBlock; } }
	}
}
