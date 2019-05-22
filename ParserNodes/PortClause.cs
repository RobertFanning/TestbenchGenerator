using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VHDLparser.ParserNodes
{
	/// <summary>Represents a for-loop.</summary>
	public class PortClause : Clause
	{
		public PortClause(List<PortInterfaceElement> portExpressions)
		{
			
			fExpressions = portExpressions;
			fClock = new PortInterfaceElement(null, null, null, false);
			fReset = new PortInterfaceElement(null, null, null, false);
			fInterfaceList = new List<ExtractedInterface> ();
		}

		PortInterfaceElement fClock;
		public PortInterfaceElement Clock { get { return fClock; } }
		PortInterfaceElement fReset;
		public PortInterfaceElement Reset { get { return fReset; } }

		readonly List<PortInterfaceElement> fExpressions;
		public List<PortInterfaceElement> Expressions { get { return fExpressions; } }

		List<ExtractedInterface> fInterfaceList;
		public List<ExtractedInterface> InterfaceList { get { return fInterfaceList; } }

		public void ExtractClockReset() {
			foreach (PortInterfaceElement element in fExpressions){
				if (element.Name.Contains ("clock") || element.Name.Contains ("clk"))
					fClock = element;
				if (element.Name.Contains ("reset") || element.Name.Contains ("rst"))
					fReset = element;
			}

			Console.WriteLine ("CLOCKCLOCKCLOCKCLOCK" + fClock.Name);
			Console.WriteLine ("RESETRESETRESETRESET" + fReset.Name);
		}

		public void ExtractInterfaces () {
			string[] indentifiers = null;
			List<string> interfaceName = new List<string> ();
	
			foreach (PortInterfaceElement element in fExpressions){
				indentifiers = element.Name.Split('_');
				Console.WriteLine(indentifiers[0]+indentifiers[1]);

				interfaceName.Add (String.Concat(indentifiers[0] + '_' +indentifiers[1]));
			}
			//In .NET framework 3.5 and above you can use Enumerable.GroupBy which returns an enumerable of enumerables of duplicate keys, and then filter out any of the enumerables that have a Count of <=1, then select their keys to get back down to a single enumerable:
			var duplicateKeys = interfaceName.GroupBy(x => x)
                        .Where(group => group.Count() > 3)
                        .Select(group => group.Key);			

			//Check for Handshake interface
			foreach (string key in duplicateKeys) {
				List<PortInterfaceElement> std_ulogic = new List<PortInterfaceElement> ();
				PortInterfaceElement audio_rxtx_t = null;
				PortInterfaceElement intea_info_t = null;
				foreach (PortInterfaceElement element in fExpressions){
					if (element.Name.Contains(key)) {
						if (element.Type == "audio_rxtx_t")
							audio_rxtx_t = element;
						if (element.Type == "std_ulogic")
							std_ulogic.Add (element);
						if (element.Type == "intea_info_t")
							intea_info_t = element;
					}
				}
				if (intea_info_t != null && audio_rxtx_t != null && std_ulogic.Count == 2){
					//Bundle all interface elements
					std_ulogic.Add (audio_rxtx_t);
					std_ulogic.Add (intea_info_t);
					ExtractedInterface extractInter = new ExtractedInterface (key, "handshake", std_ulogic);
					extractInter.ExtractSignals();
					fInterfaceList.Add (extractInter);
				}
			}
		}

	}
}
