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


        public FunctionNode(Token returnType, Token functionName, LinkedList<Token> parameterNames, LinkedList<Token> parameterTypes)
        {
            
            this.parameters = new LinkedList<ParamNode>();

            this.returnType = returnType;
            this.functionName = functionName;

            // check param counts
            if (parameterNames.Count != parameterTypes.Count)
                throw new Exception("error f1, paramiter name count doesnt match paramter type count for function "+functionName.getValue() );

            // add the parameters to the function.
            LinkedList<Token>.Enumerator types = parameterTypes.GetEnumerator();
            foreach (Token name in parameterNames)
            {
                ParamNode param = new ParamNode(types.Current, name);
                types.MoveNext();

                parameters.AddLast(param);
                localVars.Add(name.getValue(), param);
            }
        }

        override public string outputIBTL()
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
                sb.Append(cur.Value.outputIBTL());

                cur = cur.Next;
            }

            sb.Append("){ \n");

            LinkedListNode<Node> child = children.First;
            for (int i = 0; i < children.Count; i++)
            {
                sb.Append(child.Value.outputIBTL());
            }

            sb.Append("}\n");

            return sb.ToString();
        }


        public bool varInScope(Token name)
        {
            throw new NotImplementedException();
        }

        public void addToScope(DeclarationNode localVar)
        {
            throw new NotImplementedException();
        }



        public bool funcInScope(CallNode function)
        {
            throw new NotImplementedException();
        }

        public bool funcInScope(FunctionNode fn)
        {
            throw new NotImplementedException();
        }


        public string getDataType(Token varName)
        {
            throw new NotImplementedException();
        }

        internal IEnumerable<ParamNode> getParameters()
        {
            throw new NotImplementedException();
        }
    }
}
