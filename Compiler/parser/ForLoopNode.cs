using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    public class ForLoopNode : Node , LocalScope
    {
        private AssignmentNode assignment;
        private ExpressionNode eval;
        private ExpressionNode incrementer;
        private LocalScope parentScope;
        private Node assignment1;

        public ForLoopNode(Node assignment1, ExpressionNode eval, ExpressionNode incrementer, LocalScope parentScope)
        {
            // TODO: Complete member initialization
            this.assignment1 = assignment1;
            this.eval = eval;
            this.incrementer = incrementer;
            this.parentScope = parentScope;
        }


        internal void addChildren(LinkedList<Node> children)
        {
            this.children = children;
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
