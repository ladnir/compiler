using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
        public enum LetType { Local, Input, Output };
    class LetNode : Node
    {

        public override void outputGForth(int tabCount, StringBuilder sb)
        {

            foreach (Node child in children)
            {
                if (child is BlankNode)
                {
                }
                else
                { 
                    child.outputGForth(tabCount, sb);
                    sb.Append("\n" + Node.getTabs(tabCount-1));
                }
            }
        }

        public override string outputIBTL(int tabCount)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[ let [ ");

            foreach (Node child in children)
            {
                sb.Append(child.outputIBTL(tabCount));
            }

            sb.Append(" ] ]\n");

            return sb.ToString();
        }


        public override void toCircuit(List<Gate> gates, ref int nextWireID, StringBuilder dot)
        {

            foreach(var node in children)
            {
                node.toCircuit(gates, ref nextWireID, dot);
            }
            
        }

        public override string outputC(int tabCount)
        {
            throw new NotImplementedException();
        }
    }
}
