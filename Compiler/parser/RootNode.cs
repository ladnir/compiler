using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class RootNode : Node , LocalScope
    {
        Dictionary<string, FunctionNode> functions = new Dictionary<string, FunctionNode>();

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
            throw new NotImplementedException();
        }

        public VariableNode getVarRef(string token)
        {
            throw new NotImplementedException();
        }

        public void addToScope(DeclarationNode localVar)
        {
            throw new NotImplementedException();
        }

        public bool funcInScope(string token)
        {
            return functions.ContainsKey(token); 
        }

        public FunctionNode getFuncRef(string token)
        {
            return functions[token];
        }

        public void addToScope(FunctionNode func)
        {
            functions.Add(func.getName(), func);
        }
    }
}
