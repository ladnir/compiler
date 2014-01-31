using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class RootNode : Node , LocalScope
    {
        public void addFunction(FunctionNode fn)
        {
            throw new NotImplementedException();
        }


        public bool varInScope(Token name)
        {
            throw new NotImplementedException();
        }

        public void addToScope(DeclarationNode localVar)
        {
            throw new NotImplementedException();
        }


        public bool funcInScope(CallNode function)
        {
            throw new NotImplementedException();
        }

        public bool funcInScope(FunctionNode fn)
        {
            throw new NotImplementedException();
        }


        public string getDataType(Token varName)
        {
            throw new NotImplementedException();
        }
    }
}
