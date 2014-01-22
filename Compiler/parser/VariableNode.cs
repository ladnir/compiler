using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    public class VariableNode : ExpressionNode
    {
        private Token nameToken;

        public VariableNode(Token nameToken)
        {
            // TODO: Complete member initialization
            this.nameToken = nameToken;
        }
    }
}
