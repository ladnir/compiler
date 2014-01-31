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

            string source;

            source =  " [                                           \n";
            source += "     [let [myFunction i b][int int string]   \n";
            source += "         [x int]                             \n";
            source += "         [:= x [+ 3  i]]                     \n";
            source += "         [if  [>= x 23] [                    \n";
            source += "             [stdout b]                      \n";
            source += "             [return 1.0]                    \n";
            source += "         ][//else                            \n";
            source += "             [stdout \"hi\" ] //what up      \n";
            source += "             [return 0]                      \n";
            source += "         ]                                   \n";
            source += "     ]                                       \n";
            source += " ]                                           \n";

            Console.WriteLine("input:" + source);

            Tokenizer t = new Tokenizer();
            Parser p = new Parser();

            Console.WriteLine("Tokenizing...");
            Token[] tokens = t.GetTokens(source);
            Console.WriteLine("Tokenized");
            foreach (Token tok in tokens) Console.WriteLine(tok.toString());

            Node root = p.parseTokens(tokens);

            string output = root.outputIBTL(0);

            Console.WriteLine(output);

            Console.WriteLine("\n\nPress any key to close.");

            ConsoleKeyInfo key = Console.ReadKey();
        }

    }
}
