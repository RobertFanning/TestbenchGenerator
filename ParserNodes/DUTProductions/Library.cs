using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
    public class MyLibrary
    {
        public MyLibrary()
        {
        }

        public int to_sfixed(int first, int second, int third)
        {
            return first;
        }

        public int to_unsigned(int first, int second)
        {
            return first;
        }

        public int std_ulogic_vector(int first)
        {
            return first;
        }

        public int to_ufixed(int first = 0, int second = 0, int third = 0)
        {
            return first;
        }
    }
}
