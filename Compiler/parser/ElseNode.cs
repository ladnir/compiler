using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    class ElseNode : Node , LocalScope
    {
        private LocalScope scope;

        private Dictionary<string, VariableNode> localVars = new Dictionary<string, VariableNode>();

        public ElseNode(LocalScope scope)
        {
            this.scope = scope;
        }


        public override string outputIBTL(int tabCount)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[ \n");

            foreach (Node child in children)
            {
                sb.Append(Node.getTabs(tabCount) + child.outputIBTL(tabCount + 1) + "\n");
            }

            sb.Append(Node.getTabs(tabCount) + "] \n");

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

        public FunctionNode getFuncRef(string token)
        {
            return scope.getFuncRef(token);
        }

        public void addToScope(FunctionNode func)
        {
            scope.addToScope(func);
        }
    }
}
