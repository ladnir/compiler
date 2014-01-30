using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    class WhileLoopNode : Node , LocalScope
    {
        private ExpressionNode eval;
        private LocalScope scope;

        public WhileLoopNode(ExpressionNode eval, LocalScope scope)
        {
            // TODO: Complete member initialization
            this.eval = eval;
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
