using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    class LetNode : Node
    {
      
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
