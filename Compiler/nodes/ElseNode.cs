using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    class ElseNode : Node , ILocalScopeNode
    {
        private ILocalScopeNode scope;

        private Dictionary<string, VariableNode> localVars = new Dictionary<string, VariableNode>();

        public ElseNode(ILocalScopeNode scope)
        {
            this.scope = scope;
        }

        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            if (Parser.debug) Console.Write("else\n");

            if (children.Count > 1) throw new NotImplementedException("multi statemnet else blocks are nto supported");

            sb.Append("\n" + Node.getTabs(tabCount + 1));

            children.First.Value.outputGForth(tabCount,sb);
        }


        public override string outputIBTL(int tabCount)
        {
            StringBuilder sb = new StringBuilder();

            if (children.Count > 1)
            {
                sb.Append("[ // else \n");

                foreach (Node child in children)
                {
                    sb.Append(Node.getTabs(tabCount + 1) + child.outputIBTL(tabCount + 1) + "\n");
                }

                sb.Append(Node.getTabs(tabCount) + "] \n");
            }
            else
            {
                sb.Append("\n");

                sb.Append(Node.getTabs(tabCount) + children.First.Value.outputIBTL(tabCount + 1) + "\n");
            }
            return sb.ToString();
        }



        public bool varInScope(string name)
        {
            if (localVars.ContainsKey(name)) return true;
            if (scope.varInScope(name)) return true;

            return false; 
        }

        public void addToScope(DeclarationNode dec)
        {
            if(varInScope(dec.getVarName()))
                throw new Exception("error en1 at "+dec.getVarName() );

            VariableNode newVar = new VariableNode(dec);

            localVars.Add(dec.getVarName(), newVar);

        }

        public VariableNode getVarRef(string token)
        {
            if (localVars.ContainsKey(token))
                return localVars[token];
            else return scope.getVarRef(token);
        }

        public bool funcInScope(string token)
        {
            return scope.funcInScope(token);
        }

        public IFunctionNode getFuncRef(string token)
        {
            return scope.getFuncRef(token);
        }

        public void addToScope(UserFunctionNode func)
        {
            scope.addToScope(func);
        }
    }
}
