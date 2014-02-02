using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class TokenWrapper
    {
        private Token[] tokens;

        public TokenWrapper(Token[] t)
        {
            tokens = t;
        }
        public int length
        {
            get { return tokens.Length; }
        }

        public Token this[int index]
        {
            get
            {
                if (index >= length) throw new EndOfTokensException();
               
                return tokens[index];
               
            }
        }
    }

    public class EndOfTokensException : Exception
    {
        public EndOfTokensException()
            : base("End of file reached without proper closing beace(s).")
        {

        }
    }
}
