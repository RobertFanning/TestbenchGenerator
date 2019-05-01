using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace VHDLparser.ParserNodes
{
    public class NodeFunctionCall : Node
    {
        public NodeFunctionCall(string functionName, Node[] arguments)
        {
            _functionName = functionName;
            _arguments = arguments;
            _targetObject = new MyLibrary();
        }

        string _functionName;
        Node[] _arguments;
        MyLibrary _targetObject;

        public override int Eval()
        {
            // Evaluate all arguments
            var argVals = new int[_arguments.Length];
            for (int i=0; i<_arguments.Length; i++)
            {
                argVals[i] = _arguments[i].Eval();
            }

            var mi = _targetObject.GetType().GetMethod(_functionName);
            if (mi == null)
                throw new ParserException("UnknownFunction");

            // Convert int array to object array
            var argObjs = argVals.Select(x => (object)x).ToArray();

            // Call the function
            return (int)mi.Invoke(_targetObject, argObjs);
        }
    }
}
