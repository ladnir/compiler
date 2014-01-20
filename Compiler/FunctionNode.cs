using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class FunctionNode : Node
    {
        private string name;


        public FunctionNode()
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
