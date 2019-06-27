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
            int[] argVals = new int[_arguments.Length];
            for (int i=0; i<_arguments.Length; i++)
            {
                argVals[i] = _arguments[i].Eval();
            }

            //System.Reflection.MethodInfo mi = this.GetType().GetMethod(_functionName);
            //if (mi == null)
            //     throw new ParserException("UnknownFunction: " + _functionName);

            // Convert int array to object array
            //var argObjs = argVals.Select(x => (object)x).ToArray();

            // Call the function
            //Console.WriteLine(_functionName);
            if (_functionName == "to_sfixed")
                return to_sfixed(argVals);
            else if (_functionName == "to_unsigned")
                return to_unsigned(argVals);
            else if (_functionName == "std_ulogic_vector")
                return std_ulogic_vector(argVals);
            else if (_functionName == "to_ufixed")
                return to_ufixed(argVals);
            else
            {
                throw new ParserException("UnknownFunction: " + _functionName);
            }
        
            
        }

        public int to_sfixed(int[] arguments)
        {
            return arguments[0];
        }

        //Only takes 2 inputs
        public int to_unsigned(int[] arguments)
        {
            return arguments[0];
        }

        //Only takes 2 inputs
        public int std_ulogic_vector(int[] arguments)
        {
            return arguments[0];
        }


        //Only takes 2 inputs
        public int to_ufixed(int[] arguments)
        {
            return arguments[0];
        }
    }
}
