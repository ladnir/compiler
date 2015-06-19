using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class SNode : Node , ILocalScopeNode
    {
        private bool braces = false;
        private ILocalScopeNode scope;
        private Dictionary<string, VariableNode> localVars = new Dictionary<string, VariableNode>();
        Dictionary<string, UserFunctionNode> functions = new Dictionary<string, UserFunctionNode>();

        public SNode(ILocalScopeNode scope)
        {
            this.scope = scope;
        }

        public void setBraces()
        {
            braces = true;
        }

        //public override void outputCircuit(int tabCount, StringBuilder sb, ref int nextWireID)
        //{
        //    if (children != null)
        //    {
        //        foreach (Node child in children)
        //        {
        //            sb.Append("\n" + Node.getTabs(tabCount));
        //            child.outputCircuit(tabCount + 1, sb,ref nextWireID);
        //        }
        //    }
        //}

        public override string outputIBTL(int tabCount)
        {
            string output="";
            int childtab = tabCount;
            if (braces)
            {
                childtab++;
                output += Node.getTabs(tabCount)+"[";
            }
            if (children == null)
            {
                output += "]\n";
                return output;
            }

            if (braces) output += "\n";
            foreach (Node child in children)
            {
                if (child is SNode)
                    output += child.outputIBTL(childtab);

                else output += Node.getTabs(childtab) + child.outputIBTL(childtab + 1) + "\n";
            }
            
            if (braces) output += Node.getTabs(tabCount ) + "]\n";

            return output;

        }
        public override void outputGForth(int tabCount, StringBuilder sb)
        {
           // if (Parser.debug) Console.WriteLine("entering S");
           // if(braces) sb.Append(Node.getTabs(tabCount)+"[");

            if (children != null)
            {
                sb.Append(" scope ");
                foreach (Node child in children)
                {
                    sb.Append("\n" + Node.getTabs(tabCount));
                    child.outputGForth(tabCount + 1, sb);
                }
                
                sb.Append("\n"+Node.getTabs(tabCount-1 ) + "endscope ");
            }
            //if (braces) sb.Append(Node.getTabs(tabCount) + "\n]");
        }

        internal bool hasBraces()
        {
            return braces;
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
            if (this.varInImmediateScope(dec.getVarName()) ||
                   (!Program.cStyleScoping &&
                     scope.varInScope(dec.getVarName())
                   )
               ) throw new Exception("error adding declaration to while node at " + dec.gotToken().locate() + "vaiable is already in scope.");

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
            foreach(var child in children)
            {
                child.toCircuit(gates,ref nextWireID, dot);
            }
        }

        public override string outputC(int tabCount)
        {
            throw new NotImplementedException();
        }
    }
}
