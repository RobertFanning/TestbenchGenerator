using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VHDLparser.ParserNodes;


namespace VHDLparser 
{
	public class Generator 
	{
		public Generator (Parser ParsedInput, string Templates, string Testbench, string Interfaces, string configTB) 
		{
			Source = ParsedInput;
			TemplatePath = Templates;
			OutputPath = Testbench;
			InterfacePath = Interfaces;
			configPath = configTB;
			configuredInterfaces = new List<ExtractedInterface> ();

			VerifyTemplates();
			if (configPath != "")
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
		List<ExtractedInterface> configuredInterfaces;
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
					else if (line.Contains("IncludePackages:"))
					{
						foreach (ExtractedInterface configuredInterface in configuredInterfaces)
						{
							implementCongfiguration(configuredInterface, line, sbText);
						}
					}
					else if (line.Contains("InsertionPoint_UnpackedDeclared:"))
					{

						foreach (PortInterfaceElement element in Source.Portmap.Expressions)
						{
							if (element.SignalType.isUnpacked() || (element.SignalType.getSignalType().isUnpacked())){
								sbText.AppendLine("  packed_" + element.Type + " " + element.Name.Remove(element.Name.Length-2, 2) + "_p;");
							}
						}

						foreach (PortInterfaceElement element in Source.Portmap.Expressions)
						{
							if (element.isUnpacked){
								sbText.AppendLine("  " + element.Type + " " + element.Name.Remove(element.Name.Length-2, 2) + ";");
							}
						}
						foreach (ExtractedInterface configuredInterface in configuredInterfaces)
						{
							implementCongfiguration(configuredInterface, line, sbText);
						}
					}
					else if (line.Contains("bif_configDB_interface:"))
					{
						foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
						{
							if (!implementCongfiguration(interfaceInstance, line, sbText)) 
							{	
								if (!interfaceInstance.isArray) {
										sbText.AppendLine("  uvm_config_db#(" + interfaceInstance.Name + "_" + interfaceInstance.Type + "_vif_t)::set(null, \"*\", \"" + Source.Entity + "_" + interfaceInstance.Name + "_vif\", " + interfaceInstance.Name + "_vif);");
								}
							}
						}
						sbText.AppendLine("    end");
						foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
						{
							if (!implementCongfiguration(interfaceInstance, line, sbText)) 
							{	
								if (interfaceInstance.isArray) {
									sbText.AppendLine("  generate for (genvar j=0; j<" + (interfaceInstance.data.SignalType.getRight()+1) + "; j++) begin");
    								sbText.AppendLine("    initial begin");
									sbText.AppendLine("      uvm_config_db#(" + interfaceInstance.Name + "_" + interfaceInstance.Type + "_vif_t)::set(null, \"*\", $sformatf(\"" + Source.Entity + "_" + interfaceInstance.Name + "_vif_%0d\",j), " + interfaceInstance.Name + "_vif[j]);");
									sbText.AppendLine("    end");
  									sbText.AppendLine("  end endgenerate");							
								}
							}
						}

					}
					else if (line.Contains("Fetch_Interface:"))
					{
						line = reader.ReadLine();
						List<string> lines = fetchInterface(line);
						List<string> arrayLines = fetchInterface(line.Remove(line.Length-1,1) + "_array:");
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
									interfaceLine = interfaceLine.Replace("${array_emptyBrackets}", interfaceInstance.isArray ? "[]" : "");
									interfaceLine = interfaceLine.Replace("${array_sizeBrackets}", interfaceInstance.isArray ? "[" + (interfaceInstance.data.SignalType.getRight()+1).ToString() + "]" : "");
									interfaceLine = interfaceLine.Replace("${array_filledBrackets}", interfaceInstance.isArray ? "[i]" : "");
									interfaceLine = interfaceLine.Replace("${sformatOpen}", interfaceInstance.isArray ? "$sformatf(" : "");
									interfaceLine = interfaceLine.Replace("${sformatClose}", interfaceInstance.isArray ? ",i)" : "");
									interfaceLine = interfaceLine.Replace("${forloop_begin}", interfaceInstance.isArray ? ("for (int i = 0; i < " + (interfaceInstance.data.SignalType.getRight()+1).ToString() + "; i++) begin") : "");
									interfaceLine = interfaceLine.Replace("${forloop_end}", interfaceInstance.isArray ? "end" : "");
									interfaceLine = interfaceLine.Replace("${%0d}", interfaceInstance.isArray ? "_%0d" : "");
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
										sbText.AppendLine("      for (genvar i = 0; i < " + (interfaceInstance.data.SignalType.getRight()+1) + "; i++) begin");
										sbText.AppendLine("      	" + interfaceInstance.Name + "_vif[i].metadata = '0;");
										sbText.AppendLine("      end");
									}
									else
										sbText.AppendLine("      " + interfaceInstance.Name + "_vif.metadata = '0;");
								}
							}
						}
					}
					else if (line.Contains("InsertionPoint_StreamUnpacked"))
					{
						foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
						{
							string arrayBuilder = "";
							if (!implementCongfiguration(interfaceInstance, line, sbText)) 
							{
								if (interfaceInstance.isArray)
								{
									arrayBuilder = "[i]";
									sbText.AppendLine("    for (genvar i = 0; i < " + (interfaceInstance.data.SignalType.getRight()+1) + "; i++) begin");
								}
								foreach (PortInterfaceElement element in interfaceInstance.Expressions) {
									if (element.SignalType.isUnpacked() || (element.SignalType.getSignalType().isUnpacked())){
										if (interfaceInstance.InOut == "in"){
											sbText.AppendLine("    assign " + element.Name.Remove(element.Name.Length-2, 2) + "_p" + arrayBuilder + " = " + interfaceInstance.Name + "_vif" + arrayBuilder + "." + element.Role + ";");
											sbText.AppendLine("    assign {<<{" + element.Name.Remove(element.Name.Length-2, 2) + arrayBuilder + "}} = " + element.Name.Remove(element.Name.Length-2, 2) + "_p" + arrayBuilder + ";");
										}
										else if (interfaceInstance.InOut == "out"){
											sbText.AppendLine("    assign {>>{" + element.Name.Remove(element.Name.Length-2, 2) + "_p" + arrayBuilder + "}} = " + element.Name.Remove(element.Name.Length-2, 2) + arrayBuilder + ";");
											sbText.AppendLine("    assign " + interfaceInstance.Name + "_vif" + arrayBuilder + "." + element.Role + " = " + element.Name.Remove(element.Name.Length-2, 2) + "_p" + arrayBuilder + ";");
										}
									} 
									else if (element.isArray){
										sbText.AppendLine("    assign " + interfaceInstance.Name + "_vif" + arrayBuilder + "." + element.Role + " = " + element.Name.Remove(element.Name.Length-2, 2) + arrayBuilder + ";");
									}
								}
								if (interfaceInstance.isArray)
								{
									sbText.AppendLine("    end");
								}
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
							string arrayBuilder = "";
							if (!implementCongfiguration(interfaceInstance, line, sbText))
							{	
								if (interfaceInstance.isArray)
								{
									arrayBuilder = "[i]";
									sbText.AppendLine("    for (genvar i = 0; i < " + (interfaceInstance.data.SignalType.getRight()+1) + "; i++) begin");
								}
								foreach (PortInterfaceElement element in interfaceInstance.Expressions) { 
									if (element.SignalType.isUnpacked() || (element.SignalType.getSignalType().isUnpacked())) {
										sbText.AppendLine("    assign {>>{" + element.Name.Remove(element.Name.Length-2, 2) + "_p" + arrayBuilder + "}} = " + element.Name + arrayBuilder + ";");
										sbText.AppendLine("    assign " + interfaceInstance.Name + "_vif" + arrayBuilder + "." + element.Role + " = " + element.Name.Remove(element.Name.Length-2, 2)+ "_p" + arrayBuilder + ";");
									}
									else
										sbText.AppendLine("    assign " + interfaceInstance.Name + "_vif" + arrayBuilder + "." + element.Role + " = " + element.Name + arrayBuilder + ";");
								}
								if (interfaceInstance.isArray)
									sbText.AppendLine("    end");

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
							List<string> arrayLines = fetchInterface(line.Remove(line.Length-1,1) + "_array:");
							List<string> generateLines = new List<string> ();
							int IterationCounter = 0;
							foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
							{
								if (!implementCongfiguration(interfaceInstance, line, sbText) && (lines != null || arrayLines != null)) 
								{	
									if (interfaceInstance.isArray && arrayLines != null)
										generateLines = arrayLines;
									else 
										generateLines = lines;

									foreach (string entry in generateLines)
									{	
										
										string interfaceLine = entry.Replace("${if_name}", interfaceInstance.Name);
										interfaceLine = interfaceLine.Replace("${NAME}", Source.Entity);
										interfaceLine = interfaceLine.Replace("${arraySize}", (interfaceInstance.data.SignalType.getRight()+1).ToString());
										interfaceLine = interfaceLine.Replace("${IterationCounter}", IterationCounter.ToString());
										//Even thought below it states "$leftREQ" and "$rightREQ" what its really requesting is the data signal.
										interfaceLine = interfaceLine.Replace("$leftREQ", interfaceInstance.data.SignalType.getLeft().ToString());
										interfaceLine = interfaceLine.Replace("$rightREQ", interfaceInstance.data.SignalType.getRight().ToString());
										interfaceLine = interfaceLine.Replace("${array_emptyBrackets}", interfaceInstance.isArray ? "[]" : "");
										interfaceLine = interfaceLine.Replace("${array_sizeBrackets}", interfaceInstance.isArray ? "[" + (interfaceInstance.data.SignalType.getRight()+1).ToString() + "]" : "");
										interfaceLine = interfaceLine.Replace("${array_filledBrackets}", interfaceInstance.isArray ? "[i]" : "");
										interfaceLine = interfaceLine.Replace("${sformatOpen}", interfaceInstance.isArray ? "$sformatf(" : "");
										interfaceLine = interfaceLine.Replace("${sformatClose}", interfaceInstance.isArray ? ",i)" : "");
										interfaceLine = interfaceLine.Replace("${forloop_begin}", interfaceInstance.isArray ? ("for (int i = 0; i < " + (interfaceInstance.data.SignalType.getRight()+1).ToString() + "; i++) begin") : "");
										interfaceLine = interfaceLine.Replace("${forloop_end}", interfaceInstance.isArray ? "end" : "");
										interfaceLine = interfaceLine.Replace("${%0d}", interfaceInstance.isArray ? "_%0d" : "");
										//Only for _environement_pkg.sv
										if (interfaceLine[interfaceLine.Length-1] == ',' && interfaceInstance.Equals(Source.Portmap.InterfaceList.Last()))
											interfaceLine = interfaceLine.Remove(interfaceLine.Length-1, 1);
										if (interfaceInstance.InOut == "in" && !line.Contains("OnlyOutput")){
											interfaceLine = interfaceLine.Replace("${ProducerConsumer}", "producer");
											interfaceLine = interfaceLine.Replace("${ProdCons}", "prod");
											interfaceLine = interfaceLine.Replace("${InitiatorTarget}", "initiator");
											interfaceLine = interfaceLine.Replace("${MasterSlave}", "MASTER");
											if(interfaceLine.Trim() != "")
												sbText.AppendLine(interfaceLine);
										}	
										else if (interfaceInstance.InOut == "out" && !line.Contains("OnlyInput")){
											interfaceLine = interfaceLine.Replace("${ProducerConsumer}", "consumer");
											interfaceLine = interfaceLine.Replace("${ProdCons}", "cons");
											interfaceLine = interfaceLine.Replace("${InitiatorTarget}", "target");
											interfaceLine = interfaceLine.Replace("${MasterSlave}", "SLAVE");
											if(interfaceLine.Trim() != "")
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
			List<string> alreadyDefined = new List<string> ();
			List<RecordTypeDeclaration> orderedList = Source.Portmap.UnpackedList;
			orderedList = orderDependencies(orderedList);
			foreach (RecordTypeDeclaration RecordType in orderedList)
			{		
				var combined = RecordType.IdentifierList.Zip(RecordType.SubtypeList, (n, t) => new { Name = n, Type = t });
				foreach (var element in combined) 
				{
					if (!(Array.FindIndex(Source.Predefined, x => x.getIdentifier() == element.Type.getIdentifier()) > -1))
					{
						//This prevents already defined types from being defined again.
							if ((alreadyDefined.Find (x => x == element.Type.getIdentifier())) == null){
								//This ensures we use the packed version already created.
								if (!element.Type.isUnpacked())
									sbText.AppendLine("typedef" + element.Type.PortmapDefinition() + element.Type.getIdentifier()+ "_def;"); //appending def to each typedef to prevent duplication between type package and vhdl package which leads to error. "mult_result_t cannot be resolved, as it is defined in package quantize_type_pkg and package quantization_test_p which are both wild card imported"
								alreadyDefined.Add(element.Type.getIdentifier());
							}
					}
			
						
				}

				sbText.AppendLine("typedef struct packed {");
				//Need to make sure the packed version of unpacked types are declared before they are used in the packed version of another unpacked type.
				foreach (var element in combined) 
				{
					if (Array.FindIndex(Source.Predefined, x => x.getIdentifier() == element.Type.getIdentifier()) > -1)
						sbText.AppendLine(element.Type.PortmapDefinition() + " " + element.Name + ";");
					else if (element.Type.isUnpacked())
						sbText.AppendLine("  packed_" +element.Type.getIdentifier() + " " + element.Name + ";");
					else
						sbText.AppendLine("  " +element.Type.getIdentifier() + "_def " + element.Name + ";");
				}
				sbText.AppendLine("} packed_" + RecordType.getIdentifier() + ";" );
			}
			foreach (PortInterfaceElement element in Source.Portmap.Expressions)
			{
				if (element.SignalType.getType() == "ArrayType" && element.SignalType.getSignalType().isUnpacked())
					sbText.AppendLine("  typedef  packed_" + element.SignalType.getSignalType().getIdentifier() + " [" + element.SignalType.getLeft() + ":" + element.SignalType.getRight() + "] "+  "packed_" + element.SignalType.getIdentifier() + ";");
			}

			


			return sbText;

		}

		List<RecordTypeDeclaration> orderDependencies (List<RecordTypeDeclaration> unorganisedList)
		{
			bool stillGoing = true;
			while (stillGoing)
			{
				stillGoing = false;
				for (int i = 0; i < unorganisedList.Count-1; i++)
				{
					RecordTypeDeclaration x = unorganisedList[i];
					RecordTypeDeclaration y = unorganisedList[i + 1];
					if (x.SubtypeList.Any (q => q.getIdentifier() == y.getIdentifier()))
					{
						unorganisedList[i] = y;
						unorganisedList[i + 1] = x;
						stillGoing = true;
					}
				}
			}
			return unorganisedList;
		}



		StringBuilder InstantiateDUT (StringBuilder sbText)
		{
			string lineBuilder = "";
			sbText.AppendLine("      ." + Source.Portmap.Clock.Name + "  (local_clk),");
			sbText.AppendLine("      ." + Source.Portmap.Reset.Name + "  (local_rst_n),");
			sbText.AppendLine("");
			//ExtractedInterface last = Source.Portmap.InterfaceList.Last();
			foreach (ExtractedInterface interfaceInstance in Source.Portmap.InterfaceList)
			{
				if (!implementCongfiguration(interfaceInstance, "      InsertionPoint_DUTConnection", sbText))
				{
					foreach (PortInterfaceElement element in interfaceInstance.Expressions) { 
						lineBuilder = "";
						if (element.isUnpacked)
							lineBuilder += ("      ." + element.Name + "  ( " + element.Name.Remove(element.Name.Length-2, 2) + " )");
						else
							lineBuilder += ("      ." + element.Name + "  ( " + interfaceInstance.Name + "_vif." + element.Role + " )");
						
						if (interfaceInstance.Equals(Source.Portmap.InterfaceList.Last()) && element.Equals(interfaceInstance.Expressions.Last()) && (!Source.Portmap.NotInInterface.Any()))
							sbText.AppendLine(lineBuilder);
						else
						 	sbText.AppendLine(lineBuilder + ",");
					}
					sbText.AppendLine("");
				}		
				
			}

			foreach (PortInterfaceElement UnknownSignal in Source.Portmap.NotInInterface)
			{
				lineBuilder = "";
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
			string if_name = interfaceInstance.Name.Trim();
			string ip = insertionPoint.Trim();
			if (configPath == "")
				return false;
			using (var reader = new System.IO.StreamReader(configPath)) {
				while ((line = reader.ReadLine()) != null) 
				{
					if (line == "begin " + if_name)
					{ 
						
						while ((line = reader.ReadLine()) != "end " + if_name) 
						{
							if (line == ip)
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
					if (line == "InterfaceConfiguration:") 
					{
						while ((line = reader.ReadLine().Trim()) != ("end " + InterfaceName)) 
						{		
							if (line.Equals("InterfaceName:"))
							{
								line = reader.ReadLine().Trim();
								InterfaceName = line;
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
						
							ExtractedInterface extracted = new ExtractedInterface (InterfaceName, InterfaceType, Signals, new PortInterfaceElement ("", InterfaceDirection, "", new nullType(), false), new PortInterfaceElement ("", InterfaceDirection, "", new nullType(), false), new PortInterfaceElement ("", InterfaceDirection, "", new nullType(), false), new PortInterfaceElement ("", InterfaceDirection, "", new nullType(), false), false);
							configuredInterfaces.Add(extracted);
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