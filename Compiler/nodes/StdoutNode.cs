using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class StdoutNode :Node, IFunctionNode
    {
        private ExpressionNode expr;

        public StdoutNode(ExpressionNode expr)
        {
            
            this.expr = expr;
        }

        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            expr.outputGForth(tabCount, sb);

            if (expr.getReturnType() == "int")
                sb.Append(". ");
            else if (expr.getReturnType() == "float")
                sb.Append("f. ");
            else throw new NotImplementedException();
        }
       
        IEnumerable<ParamNode> IFunctionNode.getParameters()
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

        public string getReturnType()
        {
            throw new NotImplementedException();
            //return "void";
        }
    }
}
