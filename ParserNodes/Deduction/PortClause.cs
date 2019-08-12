using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a for-loop.</summary>
	public class PortClause : Clause
	{
		public PortClause(List<PortInterfaceElement> portExpressions,List<RecordTypeDeclaration> Unpacked)
		{
			
			fExpressions = portExpressions;
			fUnpackedList = Unpacked; 
			fClock = new PortInterfaceElement(null, null, null, null, false);
			fReset = new PortInterfaceElement(null, null, null, null, false);
			fInterfaceList = new List<ExtractedInterface> ();
			UnknownSignals = new List<PortInterfaceElement> ();
		}

		PortInterfaceElement fClock;
		public PortInterfaceElement Clock { get { return fClock; } }
		PortInterfaceElement fReset;
		public PortInterfaceElement Reset { get { return fReset; } }

		readonly List<PortInterfaceElement> fExpressions;
		public List<PortInterfaceElement> Expressions { get { return fExpressions; } }

		List<PortInterfaceElement> UnknownSignals;
		public List<PortInterfaceElement> NotInInterface { get { return UnknownSignals; } }

		List<RecordTypeDeclaration> fUnpackedList;
		public List<RecordTypeDeclaration> UnpackedList { get { return fUnpackedList; } }

		List<ExtractedInterface> fInterfaceList;
		
		public List<ExtractedInterface> InterfaceList { get { return fInterfaceList; } }

		public void ExtractClockReset() {
			foreach (PortInterfaceElement element in fExpressions)
			{
				if (element.Name.Contains ("clock") || element.Name.Contains ("clk"))
					fClock = element;
				if (element.Name.Contains ("reset") || element.Name.Contains ("rst"))
					fReset = element;
			}
		}

		public void ExtractInterfaces () {
			List<PortInterfaceElement> AllSignals = new List<PortInterfaceElement> ();
			AllSignals = fExpressions.ToList();
			ExtractClockReset();
			
			string[] indentifiers = null;
			List<string> interfaceName = new List<string> ();
	
			foreach (PortInterfaceElement element in fExpressions)
			{
				indentifiers = element.Name.Split('_');
				interfaceName.Add (String.Concat(indentifiers[0] + '_' +indentifiers[1]));
			}
			//In .NET framework 3.5 and above you can use Enumerable.GroupBy which returns an enumerable of enumerables of duplicate keys, and then filter out any of the enumerables that have a Count of <=1, then select their keys to get back down to a single enumerable:
			var duplicateKeys = interfaceName.GroupBy(x => x)
                        .Where(group => group.Count() > 3)
                        .Select(group => group.Key);			

			//Check for Handshake interface
			foreach (string key in duplicateKeys) {
				List<PortInterfaceElement> groupedInterface = new List<PortInterfaceElement> ();
				PortInterfaceElement data = null;
				PortInterfaceElement ready = null;
				PortInterfaceElement metadata = null;
				PortInterfaceElement acknowledge = null;
				Boolean arrayInterface;
				foreach (PortInterfaceElement element in fExpressions){
					if (element.Name.Contains(key)) {
						if (element.Name.Contains("rdy")){
							ready = element;
							ready.setRole("req");
						}
						else if (element.Name.Contains("info")){
							metadata = element;
							metadata.setRole("metadata");
						}
						else if (element.Name.Contains("ack")){
							acknowledge = element;
							acknowledge.setRole("ack");
						}
						else{
							data = element;
							data.setRole("data");
						}
					}
				}
				if (data != null && ready != null && metadata != null && acknowledge != null){
					//Bundle all interface elements
					if (data.isArray && ready.isArray && metadata.isArray  && acknowledge.isArray)
						arrayInterface = true;
					else
						arrayInterface = false;
						
					groupedInterface.Add(data);
					groupedInterface.Add(ready);
					groupedInterface.Add(metadata);
					groupedInterface.Add(acknowledge);
					ExtractedInterface extractInter = new ExtractedInterface (key, "handshake", groupedInterface, data, ready, metadata, acknowledge, arrayInterface);
					fInterfaceList.Add (extractInter);
					//REMOVE ALL SIGNALS THAT ARE IN INTERFACES 
					AllSignals.RemoveAll(x => groupedInterface.Contains(x));
					
				}
			}

			AllSignals.Remove(fClock);
			AllSignals.Remove(fReset);
			UnknownSignals = AllSignals.ToList();


		}

	}
}
