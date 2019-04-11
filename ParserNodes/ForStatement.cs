using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.ParserNodes
{
	/// <summary>Represents a for-loop.</summary>
	public class ForStatement : Statement
	{
		/// <summary>Initializes a new instance of the <see cref="ForStatement"/> class.</summary>
		/// <param name="variable">The variable that is incremented each iteration.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="stepSize">The size of the step to increment by.</param>
		public ForStatement(Variable variable, Expression startValue, Expression endValue, Expression stepSize, StatementCollection block)
		{
			if (variable == null) throw new ArgumentNullException("variable");
			if (startValue == null) throw new ArgumentNullException("startValue");
			if (endValue == null) throw new ArgumentNullException("endValue");
			if (stepSize == null) throw new ArgumentNullException("stepSize");
			if (block == null) throw new ArgumentNullException("block");

			fVariable = variable;
			fStartValue = startValue;
			fEndValue = endValue;
			fStepSize = stepSize;
			fBlock = block;
		}

		readonly Variable fVariable;
		/// <summary>Gets the variable that is incremented each iteration.</summary>
		/// <value>The variable that is incremented each iteration.</value>
		public Variable Variable { get { return fVariable; } }

		readonly Expression fStartValue;
		/// <summary>Gets the start value.</summary>
		/// <value>The start value.</value>
		public Expression StartValue { get { return fStartValue; } }

		readonly Expression fEndValue;
		/// <summary>Gets the end value.</summary>
		/// <value>The end value.</value>
		public Expression EndValue { get { return fEndValue; } }

		readonly Expression fStepSize;
		/// <summary>Gets the size of the step to increment by.</summary>
		/// <value>The size of the step to increment by.</value>
		public Expression StepSize { get { return fStepSize; } }

		readonly StatementCollection fBlock;
		/// <summary>Gets the block that is executed each iteration.</summary>
		/// <value>The block that is executed each iteration.</value>
		public StatementCollection Block { get { return fBlock; } }
	}
}
