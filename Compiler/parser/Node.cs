using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Node
    {
        protected LinkedList<Node> children;

        public Node()
        {
            children = new LinkedList<Node>();
        }

        virtual public string outputIBTL()
        {
            StringBuilder sb = new StringBuilder();

            LinkedListNode<Node> child = children.First;
            for (int i = 0; i < children.Count; i++)
            {
                sb.Append(child.Value.outputIBTL());
            }

            return sb.ToString();
        }

        virtual public void addChild(Node child)
        {
            children.AddLast(child);
        }


        virtual public void addChildren(LinkedList<Node> children)
        {
            this.children = children;
        }
      
    }
}
