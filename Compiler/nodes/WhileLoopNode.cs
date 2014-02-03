using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    class WhileLoopNode : Node , ILocalScopeNode
    {
        private ExpressionNode eval;
        private ILocalScopeNode scope;
        private Dictionary<string, VariableNode> localVars = new Dictionary<string, VariableNode>();

        public WhileLoopNode(ExpressionNode eval, ILocalScopeNode scope)
        {
            // TODO: Complete member initialization
            this.eval = eval;
            this.scope = scope;
        }
        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            if (Parser.debug) Console.Write(" while \n");
            
            sb.Append("begin \n"+Node.getTabs(tabCount));
            eval.outputGForth(tabCount, sb);
            sb.Append(" while \n");

            foreach (Node n in children)
            {
                sb.Append(Node.getTabs(tabCount + 1));
                n.outputGForth(tabCount, sb);
                sb.Append("\n");
            }

            sb.Append(Node.getTabs(tabCount) + "repeat \n");
        }

        public override string outputIBTL(int tabCount)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[ while " + eval.outputIBTL(tabCount) +"\n");

            foreach (Node child in children)
            {
                sb.Append(Node.getTabs(tabCount) + child.outputIBTL(tabCount) + " \n");

            }
            sb.Append(Node.getTabs(tabCount-1)+"]\n");

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
            if (varInScope(dec.getVarName()))
                throw new Exception("error whn1 at " + dec.getVarName());

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
