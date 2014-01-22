using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class AssignmentNode : Node 
    {
        private Token varName;
        private ExpressionNode expr;

        public AssignmentNode(Token varName, ExpressionNode expr)
        {
            // TODO: Complete member initialization
            this.varName = varName;
            this.expr = expr;
        }
    }
}
