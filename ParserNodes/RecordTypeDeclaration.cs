using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	//This is a composite type definition.
	public class RecordTypeDeclaration : SignalType
	{
		public RecordTypeDeclaration(string identifier, List<string> identifierList, List<string> subtypeIndication)
		{
			if (identifier == null) throw new ArgumentNullException("indentifier");

			fIdentifier = identifier;
		}

		readonly string fIdentifier;
	
		public string Identifier { get { return fIdentifier; } }

		public override string getType()
        {
            // Just return it.  Too easy.
            return "RecordType";
        }

		public override int getLeft()
        {
            // Just return it.  Too easy.
            return 0;
        }

		public override int getRight()
        {
            // Just return it.  Too easy.
            return 0;
        }

		public override Boolean isUnpacked()
        {
            // Just return it.  Too easy.
            return true;
        }

		public override string PortmapDefinition()
        {
            // Just return it.  Too easy.
            return ("  " + Identifier + "  ");
        }
	}
}
