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
        private string returnType;
        private LinkedList<ParamNode> parameters;

        public FunctionNode(Token nameToken) : base()
        {
            parameters = new LinkedList<ParamNode>();   
        }

        public void addParam(ParamNode param)
        {
            parameters.AddLast(param);
        }

        override public string output()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(returnType);
            sb.Append(" ");
            sb.Append(name);
            sb.Append("(");

            LinkedListNode<ParamNode> cur = parameters.First;
            for (int i = 0; i < parameters.Count; i++) 
            {
                if (i != 0) sb.Append(",");
                sb.Append(cur.Value.output());

                cur = cur.Next;
            }

            sb.Append("){ \n");

            LinkedListNode<Node> child = children.First;
            for (int i = 0; i < children.Count; i++)
            {
                sb.Append(child.Value.output());
            }

            sb.Append("}\n");

            return sb.ToString();
        }

        public override void addChild(Node child)
        {
            children.AddLast(child);
        }
    }
}
