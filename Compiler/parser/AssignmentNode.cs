using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class AssignmentNode : Node 
    {
        private ExpressionNode expr;
        private VariableNode varNode;

        public AssignmentNode(VariableNode varNode, ExpressionNode expr)
        {
            // TODO: Complete member initialization
            this.varNode = varNode;
            this.expr = expr;
        }

        public override string outputIBTL()
        {
            return "[ := " + varNode.outputIBTL() + " " + expr.outputIBTL() + " ]";
        }
    }
}
