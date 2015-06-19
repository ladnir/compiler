using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public abstract class Node
    {
        protected LinkedList<Node> children;

        public Node()
        {
            
        }

        abstract public void toCircuit(List<Compiler.Gate> gates, ref int nextWireID, StringBuilder dotString);

        abstract public string outputIBTL(int tabCount);

        abstract public void outputGForth(int tabCount, StringBuilder sb);

        abstract public string outputC(int tabCount);

        virtual public void addChild(Node child)
        {
            if (children == null) children = new LinkedList<Node>();
            children.AddLast(child);
        }


        virtual public void addChildren(LinkedList<Node> children)
        {
            this.children = children;
        }


        protected static string getTabs(int tabCount)
        {
            string output = "";
            for (int i = 0; i < tabCount; i++)
            {
                output += "    ";
            }
            return output;
        }
    }
}
