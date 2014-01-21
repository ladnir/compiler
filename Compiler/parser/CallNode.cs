using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class CallNode : ExpressionNode
    {
        private Token refName;
        private LinkedList<Token> parameters;

        public CallNode(Token refName)
        {
            // TODO: Complete member initialization
            parameters = new LinkedList<Token>();
            this.refName = refName;
        }

        internal void addParam(Token token)
        {
            parameters.AddLast(token);
        }
    }
}
