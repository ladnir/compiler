using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    public class LiteralNode : ExpressionNode
    {
        private Token litToken;

        public LiteralNode(Token nameToken)
        {
            // TODO: Complete member initialization
            this.litToken = nameToken;
        }

        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            //if (Parser.debug) Console.Write(nameToken.getValue());

            if (litToken.getTokenType() == TokenType.BOOL)
            {
                if (litToken.getValue() == "true") sb.Append("-1 ");
                else sb.Append("0 ");
            }

            else if (litToken.getTokenType() == TokenType.FLOAT)
            {
                if (litToken.getValue().Contains('e'))
                    sb.Append(litToken.getValue() + " ");
                else
                    sb.Append(litToken.getValue() + "e ");
            }
            else if (litToken.getTokenType() == TokenType.INT)
                sb.Append(litToken.getValue() + " ");

            else if (litToken.getTokenType() == TokenType.STRING)
                sb.Append("s\" "+litToken.getValue()+"\"" );

            else throw new Exception("unknown literal at " + litToken.locate() + "  " + litToken.getTokenType());
       
        }
        public override string outputIBTL(int tabCount)
        {
            if (litToken.getTokenType() == TokenType.BOOL)
                return litToken.getValue();

            else if (litToken.getTokenType() == TokenType.FLOAT)
                return litToken.getValue();

            else if (litToken.getTokenType() == TokenType.INT)
                return litToken.getValue();

            else if (litToken.getTokenType() == TokenType.STRING)
                return "\"" + litToken.getValue() + "\"";

            else throw new Exception("unknown literal at "+litToken.locate());
        }

        public override string getReturnType()
        {
            if (litToken.getTokenType() == TokenType.INT) return "int";
            if (litToken.getTokenType() == TokenType.STRING) return "string";
            if (litToken.getTokenType() == TokenType.FLOAT) return "float";
            if (litToken.getTokenType() == TokenType.BOOL) return "bool";

            throw new Exception("error  literal " + litToken.locate());
        }
    }
}
