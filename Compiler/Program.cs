using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {

                string source;

                //foreach (string s in args)
                //    Console.WriteLine(s);

                if (args.Length == 0)
                    source = System.IO.File.ReadAllText("../../input.ibtl");
                else
                    source = System.IO.File.ReadAllText(args[0]);

                Console.WriteLine("input:\n" + source);

                Tokenizer t = new Tokenizer(source);
                Parser p = new Parser();

                //doMileStone2(t);

                Console.WriteLine("Parsing...");
                Node root = p.parseT(t);

                Console.WriteLine("Parsing complete.");

                string output;// = root.outputIBTL(1);
                StringBuilder sb = new StringBuilder();

                Console.WriteLine("generating Gforth...");
                root.outputGForth(1, sb);
                Console.WriteLine("");

                output = sb.ToString();
                Console.WriteLine(output);

                using (StreamWriter outfile = new StreamWriter("../../out.gf"))
                {
                    outfile.Write(output);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\n" + e.Message + "");
            }
            Console.WriteLine("\n\nPress any key to close.");

            ConsoleKeyInfo key = Console.ReadKey();
        }

        private static void doMileStone2(Tokenizer t)
        {
            try
            {
                Console.WriteLine("Printing Tokens in order...\n");
                while (true)
                {
                    Token tok = t.pop();
                    Console.WriteLine(tok.getValue()+ " \t\t"+tok.getTokenType()+"\t"+tok.locate());
                }
            }
            catch (EndOfTokensException e)
            {
                //throw e;
            }
            Console.WriteLine("\n\nPrinting BaseTokens in symbol table...");
            t.printSymbolTable();
        }

    }
}
