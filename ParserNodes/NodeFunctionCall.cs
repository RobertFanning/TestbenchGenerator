using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
    public class NodeFunctionCall : Node
    {
        public NodeFunctionCall(string functionName, Node[] arguments)
        {
            _functionName = functionName;
            _arguments = arguments;
        }

        string _functionName;
        Node[] _arguments;

        public override int Eval(IContext ctx)
        {
            // Evaluate all arguments
            var argVals = new int[_arguments.Length];
            for (int i=0; i<_arguments.Length; i++)
            {
                argVals[i] = _arguments[i].Eval(ctx);
            }

            // Call the function
            return ctx.CallFunction(_functionName, argVals);
        }
    }
}
