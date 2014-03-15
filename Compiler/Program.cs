
#define USETRY

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
                        path = "../../tests/final/input10.ibtl";
                    else
                        path = args[i];

                    StreamReader stream = System.IO.File.OpenText(path);

                    Tokenizer t = new Tokenizer(stream);
                    Parser p = new Parser();

                    //doMileStone2(t);
                    
                    //Console.WriteLine("Parsing...");
                    root = p.parseT(t);
                    //Console.WriteLine("Parsing complete.");

                    StringBuilder sb = new StringBuilder();

                    //Console.WriteLine("generating Gforth...");
                    root.outputGForth(1, sb);
                    output = sb.ToString();

                    //output = root.outputIBTL(0);
                    Console.WriteLine(output);
                    
                    
                    using (StreamWriter outfile = new StreamWriter("../../out_"+i+".gf"))
                    {
                        outfile.Write(output);
#if !__MonoCS__
                        try
                        { // 
                            Process gforth = new Process();
                            gforth.StartInfo.FileName = @"C:\Program Files (x86)\gforth\gforth.exe";
                            gforth.StartInfo.Arguments = "../../out_" + i + ".gf";
                            gforth.Start();

                        }
                        catch (Exception e) { }
#endif
                    }

                }
#if USETRY
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\n" + e.Message + "");
            }
#endif


                //Console.WriteLine("\n\nPress any key to close.");

            ConsoleKeyInfo key = Console.ReadKey();
            
            foreach (Process proc in Process.GetProcessesByName("gforth"))
            {
                try
                {
                    proc.Kill();
                }
                catch (Exception e) { }
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
