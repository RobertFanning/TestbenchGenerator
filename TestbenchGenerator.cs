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
			GenerateEnvironment();

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
						List<string> lines = fetchInterface(line);
						Console.WriteLine("COUNT IS::::" + lines.Count);
						foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
						{
							foreach (string entry in lines)
							{
								string interfaceLine = entry.Replace("${NAME}", interfaceInstance.Name + "_if");
								sbText.AppendLine(interfaceLine);
							}
						}
					}
					else if (line.Contains("InsertionPoint_Metadata"))
					{
						foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
						{
							sbText.AppendLine(interfaceInstance.Name + "_if.metadata = '0;");
						}
					}
					else if (line.Contains("InsertionPoint_StreamUnpacked"))
					{
						foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
						{
							foreach (PortInterfaceElement element in interfaceInstance.Expressions) {
								if (element.isUnpacked){
									sbText.AppendLine("assign {<<{" + element.Name + "}} = " + interfaceInstance.Name + "_if.metadata;");
								}
							}
						}
					}
					else if (line.Contains("InsertionPoint_NotTOP"))
					{
						//Uncertain what needs to go here
					}
					else if (line.Contains("InsertionPoint_DUTConnection"))
					{
						sbText.AppendLine("." + Source.Portmap.Clock.Name + "(local_clk),");
						sbText.AppendLine("." + Source.Portmap.Reset.Name + "(local_rst_n),");
						ExtractedInterface last = Source.Portmap.InterfaceList.Last();
						foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
						{
							sbText.AppendLine("." + interfaceInstance.data.Name + "( " + interfaceInstance.Name + "_if.data ),");
							sbText.AppendLine("." + interfaceInstance.req.Name + "( " + interfaceInstance.Name + "_if.req ),");
							sbText.AppendLine("." + interfaceInstance.metadata.Name + "( " + interfaceInstance.metadata.Name + " ),");
						    if (interfaceInstance.Equals(last))
								sbText.AppendLine("." + interfaceInstance.ack.Name + "( " + interfaceInstance.Name + "_if.ack )");
							else
								sbText.AppendLine("." + interfaceInstance.ack.Name + "( " + interfaceInstance.Name + "_if.ack ),");
		
							
						}
					}
					else if (line.Contains("InsertionPoint_ConfigdbInterface"))
					{
						foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
						{
							sbText.AppendLine("uvm_config_db#(" + interfaceInstance.Name + "_" +interfaceInstance.Type + "_vif_t)::set(null, \"*\", \"${NAME}_" + interfaceInstance.Name + "_vif\", " + interfaceInstance.Name +"_if);");
						}
					}
					else 
					{
						sbText.AppendLine(line.Replace("${NAME}", Source.Entity));
					}
				}
			}
			using(var writer = new System.IO.StreamWriter(OutputPath + "template_bif.sv")) {
    		writer.Write(sbText.ToString());
			}	

		}

		void GenerateEnvironment()
		{
			string line = "";
			//string lineBuilder = "";
			StringBuilder sbText = new StringBuilder();
			using (var reader = new System.IO.StreamReader(OutputPath + "template_environment_pkg.sv")) {
				while ((line = reader.ReadLine()) != null) 
				{
					if (line.Contains("Fetch_Interface:")) 
					{
						line = reader.ReadLine();
						List<string> lines = fetchInterface(line);
						Console.WriteLine("COUNT IS::::" + lines.Count);
						foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
						{
							foreach (string entry in lines)
							{
								string interfaceLine = entry.Replace("${NAME}", interfaceInstance.Name + "_if");
								//if (interfaceInstance.InOut == "in")
							//		interfaceLine = interfaceLine.Replace("${refModel}", "producer");
							//	else if (interfaceInstance.InOut == "out")
							//		interfaceLine = interfaceLine.Replace("${refModel}", "consumer");
								sbText.AppendLine(interfaceLine);
							}
						}
					}
					else {
						sbText.AppendLine(line.Replace("${NAME}", Source.Entity));
					}
				}
			}
			using(var writer = new System.IO.StreamWriter(OutputPath + "template_environment_pkg.sv")) {
    		writer.Write(sbText.ToString());
			}	

		}


		List<string> fetchInterface (string insertionPoint)
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


