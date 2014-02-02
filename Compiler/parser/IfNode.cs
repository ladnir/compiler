using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    class IfNode : Node , LocalScope
    {

        private ExpressionNode eval;
        private LocalScope scope;
        private Dictionary<string, VariableNode> localVars = new Dictionary<string, VariableNode>();

        private ElseNode elseNode;

        public IfNode(ExpressionNode eval, LocalScope scope)
        {
            // TODO: Complete member initialization
            this.eval = eval;
            this.scope = scope;
        }

        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            if (children.Count > 1) throw new NotImplementedException();


        }

        public override string outputIBTL(int tabCount)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[ if ");

            sb.Append(eval.outputIBTL(tabCount));

            if (children.Count > 1)
            {
                sb.Append(" [\n");
                foreach (Node child in children)
                {
                    sb.Append(Node.getTabs(tabCount + 1) + child.outputIBTL(tabCount + 2) + "\n");

                }
                sb.Append(Node.getTabs(tabCount) + "]");
            }
            else
            {

                sb.Append("\n");
                sb.Append(Node.getTabs(tabCount ) + children.First.Value.outputIBTL(tabCount + 1) + "\n");
            }

            if (elseNode != null) sb.Append(elseNode.outputIBTL(tabCount));
            else sb.Append("\n");

            sb.Append(Node.getTabs(tabCount-1) + "]");
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
                throw new Exception("error ifn1 at " + dec.getVarName());

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

        internal void addElse(ElseNode elseNode)
        {
            this.elseNode = elseNode;
        }
    }
}
