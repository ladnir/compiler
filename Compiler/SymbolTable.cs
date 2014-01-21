using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class SymbolTable
    {
        private static SymbolTable instance;

        static String[] keyWords = {"for", "while","switch","case" , "if","else","int", 
                                 "boolean","real","string", "void", "true", "false"};
        static string[] dataTypes = { "int", "bool", "real", "string", "void" };
        static string[] constructs = { "for", "while", "if" };

        Dictionary<string, FunctionNode> FunctionTable = new Dictionary<string, FunctionNode>();

        public static SymbolTable Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SymbolTable();
                }
                return instance;
            }
        }

        public static bool isKeyWord(string word)
        {
            foreach (string s in keyWords)
            {
                if (s.Equals(word)) return true;
            }
            return false;
        }

        internal void addFunction(FunctionNode fn)
        {
            throw new NotImplementedException();
        }

        public bool funcInScope(CallNode function)
        {
            throw new NotImplementedException();
        }
    }
}
