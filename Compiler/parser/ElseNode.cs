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
        public bool inScope(Token name)
        {
            throw new NotImplementedException();
        }

        public void addToScope(DeclarationNode localVar)
        {
            throw new NotImplementedException();
        }
    }
}
