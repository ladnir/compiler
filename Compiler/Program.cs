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

            //try
            //{
                string source = System.IO.File.ReadAllText("../../input.ibtl");

                Console.WriteLine("input:" + source);

                Tokenizer t = new Tokenizer();
                Parser p = new Parser();

                Console.WriteLine("Tokenizing...");
                Token[] tokens = t.GetTokens(source);
                Console.WriteLine("Tokenized");
          
                Node root = p.parseTokens(tokens);

                string output;// = root.outputIBTL(1);
                StringBuilder sb = new StringBuilder();

                root.outputGForth(1, sb);
                output = sb.ToString();
                Console.WriteLine(output);

                using (StreamWriter outfile = new StreamWriter("../../out.gf"))
                {
                    outfile.Write(output);
                }
            //}
            //catch (Exception e)
            //{
                //Console.WriteLine("\n\n" + e.Message + "");
            //}
            Console.WriteLine("\n\nPress any key to close.");

            ConsoleKeyInfo key = Console.ReadKey();
        }

    }
}
