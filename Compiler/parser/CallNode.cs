using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class CallNode : ExpressionNode
    {
        private FunctionNode func;
        private LinkedList<ExpressionNode> parameters;


        public CallNode(FunctionNode func, LinkedList<ExpressionNode> parameters1)
        {
            // TODO: Complete member initialization
            this.func = func;
            this.parameters = parameters1;
        }


        public override string outputIBTL()
        {
            string output =  "[ " + func.outputIBTL() ;

            foreach (ExpressionNode expr in parameters)
            {
                output += " " + expr.outputIBTL();
            }

            output += " ]";

            return output;
        }
    }
}
