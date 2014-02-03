using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class CallNode : ExpressionNode
    {
        private IFunctionNode func;
        private LinkedList<ExpressionNode> parameters;


        public CallNode(IFunctionNode func, LinkedList<ExpressionNode> parameters1)
        {
            // TODO: Complete member initialization
            this.func = func;
            this.parameters = parameters1;
        }


        public override string outputIBTL(int tabCount)
        {
            

            string output =  "[ " + func.getName() ;

            foreach (ExpressionNode expr in parameters)
            {
                output += " " + expr.outputIBTL(tabCount);
            }

            output += " ]";

            return output;
        }
        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            if (Parser.debug) Console.Write(" " + func.getName() + " ");

            if (func.getName() == "stdout")
            {
                ((StdoutNode)(IFunctionNode)func).outputGForth(tabCount, sb);
                
            }else{

                foreach (Node n in parameters)
                {
                    n.outputGForth(tabCount, sb);
                }
                sb.Append(" "+func.getName()+" ");
            }
        }

        public override string getReturnType()
        {
            return func.getReturnType();
        }
    }
}
