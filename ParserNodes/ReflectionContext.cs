using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace VHDLparser.ParserNodes
{
    public class ReflectionContext : IContext
    {
        public ReflectionContext()
        {
            _targetObject = new MyLibrary();
        }

        MyLibrary _targetObject;

        public int CallFunction(string name, int[] arguments)
        {
            // Find method
            var mi = _targetObject.GetType().GetMethod(name);
            if (mi == null)
                throw new ParserException("UnknownFunction");

            // Convert int array to object array
            var argObjs = arguments.Select(x => (object)x).ToArray();
   
            // Call the method
            return (int)mi.Invoke(_targetObject, argObjs);
        }
    }
}
