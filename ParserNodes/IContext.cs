using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
    public interface IContext
    {
        int CallFunction(string name, int[] arguments);
    }
}
