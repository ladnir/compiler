using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {

            string source = System.IO.File.ReadAllText(@"C:\Users\peter\Documents\Visual Studio 2013\Projects\Compiler\Compiler\input.ibtl");

            Console.WriteLine("input:" + source);

            Tokenizer t = new Tokenizer();
            Parser p = new Parser();

            Console.WriteLine("Tokenizing...");
            Token[] tokens = t.GetTokens(source);
            Console.WriteLine("Tokenized");
            
            try
            {
                Node root = p.parseTokens(tokens);

                string output = root.outputIBTL(1);

                Console.WriteLine(output);
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\n" + e.Message + "");
            }
            Console.WriteLine("\n\nPress any key to close.");

            ConsoleKeyInfo key = Console.ReadKey();
        }

    }
}
