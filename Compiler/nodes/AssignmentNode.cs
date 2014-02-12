using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class AssignmentNode : ExpressionNode 
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
            //if (Parser.debug) Console.Write(" { " + varNode.getVarName() + " } \n");

            expr.outputGForth(tabCount, sb);
            sb.Append(" TO "+varNode.getVarName());
        }

        public override string getReturnType()
        {
            throw new NotImplementedException();
        }
    }
}
