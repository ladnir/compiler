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

            string source = "";
            source += " int myFunction( int i, string b ){\n";
            source += "     int x;\n";
            source += "     x= 3 + i;\n";
            source += "     if(x> 23){\n";
            source += "         print(b);\n";
            source += "         return 1.0;\n";
            source += "     }else{\n";
            source += "         print(\"hi\");//what up\n";
            source += "         return 0;\n";
            source += "     }a\n";
            source += " }\n";

            Console.WriteLine("input:" + source);

            Tokenizer t = new Tokenizer();
            SymbolTable s = new SymbolTable();
            Parser p = new Parser();

            Console.WriteLine("Tokenizing...");
            Token[] tokens = t.GetTokens(source);
            Console.WriteLine("Tokenized");
            foreach (Token tok in tokens) Console.WriteLine(tok.toString());



            Console.WriteLine("\n\nPress any key to close.");

            ConsoleKeyInfo key = Console.ReadKey();
        }

    }
}
