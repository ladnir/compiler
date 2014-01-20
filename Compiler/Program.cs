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

            string source = "int x; 3 + 3; 3.6*3;";

            Console.WriteLine("input:" + source);

            Tokenizer t = new Tokenizer();

            Console.WriteLine("Tokenizing...");
            Token[] tokens = t.GetTokens(source);
            Console.WriteLine("Tokenized");
            foreach (Token tok in tokens) Console.WriteLine(tok.toString());



            Console.WriteLine("\n\nPress any key to close.");

            ConsoleKeyInfo key = Console.ReadKey();
        }

    }
}
