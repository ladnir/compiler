using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    class IfNode : Node , ILocalScopeNode
    {

        private ExpressionNode eval;
        private ILocalScopeNode scope;
        private Dictionary<string, VariableNode> localVars = new Dictionary<string, VariableNode>();
        Dictionary<string, IFunctionNode> functions = new Dictionary<string, IFunctionNode>();

        private ElseNode elseNode;

        public IfNode(ExpressionNode eval, ILocalScopeNode scope)
        {
            // TODO: Complete member initialization
            this.eval = eval;
            this.scope = scope;
        }
  
        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            //if (Parser.debug) Console.Write("if \n");
            
            if (children.Count > 1) throw new NotImplementedException();

            eval.outputGForth(tabCount,sb);
            sb.Append(" if \n");
            sb.Append(Node.getTabs(tabCount + 1));
            children.First.Value.outputGForth(tabCount+1, sb);

            if (elseNode != null) 
                elseNode.outputGForth(tabCount, sb);

            sb.Append("\n" + Node.getTabs(tabCount) + "endif");
        }

        public override string outputIBTL(int tabCount)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[ if ");

            sb.Append(eval.outputIBTL(tabCount+1));

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
                throw new Exception("error adding declaration to if node at "+dec.gotToken().locate()+"\nvaiable is already in scope.");

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
            if (functions.ContainsKey(token)) return true;
            return scope.funcInScope(token);
        }

        public IFunctionNode getFuncRef(string token)
        {
            if (functions.ContainsKey(token)) return functions[token];
            return scope.getFuncRef(token);
        }

        public void addToScope(UserFunctionNode func)
        {
            IFunctionNode ifunc = (IFunctionNode)func;
            functions.Add(ifunc.getName(), ifunc);
            //scope.addToScope(func);
        }

        internal void addElse(ElseNode elseNode)
        {
            this.elseNode = elseNode;
        }

        public void defineFunc(string name)
        {
            scope.defineFunc(name);
        }
    }
}
