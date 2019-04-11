using System;
using System.IO;
using myApp.ParserNodes;

namespace myApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("The current time is " + DateTime.Now);
            string text = File.ReadAllText(@"C:\Users\rtfa\Documents\WritingAParserBlog\myApp\typicalPortmap.vhd");

            StringReader lSource = new StringReader(text);
	
			Tokenizer lTokenizer = new Tokenizer(lSource);

			Token lToken = lTokenizer.ReadNextToken();
            //Console.WriteLine(lToken.Value);
			while (lToken != null)
				{
					//ListBoxTokens.Items.Add(lToken.Type.ToString() + ":\t" + lToken.Value);
                    Console.WriteLine("The type of token is: " + lToken.Type + ", and it's value is: " + lToken.Value);
                    lToken = lTokenizer.ReadNextToken();
				}
			
		}
			
        
    }
}
