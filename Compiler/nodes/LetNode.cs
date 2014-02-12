using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    //TODO: change back to node
    class LetNode : ExpressionNode
    {
        // TODO remove this
        public override string getReturnType()
        {
            throw new NotImplementedException();
        }
        public override void outputGForth(int tabCount, StringBuilder sb)
        {

            foreach (Node child in children)
            {
                child.outputGForth(tabCount, sb);
                sb.Append( "\n" + Node.getTabs(tabCount));
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


    }
}
