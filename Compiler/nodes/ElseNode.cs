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
        Dictionary<string, UserFunctionNode> functions = new Dictionary<string, UserFunctionNode>();

        public ElseNode(ILocalScopeNode scope)
        {
            this.scope = scope;
        }

        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            if (Program.parserDebug) Console.Write("else\n");

            if (children.Count > 1) throw new NotImplementedException("multi statemnet else blocks are nto supported");

            sb.Append("\n" + Node.getTabs(tabCount) + "else\n" + Node.getTabs(tabCount+1));

            children.First.Value.outputGForth(tabCount+1,sb);
        }


        public override string outputIBTL(int tabCount)
        {
            StringBuilder sb = new StringBuilder();

            if (children.Count > 1)
            {
                sb.Append("[ // else \n");

                foreach (Node child in children)
                {
                    sb.Append(Node.getTabs(tabCount ) + child.outputIBTL(tabCount + 1) + "\n");
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


        public bool varInImmediateScope(string name)
        {
            if (localVars.ContainsKey(name)) return true;
            return false;
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
            if (functions.ContainsKey(token)) return true;
            return scope.funcInScope(token);
        }

        public UserFunctionNode getFuncRef(string token)
        {
            if (functions.ContainsKey(token)) return functions[token];
            return scope.getFuncRef(token);
        }

        public void addToScope(UserFunctionNode func)
        {
            functions.Add(func.functionName.toString(), func);
            //scope.addToScope(func);
        }


        public void defineFunc(string name)
        {
            scope.defineFunc(name);
        }

        public UserFunctionNode getParentFunc()
        {
            return scope.getParentFunc();
        }

        public override void toCircuit(List<Gate> gates, ref int nextWireID, StringBuilder dot)
        {
            throw new NotImplementedException();
        }

        public override string outputC(int tabCount)
        {
            throw new NotImplementedException();
        }
    }
}
