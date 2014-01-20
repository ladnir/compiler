using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Parser
    {
        private enum State { Start}
        private Node root;
        private int index,length;
        private Token[] tokens;
        public Node parseTokens(Token[] tokens)
        {
            State state = State.Start;
            this.tokens = tokens;
            index = 0;
            length=tokens.Length;
            root = new Node();

            while (index < length)
            {
                Node function = getFunction();
                root.addChild(function);

            }

            return null;
        }

        private Node getFunction()
        {


            return null;
        }
    }
}
