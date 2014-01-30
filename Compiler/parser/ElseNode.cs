using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    class ElseNode : Node , LocalScope
    {
        private LocalScope scope;

        public ElseNode(LocalScope scope)
        {
            // TODO: Complete member initialization
            this.scope = scope;
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
