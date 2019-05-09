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


		}

		Parser Source;
		string TemplatePath;
		string OutputPath;

		void VerifyTemplates()
		{
			if (!File.Exists(TemplatePath + "template_bif.sv")) throw new ParserException ("Template for bif.sv missing");
		}

		void GenerateBif()
		{
			StringReader bifTemplate = new StringReader(TemplatePath + "template_bif.sv");
			Tokenizer bifTokenizer = new Tokenizer (bifTemplate);


		}

		void SkipOver (TokenType type, string value) 
		{
		
			while (!bifTokenizer.ReadNextToken ().Equals (type, value)) 
			{

			}
			bifTokenizer.ReadNextToken (); //Skip specified token
		}
	}
}


