using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a while-loop.</summary>
	public class WhileClause : Clause
	{
		/// <summary>Initializes a new instance of the <see cref="WhileClause"/> class.</summary>
		/// <param name="condition">The condition of this while-Clause.</param>
		/// <param name="block">The block that is executed each iteration.</param>
		public WhileClause(Expression condition, ClauseCollection block)
		{
			if (condition == null) throw new ArgumentNullException("condition");
			if (block == null) throw new ArgumentNullException("block");

			fCondition = condition;
			fBlock = block;
		}

		readonly Expression fCondition;
		/// <summary>Gets the condition of this while-Clause.</summary>
		/// <value>The condition of this while-Clause.</value>
		public Expression Condition { get { return fCondition; } }

		readonly ClauseCollection fBlock;
		/// <summary>Gets the block that is executed each iteration.</summary>
		/// <value>The block that is executed each iteration.</value>
		public ClauseCollection Block { get { return fBlock; } }
	}
}
