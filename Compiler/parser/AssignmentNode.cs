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

        public override string outputIBTL(int tabCount)
        {
            string output = "[ := " + varNode.outputIBTL(tabCount) + " " + expr.outputIBTL(tabCount) + " ]";

            return output;
        }

        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            expr.outputGForth(tabCount, sb);
            sb.Append(varNode.getVarName());
        }
    }
}
