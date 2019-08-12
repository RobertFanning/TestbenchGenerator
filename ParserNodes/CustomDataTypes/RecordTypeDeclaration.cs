using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	//This is a composite type definition.
	public class RecordTypeDeclaration : SignalType
	{
		public RecordTypeDeclaration(string identifier, List<string> identifierList, List<SignalType> subtypeIndication)
		{
			if (identifier == null) throw new ArgumentNullException("indentifier");

			fIdentifier = identifier;
            fIdentifierList = identifierList;
            fSubtypeIndication = subtypeIndication;
		}

		readonly string fIdentifier;
	
		public string Identifier { get { return fIdentifier; } }

       List<string> fIdentifierList;
		public List<string> IdentifierList { get { return fIdentifierList; } }

        List<SignalType> fSubtypeIndication;
		public List<SignalType> SubtypeList { get { return fSubtypeIndication; } }

        public override string getIdentifier()
        {
            // Just return it.  Too easy.
            return fIdentifier;
        }

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

        public override SignalType getSignalType()
        {
            // Just return it.  Too easy.
            throw new ParserException ("Error: Attempting to obtain signal type from enumeration signal. Enumeration signals have no subtype.");
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
