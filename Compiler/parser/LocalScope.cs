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
        bool varInScope(Token name); 
        VariableNode getVarRef(Token token);
        void addToScope(DeclarationNode localVar);


        // functions
        bool funcInScope(Token token);
        FunctionNode getFuncRef(Token token);
        void AddToScope(FunctionNode func);



    }
}
