using System;
using System.IO;
using VHDLparser.ParserNodes;

namespace VHDLparser
{
    class Program
    {
        static void Main(string[] args)
        {
            string contents = null;

            //Obtaining paths from the user --------------------------------------------------
            Console.WriteLine("Please enter the folder path to the files you wish to parse:");
            var parserFiles = @"" + Console.ReadLine();
            Console.WriteLine("Please enter the folder path to the template files that are to be used for generation:");
            var TemplateIn = @"" + Console.ReadLine();
            Console.WriteLine("Please enter the folder path to the insertion points that are to be used for generation:");
            var InterfaceIn = @"" + Console.ReadLine();
            Console.WriteLine("If you wish to configure the testbench please enter the path to the configuration file:");
            var configTB = @"" + Console.ReadLine();
            Console.WriteLine("Please enter the path of the desired output: ");
            var TemplateOut = @"" + Console.ReadLine();
            //--------------------------------------------------------------------------------

            //Merging files to be parsed before initiating the parser.
            foreach (string file in Directory.EnumerateFiles(parserFiles, "*.vhd"))
            {
                contents += File.ReadAllText(file);
                Console.WriteLine("Added file: " + file);
                contents += " EndOfFileIdentifier ";
            }
            StringReader lSource = new StringReader(contents);
            Parser lParser = new Parser(lSource);
			ParserNode lClause = lParser.ParseNextNode();
			while (lClause != null)
			{
   				lClause = lParser.ParseNextNode();
			}
            Generator lGenerator = new Generator(lParser, TemplateIn, TemplateOut, InterfaceIn, configTB);


            Console.WriteLine("Testbench files have been successfully generated.");

		}
    }
}
