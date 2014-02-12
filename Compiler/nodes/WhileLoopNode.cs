using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    //TODO: change back to Node
    class WhileLoopNode : ExpressionNode , ILocalScopeNode
    {
        private ExpressionNode eval;
        private ILocalScopeNode scope;
        private Dictionary<string, VariableNode> localVars = new Dictionary<string, VariableNode>();
        Dictionary<string, IFunctionNode> functions = new Dictionary<string, IFunctionNode>();

        public WhileLoopNode(ExpressionNode eval, ILocalScopeNode scope)
        {
            // TODO: Complete member initialization
            this.eval = eval;
            this.scope = scope;
        }
        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            //if (Parser.debug) Console.Write(" while \n");
            
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

            sb.Append("[ while " + eval.outputIBTL(tabCount+1) +"\n");

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

        public override string getReturnType()
        {
            //TODO remove this
            throw new NotImplementedException();
        }
    }
}
