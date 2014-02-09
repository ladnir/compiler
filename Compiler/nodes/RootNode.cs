using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class RootNode : Node , ILocalScopeNode
    {
        Dictionary<string, IFunctionNode> functions = new Dictionary<string, IFunctionNode>();
        Dictionary<string, IFunctionNode> builtInFunctions = new Dictionary<string, IFunctionNode>();
        Dictionary<string, VariableNode> localVars = new Dictionary<string, VariableNode>();

        public RootNode()
        {
            InitBuiltInFunctions();

        }

        private void InitBuiltInFunctions()
        {
            //initPlus();         // +
            //initMinus();        // -
            //initMult();         // *
            //initDivision();     // /
            //initMod();          // %
            //initPow();          // ^
            //initEquals();       // =
            //initGreater();      // >
            //initGrtEqu();       // >=
            //initLess();         // <
            //initLessEqu();      // <=
            //initNotEqu();       // !=
            //initOr();           // or
            //intiAnd();          // and
            //initNagative();     // ~     or later it will be -
            //initNot();          // not
            //initSin();          // sin
            //initCos();          // cos
            //initTan();          // tan
            //initStdout();         // stdout
        }

        //private void initStdout()
        //{
        //    builtInFunctions.Add("stdput", new StdoutNode());
        //}

        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            foreach (Node n in children)
                n.outputGForth(tabCount, sb);

            sb.Append("\n\n 1 Main");
        }

        public override string outputIBTL(int tabCount)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[\n");

            foreach (Node child in children)
            {
                sb.Append(Node.getTabs(tabCount) + child.outputIBTL(tabCount+1) + "\n\n");
            }

            sb.Append("]\n");

            return sb.ToString();
        }

        public bool varInScope(string name)
        {
            if (localVars.ContainsKey(name)) return true;

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
            
            return localVars[token];
            
        }

        public bool funcInScope(string token)
        {
            bool b =functions.ContainsKey(token); 
            
            if (Parser.debug) Console.WriteLine("Root funcInScope " + token +" "+b);

            if (!b) builtInFunctions.ContainsKey(token);

            return b;
        }

        public IFunctionNode getFuncRef(string token)
        {
            if(functions.ContainsKey(token))return functions[token];
            return builtInFunctions[token];
           
        }

        public void addToScope(UserFunctionNode func)
        {
            IFunctionNode ifunc = (IFunctionNode)func;
            if (Parser.debug) Console.WriteLine("Root adding func " + ifunc.getName());
           
            functions.Add(ifunc.getName(), func);
        }
    }
}
