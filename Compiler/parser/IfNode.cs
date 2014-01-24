﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    class IfNode : Node , LocalScope
    {

        private ExpressionNode eval;
        private LocalScope parentScope;

        private ElseNode elseNode;

        public IfNode(ExpressionNode eval, LocalScope scope)
        {
            // TODO: Complete member initialization
            this.eval = eval;
            this.parentScope = scope;
        }

        public bool inScope(Token name)
        {
            throw new NotImplementedException();
        }

        public void addToScope(DeclarationNode localVar)
        {
            throw new NotImplementedException();
        }

        internal void addElse(ElseNode elseNode)
        {
            this.elseNode = elseNode;
        }
    }
}