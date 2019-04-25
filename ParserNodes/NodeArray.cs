using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
    // NodeArray represents a literal Array in the expression
    class NodeArray : Node
    {
        public NodeArray(int Array)
        {
            _Array = Array;
        }

        int _Array;             // The Array

        public override int Eval(IContext ctx)
        {
            // Just return it.  Too easy.
            return _Array;
        }
    }
}
