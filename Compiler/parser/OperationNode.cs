using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    public class OperationNode : ExpressionNode
    {
        private Token token;
        private ExpressionNode leftExpr;
        private ExpressionNode rightExpr;

        public OperationNode(Token token, ExpressionNode leftExpr, ExpressionNode rightExpr)
        {
            // TODO: Complete member initialization
            this.token = token;
            this.leftExpr = leftExpr;
            this.rightExpr = rightExpr;
        }
    }
}
