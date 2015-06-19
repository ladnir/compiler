using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public interface ILocalScopeNode
    {
        // vars
        bool varInScope(string name);
        bool varInImmediateScope(string name);
        VariableNode getVarRef(string name);
        void addToScope(DeclarationNode localVar);


        // functions
        bool funcInScope(string token);
        UserFunctionNode getFuncRef(string name);
        void addToScope(UserFunctionNode func);
        void defineFunc(string name);



        UserFunctionNode getParentFunc();
    }
}
