using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class SNode : Node
    {
        private bool braces = false;

        public void setBraces()
        {
            braces = true;
        }

        public override string outputIBTL(int tabCount)
        {
            string output="";
            int childtab = tabCount;
            if (braces)
            {
                childtab++;
                output += Node.getTabs(tabCount)+"[";
            }
            if (children == null)
            {
                output += "]\n";
                return output;
            }

            if (braces) output += "\n";
            foreach (Node child in children)
            {
                if (child is SNode)
                    output += child.outputIBTL(childtab);

                else output += Node.getTabs(childtab) + child.outputIBTL(childtab + 1) + "\n";
            }
            
            if (braces) output += Node.getTabs(tabCount ) + "]\n";

            return output;

        }
        public override void outputGForth(int tabCount, StringBuilder sb)
        {
           // if (Parser.debug) Console.WriteLine("entering S");
           // if(braces) sb.Append(Node.getTabs(tabCount)+"[");

            if (children != null)
            {
                foreach (Node child in children)
                {
                    sb.Append("\n" + Node.getTabs(tabCount));
                    child.outputGForth(tabCount + 1, sb);
                }
            }
            //if (braces) sb.Append(Node.getTabs(tabCount) + "\n]");
        }

        internal bool hasBraces()
        {
            return braces;
        }
    }
}
