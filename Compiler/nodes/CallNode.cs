using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class CallNode : ExpressionNode
    {
        private UserFunctionNode func;
        private List<ExpressionNode> parameters;


        public CallNode(UserFunctionNode func, List<ExpressionNode> parameters1)
        {
            // TODO: Complete member initialization
            this.func = func;
            this.parameters = parameters1;
        }

        public override List<Gate> NodeOutGates
        {
            get { return func.NodeOutGates; }
        }


        public override string outputIBTL(int tabCount)
        {
            

            string output =  "[ " + func.functionName ;

            foreach (ExpressionNode expr in parameters)
            {
                output += " " + expr.outputIBTL(tabCount);
            }

            output += " ]";

            return output;
        }
        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            //if (Parser.debug) Console.Write(" " + func.getName() + " ");



            foreach (Node n in parameters)
            {
                n.outputGForth(tabCount, sb);
            }
            sb.Append(" " + func.functionName + " ");
            
        }

        public override string getReturnType()
        {
            return func.getReturnType();
        }

        public override void toCircuit(List<Gate> gates, ref int nextWireID, StringBuilder dot)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                func.getParameters()[i].SetOutGates(parameters[i].NodeOutGates, ref nextWireID, gates);
            }

            func.toCircuit(gates,ref nextWireID, dot);
        }

        public override string outputC(int tabCount)
        {
            throw new NotImplementedException();
        }
    }
}
