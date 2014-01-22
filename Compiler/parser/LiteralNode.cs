using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    public class LiteralNode : ExpressionNode
    {
        private Token nameToken;

        public LiteralNode(Token nameToken)
        {
            // TODO: Complete member initialization
            this.nameToken = nameToken;
        }
    }
}
