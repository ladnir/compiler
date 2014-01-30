using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    class WhileLoopNode : Node , LocalScope
    {
        private ExpressionNode eval;
        private LocalScope parentScope;

        public WhileLoopNode(ExpressionNode eval, LocalScope scope)
        {
            // TODO: Complete member initialization
            this.eval = eval;
            this.parentScope = scope;
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
