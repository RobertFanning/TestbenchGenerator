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

        public int r { get; set; }

        public int rectArea(int width, int height)
        {
            return width * height;
        }

        public int rectPerimeter(int width, int height)
        {
            return (width + height) * 2;
        }

        public int to_sfixed(int first, int second, int third)
        {
            return first;
        }

        //Only takes 2 inputs
        public int to_unsigned(int first, int second)
        {
            return first;
        }
        
    }
}
