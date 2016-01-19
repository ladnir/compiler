
//#define USETRY

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Compiler
{
    class Program
    {

        public  const bool parserDebug = false;
        public  const bool cStyleScoping = true;

        static void Main(string[] args)
        {
            
            Node root=null;
            string output = "";

#if USETRY
            try
            {
#endif          
                string path;
                for (int i = 0; i < args.Length || (i==0 && args.Length == 0); i++)
                {
                    if (args.Length == 0)
                        path = "C:\\Users\\peter\\Source\\Repos\\compiler\\Compiler\\tests\\circuit\\circuit1.ibtl";
                    else
                        path = args[i];

                    StreamReader stream = System.IO.File.OpenText(path);

                    Tokenizer t = new Tokenizer(stream);
                    Parser p = new Parser();
                    root = p.parseT(t);
                    StringBuilder sb = new StringBuilder();
                    sb.Append("digraph{\n");

                    int wireIDStart = 0;
                    List<Gate> gates = new List<Gate>();
                    root.toCircuit(gates,ref wireIDStart, sb);

                    sb.Append("}\n");

                    SetInputVar("message", 12345, gates);


                    foreach (var gate in gates)
                        gate.Evalutate();

                    output = sb.ToString();

                    //output = root.outputIBTL(0);
                    //Console.WriteLine(output);


                    using (StreamWriter outfile = new StreamWriter("C:\\Users\\peter\\Source\\Repos\\compiler\\Compiler\\tests\\circuit\\circuit1.dot"))
                    {
                        outfile.Write(output);
                    }

                }
#if USETRY
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\n" + e.Message + "");
            }
#endif
        }

        private static void SetInputVar(string varName, int value,List<Gate> gates )
        {
            int mask = 1;
            int idx =0;

            while (gates[idx].mOutLabel != (varName + "_" + 0))
                idx++;

            int i = 0;
            while(value != 0)
            {
                gates[idx].Value = ((value & mask) == 1);
                
                if (gates[idx].mOutLabel != (varName + "_" + i))
                    throw new Exception();

                idx++;
                i++;
            }
        }

        public static string ShellExecute(string path, string command,  params string[] arguments)
        {
            using (var process = Process.Start(new ProcessStartInfo { WorkingDirectory = path, FileName = command, Arguments = string.Join(" ", arguments), UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true }))
            {
                using (process.StandardOutput)
                {
                    Console.WriteLine(process.StandardOutput.ReadToEnd());
                }
                using (process.StandardError)
                {
                    Console.WriteLine(process.StandardError.ReadToEnd());
                }
            }

            return path;
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
