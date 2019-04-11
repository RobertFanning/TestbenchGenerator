using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents a while-loop.</summary>
	public class WhileStatement : Statement
	{
		/// <summary>Initializes a new instance of the <see cref="WhileStatement"/> class.</summary>
		/// <param name="condition">The condition of this while-statement.</param>
		/// <param name="block">The block that is executed each iteration.</param>
		public WhileStatement(Expression condition, StatementCollection block)
		{
			if (condition == null) throw new ArgumentNullException("condition");
			if (block == null) throw new ArgumentNullException("block");

			fCondition = condition;
			fBlock = block;
		}

		readonly Expression fCondition;
		/// <summary>Gets the condition of this while-statement.</summary>
		/// <value>The condition of this while-statement.</value>
		public Expression Condition { get { return fCondition; } }

		readonly StatementCollection fBlock;
		/// <summary>Gets the block that is executed each iteration.</summary>
		/// <value>The block that is executed each iteration.</value>
		public StatementCollection Block { get { return fBlock; } }
	}
}
