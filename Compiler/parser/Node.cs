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

        virtual public string outputIBTL(int tabCount)
        {
            throw new NotImplementedException();
        }
        virtual public void outputGForth(int tabCount, StringBuilder sb)
        {
            throw new NotImplementedException();
        }
        virtual public string outputC(int tabCount)
        {

            throw new NotImplementedException();
        }

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
