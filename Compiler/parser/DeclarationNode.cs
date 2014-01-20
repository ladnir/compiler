using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class DeclarationNode : Node 
    {
        private Token dataType;
        private Token variableName;
        private ExpressionNode expr;

        public DeclarationNode(Token dataType, Token variableName, ExpressionNode value)
        {
            // TODO: Complete member initialization
            this.dataType = dataType;
            this.variableName = variableName;
            this.expr = value;

        }
        
    }
}
