using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    public class LiteralNode : ExpressionNode
    {
        private Token nameToken;

        public LiteralNode(Token nameToken)
        {
            // TODO: Complete member initialization
            this.nameToken = nameToken;
        }

        public override string outputIBTL(int tabCount)
        {
            if (nameToken.getTokenType() == TokenType.BOOL)
                return nameToken.getValue();

            if (nameToken.getTokenType() == TokenType.FLOAT)
                return nameToken.getValue();

            if (nameToken.getTokenType() == TokenType.INT)
                return nameToken.getValue();

            if (nameToken.getTokenType() == TokenType.STRING)
                return "\"" + nameToken.getValue() + "\"";

            else throw new Exception("unknown literal at "+nameToken.locate());
        }
    }
}
