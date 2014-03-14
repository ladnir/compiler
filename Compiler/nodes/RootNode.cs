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

        bool fPow = false;
        bool iPow = false;

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
            //initNot();          // not
            //initSin();          // sin
            //initCos();          // cos
            //initTan();          // tan
            //initStdout();         // stdout
        }

        //private void initStdout()
        //{
        //    builtInFunctions.add("stdout",(IFunctionNode) new StdoutNode() );
        //}

        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            if(iPow) sb.Append(": ^ {  input ex } 1 { sum } begin ex 0 > while sum input * TO sum ex 1 - TO ex repeat sum ; \n");
            if(fPow) sb.Append(": f^ { F: input  ex } 1.0e { F: sum } begin ex 0 > while sum input f* TO sum ex 1 - TO ex repeat sum ; \n\n");

            foreach (KeyValuePair<string, IFunctionNode> entry in functions)
            {
                IFunctionNode func = entry.Value;

                Node funcNode = (Node)func;
                funcNode.outputGForth(tabCount, sb);
                sb.Append("\n");
            }

            sb.Append(": main___ \n");
            foreach (Node n in children)
                n.outputGForth(tabCount, sb);

           // if(functions.ContainsKey("Main"))
            sb.Append("\n; \n\n cr  main___ cr");
        }

        public override string outputIBTL(int tabCount)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[\n");

            foreach (Node child in children)
            {
                sb.Append( child.outputIBTL(tabCount+1) + "\n");
            }

            sb.Append("]\n");

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

            return false;
        }

        public void addToScope(DeclarationNode dec)
        {
            if (varInScope(dec.getVarName()))
                throw new Exception("error adding declaration to root node at " + dec.gotToken().locate() + "\nvaiable is already in scope.");

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
            
            if (Program.parserDebug) Console.WriteLine("Root funcInScope " + token +" "+b);

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
            if (Program.parserDebug) Console.WriteLine("Root adding func " + ifunc.getName());
           
            functions.Add(ifunc.getName(), func);
        }
        public void defineFunc(string name)
        {
            if (name == "f^") fPow = true;
            else if (name == "^") iPow = true;

        }


        public UserFunctionNode getParentFunc()
        {
            throw new Exception("root error, return can only be used inside of a fucntion");
        }
    }
}
