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
		public TestbenchGenerator (Parser ParsedInput, string Templates, string Testbench, string Interfaces, string configTB) 
		{
			Source = ParsedInput;
			TemplatePath = Templates;
			OutputPath = Testbench;
			InterfacePath = Interfaces;
			configPath = configTB;
			configuredInterfaces = new List<string> ();

			VerifyTemplates();
			configureTB(configPath);
			GenerateBif();
			GenerateTestbench();
		}

		Parser Source;
		string TemplatePath;
		string OutputPath;
		string InterfacePath;
		string configPath;
		string[] FileNames = {"_environment_pkg.sv", "_env_config_pkg.sv", "_test_pkg.sv", "_base_test_pkg.sv", "_type_pkg.sv", "_scoreboard_pkg.sv", "_ref_model_pkg.sv"};
		List<string> configuredInterfaces;
		void VerifyTemplates()
		{
			if (!File.Exists(TemplatePath + "template_bif.sv")) throw new ParserException ("Template for bif.sv missing");
		}

		void GenerateBif()
		{
			string line = "";
			StringBuilder sbText = new StringBuilder();
			using (var reader = new System.IO.StreamReader(TemplatePath + "template_bif.sv")) {
				while ((line = reader.ReadLine()) != null) 
				{
					if (line.Contains("InsertionPoint_Portmap")) 
					{
						PortMapSpecification (sbText);
					}
					else if (line.Contains("InsertionPoint_UnpackedDeclared"))
					{

						foreach (PortInterfaceElement element in Source.Portmap.Expressions)
						{
							if (element.isUnpacked){
								sbText.AppendLine("  packed_" + element.Type + " " + element.Name.Remove(element.Name.Length-2, 2) + "_p;");
							}
						}

						foreach (PortInterfaceElement element in Source.Portmap.Expressions)
						{
							if (element.isUnpacked){
								sbText.AppendLine("  " + element.Type + " " + element.Name.Remove(element.Name.Length-2, 2) + ";");
							}
						}

						
					}
					else if (line.Contains("Fetch_Interface:"))
					{
						line = reader.ReadLine();
						List<string> lines = fetchInterface(line);
						List<string> arrayLines = fetchInterface(line + "_array");
						List<string> generateLines = new List<string> (); 
						
						foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
						{
							if (!implementCongfiguration(interfaceInstance, line, sbText)) 
							{
								if (interfaceInstance.isArray && arrayLines != null)
									generateLines = arrayLines;
								else 
									generateLines = lines;
								foreach (string entry in generateLines)
								{
									string interfaceLine = entry.Replace("${if_name}", interfaceInstance.Name);
									interfaceLine = interfaceLine.Replace("${NAME}", Source.Entity);
									//Even thought below it states "$leftREQ" and "$rightREQ" what its really requesting is the data signal.
									interfaceLine = interfaceLine.Replace("$leftREQ", interfaceInstance.data.SignalType.getLeft().ToString());
									interfaceLine = interfaceLine.Replace("$rightREQ", interfaceInstance.data.SignalType.getRight().ToString());
									sbText.AppendLine(interfaceLine);
								}
							}
						}
					}
					else if (line.Contains("InsertionPoint_Metadata"))
					{
						foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
						{
							if (!implementCongfiguration(interfaceInstance, line, sbText)) 
							{	
								if (interfaceInstance.InOut == "in") {
									if (interfaceInstance.isArray){
										sbText.AppendLine("for (genvar i = 0; i < NUM_SBLKS; i++) begin");
										sbText.AppendLine("      " + interfaceInstance.Name + "_vif[i].metadata = '0;");
										sbText.AppendLine("end");
									}
									else
										sbText.AppendLine("        " + interfaceInstance.Name + "_vif.metadata = '0;");
								}
							}
						}
					}
					else if (line.Contains("InsertionPoint_StreamUnpacked"))
					{
						foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
						{
							if (!implementCongfiguration(interfaceInstance, line, sbText)) 
							{
								if (interfaceInstance.isArray)
									sbText.AppendLine("for (genvar i = 0; i < " + (interfaceInstance.data.SignalType.getRight()+1) + "; i++) begin");
								foreach (PortInterfaceElement element in interfaceInstance.Expressions) {
									if (element.isUnpacked){
											if (interfaceInstance.InOut == "in"){
												sbText.AppendLine("    assign " + element.Name.Remove(element.Name.Length-2, 2) + "_p = " + interfaceInstance.Name + "_vif.metadata;");
												sbText.AppendLine("    assign {<<{" + element.Name.Remove(element.Name.Length-2, 2) + "}} = " + element.Name.Remove(element.Name.Length-2, 2) + "_p;");
											}
											else if (interfaceInstance.InOut == "out"){
												sbText.AppendLine("    assign {>>{" + element.Name.Remove(element.Name.Length-2, 2) + "_p}} = " + element.Name.Remove(element.Name.Length-2, 2) + ";");
												sbText.AppendLine("    assign " + interfaceInstance.Name + "_vif.metadata = " + element.Name.Remove(element.Name.Length-2, 2) + "_p;");
											}
									}
								}
								if (interfaceInstance.isArray)
									sbText.AppendLine("end");
								
								sbText.AppendLine("");
							}
						}
						foreach (PortInterfaceElement element in Source.Portmap.NotInInterface){
							if (element.isUnpacked){
								if (element.InOut == "in"){
									sbText.AppendLine("    assign " + element.Name.Remove(element.Name.Length-2, 2) + "_p = '0;");
									sbText.AppendLine("    assign {<<{" + element.Name.Remove(element.Name.Length-2, 2) + "}} = " + element.Name.Remove(element.Name.Length-2, 2) + "_p;");
								}
								else if (element.InOut == "out"){
									sbText.AppendLine("    assign " + element.Name.Remove(element.Name.Length-2, 2) + "_p = '0;");
									sbText.AppendLine("    assign {>>{" + element.Name.Remove(element.Name.Length-2, 2) + "}} = " + element.Name.Remove(element.Name.Length-2, 2) + "_p;");
									
								}
								sbText.AppendLine("");
							}
						}
					}

					else if (line.Contains("InsertionPoint_NotTop"))
					{
						foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
						{
							if (!implementCongfiguration(interfaceInstance, line, sbText))
							{	
								sbText.AppendLine("    assign " + interfaceInstance.Name + "_vif.req = " + interfaceInstance.req.Name + ";");
								sbText.AppendLine("    assign " + interfaceInstance.Name + "_vif.data = " + interfaceInstance.data.Name + ";");
								sbText.AppendLine("    assign {>>{" + interfaceInstance.meta.Name.Remove(interfaceInstance.meta.Name.Length-2, 2) + "_p}} = " + interfaceInstance.meta.Name + ";");
								sbText.AppendLine("    assign " + interfaceInstance.Name + "_vif.metadata = " + interfaceInstance.meta.Name.Remove(interfaceInstance.meta.Name.Length-2, 2)+ "_p;");
								sbText.AppendLine("    assign " + interfaceInstance.Name + "_vif.ack = " + interfaceInstance.ack.Name + ";");
								sbText.AppendLine("");
							}
						}
					}
					else if (line.Contains("InsertionPoint_NotTOP"))
					{
						//Uncertain what needs to go here
					}
					else if (line.Contains("InsertionPoint_DUTConnection"))
					{
						sbText = InstantiateDUT (sbText);
					}
					else 
					{
						sbText.AppendLine(line.Replace("${NAME}", Source.Entity));
					}
				}
			}
			using(var writer = new System.IO.StreamWriter(OutputPath + Source.Entity +"_bif.sv")) {
    		writer.Write(sbText.ToString());
			}
		}

		void GenerateTestbench()
		{
			foreach (string file in FileNames){
				string line = "";
				StringBuilder sbText = new StringBuilder();	
				using (StreamReader reader = new System.IO.StreamReader(TemplatePath + "template" + file)) {
					while ((line = reader.ReadLine()) != null) 
					{
						if (line.Contains("Fetch_Interface:")) 
						{
							line = reader.ReadLine();
							List<string> lines = fetchInterface(line);
							int IterationCounter = 0;
							foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
							{
								if (!implementCongfiguration(interfaceInstance, line, sbText)) 
								{	
									foreach (string entry in lines)
									{	
										
										string interfaceLine = entry.Replace("${if_name}", interfaceInstance.Name);
										interfaceLine = interfaceLine.Replace("${NAME}", Source.Entity);
										interfaceLine = interfaceLine.Replace("${IterationCounter}", IterationCounter.ToString());
										//Even thought below it states "$leftREQ" and "$rightREQ" what its really requesting is the data signal.
										interfaceLine = interfaceLine.Replace("$leftREQ", interfaceInstance.data.SignalType.getLeft().ToString());
										interfaceLine = interfaceLine.Replace("$rightREQ", interfaceInstance.data.SignalType.getRight().ToString());
										if (interfaceLine[interfaceLine.Length-1] == ',' && interfaceInstance.Equals(Source.Portmap.InterfaceList.Last()))
											interfaceLine = interfaceLine.Remove(interfaceLine.Length-1, 1);
										//Only for _environement_pkg.sv
										if (interfaceInstance.InOut == "in" && !line.Contains("OnlyOutput")){
											interfaceLine = interfaceLine.Replace("${ProducerConsumer}", "producer");
											interfaceLine = interfaceLine.Replace("${ProdCons}", "prod");
											interfaceLine = interfaceLine.Replace("${InitiatorTarget}", "initiator");
											interfaceLine = interfaceLine.Replace("${MasterSlave}", "MASTER");
											sbText.AppendLine(interfaceLine);
										}	
										else if (interfaceInstance.InOut == "out" && !line.Contains("OnlyInput")){
											interfaceLine = interfaceLine.Replace("${ProducerConsumer}", "consumer");
											interfaceLine = interfaceLine.Replace("${ProdCons}", "cons");
											interfaceLine = interfaceLine.Replace("${InitiatorTarget}", "target");
											interfaceLine = interfaceLine.Replace("${MasterSlave}", "SLAVE");
											sbText.AppendLine(interfaceLine);
										}
									}
									IterationCounter++;
								}
							}
						}
						else if (line.Contains("Type_Package_Unpacked:"))
							DefineUnpackedTypes(sbText);
						else {
							sbText.AppendLine(line.Replace("${NAME}", Source.Entity));
						}
					}
				}
			
				using(var writer = new System.IO.StreamWriter(OutputPath + Source.Entity + file)) {
				writer.Write(sbText.ToString());
				}
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
						line = reader.ReadLine();
						while (!string.IsNullOrWhiteSpace(line))
						{
							lines.Add (line);
							line = reader.ReadLine();
						}
						
						return lines;
					}
					
						
				}
			}
			return null;
		}


		StringBuilder PortMapSpecification (StringBuilder sbText)
		{
			string lineBuilder = "";
			sbText.AppendLine("    " + Source.Portmap.Clock.InputOutput +  Source.Portmap.Clock.SignalType.PortmapDefinition() + Source.Portmap.Clock.Name + ",");
			sbText.AppendLine("    " + Source.Portmap.Reset.InputOutput +  Source.Portmap.Reset.SignalType.PortmapDefinition() + Source.Portmap.Reset.Name + ",");
			sbText.AppendLine("");

			foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
			{
				if (!implementCongfiguration(interfaceInstance, "InsertionPoint_Portmap", sbText))
				{
					foreach (PortInterfaceElement interfaceElement in interfaceInstance.Expressions)
					{
						if (interfaceInstance.Equals(Source.Portmap.InterfaceList.Last()) && interfaceElement.Equals(interfaceInstance.Expressions.Last()))
							sbText.AppendLine("    input  " + interfaceElement.SignalType.PortmapDefinition() + interfaceElement.Name);
						else
							sbText.AppendLine("    input  " + interfaceElement.SignalType.PortmapDefinition() + interfaceElement.Name + ",");
					}
					sbText.AppendLine("");
				}			
			}

			return sbText;

		}

		StringBuilder DefineUnpackedTypes (StringBuilder sbText)
		{
			string lineBuilder = "";
			List<string> alreadyDefined = new List<string> ();
			foreach (RecordTypeDeclaration RecordType in Source.Portmap.UnpackedList)
			{		
				var combined = RecordType.IdentifierList.Zip(RecordType.SubtypeList, (n, t) => new { Name = n, Type = t });
				foreach (var element in combined) 
				{
					if (!(Array.FindIndex(Source.Predefined, x => x.getIdentifier() == element.Type.getIdentifier()) > -1))
					{
					//	if (element.Type.getType() == "EnumerationType")
						//	sbText.AppendLine("typedef enum " + element.Type.PortmapDefinition() + element.Type.getIdentifier()+ ";");
						//else

						//This prevents already defined types from being defined again.
							if ((alreadyDefined.Find (x => x == element.Type.getIdentifier())) == null){
								sbText.AppendLine("typedef" + element.Type.PortmapDefinition() + element.Type.getIdentifier()+ ";");
								alreadyDefined.Add(element.Type.getIdentifier());
							}
					}
			
						
				}

				sbText.AppendLine("typedef struct packed {");
				
				foreach (var element in combined) 
				{
					if (Array.FindIndex(Source.Predefined, x => x.getIdentifier() == element.Type.getIdentifier()) > -1)
						sbText.AppendLine(element.Type.PortmapDefinition() + " " + element.Name + ";");
					else 
						sbText.AppendLine("  " +element.Type.getIdentifier() + " " + element.Name + ";");
				}
				sbText.AppendLine("} packed_" + RecordType.getIdentifier() + ";" );
			}
			


			return sbText;

		}

		StringBuilder InstantiateDUT (StringBuilder sbText)
		{
			sbText.AppendLine("      ." + Source.Portmap.Clock.Name + "  (local_clk),");
			sbText.AppendLine("      ." + Source.Portmap.Reset.Name + "  (local_rst_n),");
			sbText.AppendLine("");
			ExtractedInterface last = Source.Portmap.InterfaceList.Last();
			foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
			{
				if (!implementCongfiguration(interfaceInstance, "      InsertionPoint_DUTConnection", sbText))
				{
					sbText.AppendLine("      ." + interfaceInstance.data.Name + "  ( " + interfaceInstance.Name + "_vif.data ),");
					sbText.AppendLine("      ." + interfaceInstance.req.Name +  "  ( " + interfaceInstance.Name + "_vif.req ),");
					sbText.AppendLine("      ." + interfaceInstance.meta.Name + "  ( " + interfaceInstance.meta.Name.Remove(interfaceInstance.meta.Name.Length-2, 2) + " ),");
					//Necessary to check if notInInterface list is empty, if empty last interface element should have no comma, otherwise comma is necessary for proceeding not in interface element.
					if (interfaceInstance.Equals(last) && (!Source.Portmap.NotInInterface.Any()))
						sbText.AppendLine("      ." + interfaceInstance.ack.Name + "  ( " + interfaceInstance.Name + "_vif.ack )");
					else 
						sbText.AppendLine("      ." + interfaceInstance.ack.Name + "  ( " + interfaceInstance.Name + "_vif.ack ),");
					
					sbText.AppendLine("");
				}		
				
			}

			foreach (PortInterfaceElement UnknownSignal in Source.Portmap.NotInInterface)
			{
				string lineBuilder = "";
				lineBuilder += "      ." + UnknownSignal.Name + "        (";
				if (UnknownSignal.isUnpacked){
					lineBuilder += UnknownSignal.Name.Remove(UnknownSignal.Name.Length-2, 2);
				}
				if (UnknownSignal.Equals(Source.Portmap.NotInInterface.Last())){
					lineBuilder += ")";
					sbText.AppendLine(lineBuilder);
					sbText.AppendLine("");
				}
				else{
					lineBuilder += "),";
					sbText.AppendLine(lineBuilder);
				}
			}

			return sbText;

		}

		Boolean implementCongfiguration(ExtractedInterface interfaceInstance, string insertionPoint, StringBuilder sbText)
		{	
			Boolean configOccured = false;		
			string line = "";
			using (var reader = new System.IO.StreamReader(configPath)) {
				while ((line = reader.ReadLine()) != null) 
				{
					if (line == "begin " + interfaceInstance.Name)
					{ 
						
						while ((line = reader.ReadLine()) != "end " + interfaceInstance.Name) 
						{
							if (line == insertionPoint)
							{
								configOccured = true;
								line = reader.ReadLine();
								while (!string.IsNullOrWhiteSpace(line))
								{
									sbText.AppendLine(line);
									line = reader.ReadLine();
								}
							}
						}
					}	
				}
			}
			return configOccured;
		}


		
        void configureTB (string configPath)
		{
			string line = "";

			using (var reader = new System.IO.StreamReader(configPath)) {
				while ((line = reader.ReadLine()) != null)
				{
					string InterfaceName = "";
					string InterfaceType = "";
					string InterfaceDirection = "";
					List<string> SignalNames = new List<string> ();
					List<PortInterfaceElement> Signals = new List<PortInterfaceElement> ();
					if (line == "InterfaceConfiguration") 
					{
						while ((line = reader.ReadLine().Trim()) != ("end " + InterfaceName)) 
						{		
							if (line.Equals("InterfaceName:"))
							{
								line = reader.ReadLine().Trim();
								InterfaceName = line;
								configuredInterfaces.Add(InterfaceName);
							}
							if (line.Equals("InterfaceType:"))
							{

								line = reader.ReadLine().Trim();
								InterfaceType = line;
							}
							if (line.Equals("InterfaceDirection:"))
							{
								line = reader.ReadLine().Trim();
								InterfaceDirection = line;
							}
							if (line.Equals("Signals:"))
							{
								line = reader.ReadLine().Trim();
								while (!string.IsNullOrWhiteSpace(line))
								{
									SignalNames.Add (line);
									line = reader.ReadLine().Trim();
								}
							}
						}

						if (InterfaceName != null && InterfaceType != null && SignalNames != null)
						{
							//The actual signals are now extract from port map using the names specified in the config file.
							//The signals added must now also be removed from the not in interface list.
							foreach (PortInterfaceElement Element in Source.Portmap.Expressions)
							{
								if (SignalNames.Contains(Element.Name)){
									Signals.Add(Element);
									Source.Portmap.NotInInterface.Remove(Element);
								}
							}
						
							ExtractedInterface extracted = new ExtractedInterface (InterfaceName, InterfaceType, Signals, new PortInterfaceElement ("", InterfaceDirection, "", new nullType()), new PortInterfaceElement ("", InterfaceDirection, "", new nullType()), new PortInterfaceElement ("", InterfaceDirection, "", new nullType()), new PortInterfaceElement ("", InterfaceDirection, "", new nullType()), false);
							Source.Portmap.InterfaceList.Add(extracted);
						}
						else 
							throw new ParserException ("Not all parameters have been specified in the config.sv file for interface: " + InterfaceName);
					}	
				}
			}
		}

	}
}