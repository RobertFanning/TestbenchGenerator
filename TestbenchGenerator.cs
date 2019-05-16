using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VHDLparser.ParserNodes;

//SHOULD BE ABLE TO CALL TOKENIZER WITH NEW SOURCE, THEREFORE I WONT NEED A TOKENIZER FOR EACH TEMPLATE FILE
//MOVE THE HELPER FUNCTIONS FROM THE PARSER TO THE TOKENIZER SO ITS ACCESSIBLE IN THE CODE GENERATOR.


//"""" should have method that scans file at the end and replaces placeholders
namespace VHDLparser 
{
	public class TestbenchGenerator 
	{
		public TestbenchGenerator (Parser ParsedInput, string Templates, string Testbench, string Interfaces) 
		{
			Source = ParsedInput;
			TemplatePath = Templates;
			OutputPath = Testbench;
			InterfacePath = Interfaces;

			VerifyTemplates();
			DuplicateTemplates();
			GenerateBif();

		}

		Parser Source;
		string TemplatePath;
		string OutputPath;
		string InterfacePath;

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
							
							if (element.isUnpacked){
								sbText.AppendLine(lineBuilder + "  " + element.Type + " " + element.Name);
							}
							else {
								sbText.AppendLine(lineBuilder + "  logic " + element.Name);
							}
							lineBuilder = "";
						}
					}
					else if (line.Contains("InsertionPoint_UnpackedDeclared"))
					{
						foreach (PortInterfaceElement element in Source.Portmap.Expressions)
						{
							if (element.isUnpacked){
								sbText.AppendLine(element.Type + " " + element.Name);
							}
						}
					}
					else if (line.Contains("Fetch_Interface:"))
					{
						line = reader.ReadLine();
						line = fetchInterface(line);
						for (int interface_index = 0; interface_index < 2; interface_index++)
						{
						sbText.AppendLine(line);
						}
					}
					else {
						sbText.AppendLine(line);
					}
				}
			}
			using(var writer = new System.IO.StreamWriter(OutputPath + "template_bif.sv")) {
    		writer.Write(sbText.ToString());
			}	

		}

		string fetchInterface (string insertionPoint)
		{
			string line = "";
			using (var reader = new System.IO.StreamReader(InterfacePath + "handshake_if.sv")) {
				while ((line = reader.ReadLine()) != null) 
				{
					if (line == "bif_instantiation:")
					{
						line = reader.ReadLine();
						return line;
					}
						
				}
			}

			return null;
		}

	}
}


