using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class CallNode : ExpressionNode
    {
        private Token refName;
        private LinkedList<Token> parameters;
        private FunctionNode func;
        private LinkedList<ExpressionNode> parameters1;

        public CallNode(Token refName)
        {
            // TODO: Complete member initialization
            parameters = new LinkedList<Token>();
            this.refName = refName;
        }

        public CallNode(FunctionNode func, LinkedList<ExpressionNode> parameters1)
        {
            // TODO: Complete member initialization
            this.func = func;
            this.parameters1 = parameters1;
        }

        internal void addParam(Token token)
        {
            parameters.AddLast(token);
        }
    }
}
