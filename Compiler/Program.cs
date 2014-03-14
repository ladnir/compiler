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

            Node root=null;
            string output = "";

            //try
            //{
            
                string path;
                for (int i = 0; i < args.Length || (i==0 && args.Length == 0); i++)
                {
                    if (args.Length == 0)
                        path = "../../tests/final/input01.ibtl";
                    else
                        path = args[i];

                    StreamReader stream = System.IO.File.OpenText(path);

                    Tokenizer t = new Tokenizer(stream);
                    Parser p = new Parser();

                    //doMileStone2(t);
                    if (true)
                    {
                        Console.WriteLine("Parsing...");
                        root = p.parseT(t);
                        Console.WriteLine("Parsing complete.");

                        StringBuilder sb = new StringBuilder();

                        Console.WriteLine("generating Gforth...");
                        root.outputGForth(1, sb);
                        output = sb.ToString();

                        //output = root.outputIBTL(0);
                        Console.WriteLine(output);
                    }
                    if (args.Length == 0)
                    {
                        using (StreamWriter outfile = new StreamWriter("../../out_"+i+".gf"))
                        {
                            outfile.Write(output);
                        }
                    }
                }

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("\n\n" + e.Message + "");
            //}



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
