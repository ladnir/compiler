using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class StdoutNode : ExpressionNode, IFunctionNode
    {
        private ExpressionNode expr;

        public StdoutNode(ExpressionNode expr)
        {
            
            this.expr = expr;
        }

        public override string outputIBTL(int tabCount)
        {
            return "[ stdout " + expr.outputIBTL(tabCount) + "]";
        }

        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            expr.outputGForth(tabCount, sb);

            if (expr.getReturnType() == "int")
                sb.Append(" . ");
            else if (expr.getReturnType() == "float")
                sb.Append(" f. ");
            else if (expr.getReturnType() == "bool")
                sb.Append(" . ");
            else throw new NotImplementedException();
        }

        LinkedList<ParamNode> IFunctionNode.getParameters()
        {
            throw new NotImplementedException();
            //IntToken i = new IntToken("", -1, -1);
            //LinkedList<ParamNode> param = new LinkedList<ParamNode>();
            //ParamNode integer = new ParamNode(i, new ReferenceToken("", -1, -1));
            //param.AddFirst(integer);

            //return param;
        }

        public string getName()
        {
            throw new NotImplementedException();
            //return "stdout";
        }

        public override string getReturnType()
        {
            return "void";
        }
    }
}
