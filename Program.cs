using System;
using System.IO;
using VHDLparser.ParserNodes;

namespace VHDLparser
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("The current time is " + DateTime.Now);
            string contents = null;
            foreach (string file in Directory.EnumerateFiles(@"C:\Users\rtfa\Documents\WritingAParserBlog\VHDLparser\ParserFiles\", "*.vhd"))
            {
                contents += File.ReadAllText(file);
                contents += " EndOfFileIdentifier ";
            }



            StringReader lSource = new StringReader(contents);
				
            //PARSER
            Parser lParser = new Parser(lSource);
			ParserNode lClause = lParser.ParseNextNode();
			while (lClause != null)
			{
				
   				lClause = lParser.ParseNextNode();
                Console.WriteLine(lClause);
                Console.WriteLine("Jockee");

			}
            
            
            Console.WriteLine("Accessing list item: " + lParser.Portmap.Expressions[0].Name);

            string TemplateIn = @"C:\Users\rtfa\Documents\Templates\tmpl_uvm_module_testbench\";
            string TemplateOut = @"C:\Users\rtfa\Documents\Templates\DELETEme\";
            string InterfaceIn = @"C:\Users\rtfa\Documents\Templates\Interfaces\";


            TestbenchGenerator lTestbenchGenerator = new TestbenchGenerator(lParser, TemplateIn, TemplateOut, InterfaceIn);


            Console.WriteLine("GAME OVER");

		}
    }
}
