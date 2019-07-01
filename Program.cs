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
            foreach (string file in Directory.EnumerateFiles(@"C:\Users\rober\Documents\VHDLparser\ParserFiles\quantize\", "*.vhd"))
            {
                contents += File.ReadAllText(file);
                Console.WriteLine("Added file: " + file);
                contents += " EndOfFileIdentifier ";
            }

            string configTB = @"C:\Users\rober\Desktop\2706templates\Templates\Interfaces\config.sv";


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

            string TemplateIn = @"C:\Users\rober\Desktop\2706templates\Templates\tmpl_uvm_module_testbench\";
            string TemplateOut = @"C:\Users\rober\Desktop\predict_tb_generated\predict_tb_generated\OUTPUT\";
            string InterfaceIn = @"C:\Users\rober\Desktop\2706templates\Templates\Interfaces\";


            TestbenchGenerator lTestbenchGenerator = new TestbenchGenerator(lParser, TemplateIn, TemplateOut, InterfaceIn);


            Console.WriteLine("GAME OVER");

		}


        List<string> configTB (string configPath)
		{
			string line = "";
			List<string> lines = new List<string> ();

			using (var reader = new System.IO.StreamReader(InterfacePath + "handshake_if.sv")) {
				while ((line = reader.ReadLine()) != null) 
				{
					
					if (insertionPoint == line)
					{
						Console.WriteLine("SEARCHING FOR ITEM:::" + insertionPoint + "GOT IN WITH:" + line);
						line = reader.ReadLine();
						while (!string.IsNullOrWhiteSpace(line))
						{
							Console.WriteLine(line);
							lines.Add (line);
							line = reader.ReadLine();
						}
						
						return lines;
					}
					
						
				}
			}
			return null;
		}
    }
}
