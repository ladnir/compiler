using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class SymbolTable
    {
        static String[] keyWords = {"for", "while","switch","case" , "int", 
                                 "boolean","real","string", "true", "false"};

        public static bool isKeyWord(string word)
        {
            foreach (string s in keyWords)
            {
                if (s.Equals(word)) return true;
            }
            return false;
        }
    }
}
