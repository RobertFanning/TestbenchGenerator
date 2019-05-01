using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
    // Node - abstract class representing one node in the expression 
    public abstract class Node
    {
        public abstract int Eval();
    }
}
