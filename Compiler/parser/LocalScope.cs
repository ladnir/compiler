using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public interface LocalScope
    {
        // vars
        bool varInScope(string name);
        VariableNode getVarRef(string name);
        void addToScope(DeclarationNode localVar);


        // functions
        bool funcInScope(string token);
        FunctionNode getFuncRef(string name);
        void addToScope(FunctionNode func);



    }
}
