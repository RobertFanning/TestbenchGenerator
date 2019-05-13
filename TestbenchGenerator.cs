using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VHDLparser.ParserNodes;

//SHOULD BE ABLE TO CALL TOKENIZER WITH NEW SOURCE, THEREFORE I WONT NEED A TOKENIZER FOR EACH TEMPLATE FILE
//MOVE THE HELPER FUNCTIONS FROM THE PARSER TO THE TOKENIZER SO ITS ACCESSIBLE IN THE CODE GENERATOR.
namespace VHDLparser 
{
	public class TestbenchGenerator 
	{
		public TestbenchGenerator (Parser ParsedInput, string Templates, string Testbench) 
		{
			Source = ParsedInput;
			TemplatePath = Templates;
			OutputPath = Testbench;

			VerifyTemplates();
			DuplicateTemplates();
			GenerateBif();

		}

		Parser Source;
		string TemplatePath;
		string OutputPath;

		void VerifyTemplates()
		{
			if (!File.Exists(TemplatePath + "template_bif.sv")) throw new ParserException ("Template for bif.sv missing");
		}

		void DuplicateTemplates()
		{
			foreach (string file in Directory.EnumerateFiles(TemplatePath, "*.sv"))
			{
   				File.Copy(file, OutputPath + Path.GetFileName(file), true);
			}
		}

		void GenerateBif()
		{
			string line = "";
			string lineBuilder = "";
			StringBuilder sbText = new StringBuilder();
			using (var reader = new System.IO.StreamReader(OutputPath + "template_bif.sv")) {
				while ((line = reader.ReadLine()) != null) 
				{
					if (line.Contains("InsertionPoint_Portmap")) 
					{
						//INSERT PORTMAP
						foreach (PortInterfaceElement element in Source.Portmap.Expressions)
						{
							switch (element.InOut){
								case "in": 
									lineBuilder += "input";
									break;
								case "out":
									lineBuilder += "output";
									break;
							}
							sbText.AppendLine(lineBuilder + " " + element.Name);
							lineBuilder = "";
						}
					}else 
					{
						sbText.AppendLine(line);
					}
				}
			}
			using(var writer = new System.IO.StreamWriter(OutputPath + "template_bif.sv")) {
    		writer.Write(sbText.ToString());
			}	

		}

	}
}


