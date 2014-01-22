using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class FunctionNode : Node , LocalScope
    {
        private LinkedList<ParamNode> parameters;

        private Token returnType;
        private Token functionName;
        private Dictionary<string, Node> localVars = new Dictionary<string, Node>();

        public FunctionNode(Token returnType, Token functionName)
        {
            // TODO: Complete member initialization
            this.returnType = returnType;
            this.functionName = functionName;


        }

        public void addParam(ParamNode param)
        {
            localVars.Add(param.label(), param);
            parameters.AddLast(param);

        }

        override public string output()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(returnType.getValue() );
            sb.Append(" ");
            sb.Append(functionName.getValue() );
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


        internal void addChildren(LinkedList<Node> children)
        {
            this.children = children;
        }

        public bool inScope(Token name)
        {
            throw new NotImplementedException();
        }

        public void addToScope(DeclarationNode localVar)
        {
            throw new NotImplementedException();
        }
    }
}
