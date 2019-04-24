using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
    // NodeNumber represents a literal number in the expression
    class NodeNumber : Node
    {
        public NodeNumber(int number)
        {
            _number = number;
        }

        int _number;             // The number

        public override int Eval()
        {
            // Just return it.  Too easy.
            return _number;
        }
    }
}
