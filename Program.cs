using System;
using System.IO;
using VHDLparser.ParserNodes;

namespace VHDLparser
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("The current time is " + DateTime.Now);
            string text = File.ReadAllText(@"C:\Users\rober\Documents\VHDL PARSER\VHDLparser\ExpressionTest.vhd");

            StringReader lSource = new StringReader(text);
	
			
            //PARSER
            Parser lParser = new Parser(lSource);
			ParserNode lClause = lParser.ParseNextNode();
			while (lClause != null)
			{
				//Console.WriteLine("The type of token is: " + lToken.Type + ", and it's value is: " + lToken.Value);
   				lClause = lParser.ParseNextNode();
                Console.WriteLine(lClause);
			}

            //lParser.ConstantList.ForEach(Console.WriteLine);

            Console.WriteLine("GAME OVER");
            //TOKENIZER 
            //Tokenizer lTokenizer = new Tokenizer(lSource);
			//Token lToken = lTokenizer.ReadNextToken();
			//while (lToken != null)
			//	{
			//		//ListBoxTokens.Items.Add(lToken.Type.ToString() + ":\t" + lToken.Value);
            //        Console.WriteLine("The type of token is: " + lToken.Type + ", and it's value is: " + lToken.Value);
            //        lToken = lTokenizer.ReadNextToken();
			//	}
			
		}
			
        
    }
}
