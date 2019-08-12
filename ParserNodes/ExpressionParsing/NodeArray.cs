using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
    // NodeArray represents a literal Array in the expression
    class NodeArray : Node
    {
        public NodeArray(Node[] arrayElements)
        {
            fArrayElements = arrayElements;
        }

        Node[] fArrayElements;             // The Array

        public override int Eval()
        {
    
            return fArrayElements.Length;
        }
    }
}
