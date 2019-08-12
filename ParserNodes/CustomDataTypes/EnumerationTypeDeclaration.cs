using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLparser.ParserNodes
{
	
	public class EnumerationTypeDeclaration : SignalType
	{
		public EnumerationTypeDeclaration(string identifier, List<string> enumerationList)
		{
			if (identifier == null) throw new ArgumentNullException("indentifier");


			fIdentifier = identifier;
			fEnumerationList = enumerationList;

		}

		readonly string fIdentifier;

		readonly List<string> fEnumerationList;
	
		public string Identifier { get { return fIdentifier; } }

		public int Left { get { return (fEnumerationList.Count-1); } }

		public int Right { get { return 0; } }

        public override string getIdentifier()
        {
            return fIdentifier;
        }

		public override string getType()
        {
            return "EnumerationType";
        }

		public override int getLeft()
        {
            return Left;
        }

		public override int getRight()
        {
            return Right;
        }

		public override Boolean isUnpacked()
        {
            return false;
        }

        public override SignalType getSignalType()
        {
            throw new ParserException ("Error: Attempting to obtain signal type from enumeration signal. Enumeration signals have no subtype.");
        }

		public override string PortmapDefinition()
        {
            string linebuilder ="";
            foreach (string enumer in fEnumerationList)
            {
                linebuilder += enumer + ", ";
            }            
            linebuilder = linebuilder.Remove(linebuilder.Length-2, 2);

            return ("  enum bit[31:0] {" + linebuilder + "} ");
        }

	}
}
