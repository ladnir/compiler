using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Node
    {
        protected LinkedList<Node> children;

        public Node()
        {
            children = new LinkedList<Node>();
        }

        virtual public string print()
        {

            return null;
        }

        virtual public void addChild(Node child)
        {

        }
    }
}
