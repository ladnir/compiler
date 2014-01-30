using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class VariableNode : ExpressionNode
    {
        private Token nameToken;
        private DeclarationNode dec;

        public VariableNode(Token nameToken)
        {
            // TODO: Complete member initialization
            this.nameToken = nameToken;
        }

        public VariableNode(DeclarationNode dec)
        {
            // TODO: Complete member initialization
            this.dec = dec;
        }
    }
}
