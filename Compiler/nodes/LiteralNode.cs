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
        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            //if (Parser.debug) Console.Write(nameToken.getValue());
            
            if (nameToken.getTokenType() == TokenType.BOOL){
                if (nameToken.getValue() == "true") sb.Append("-1 ");
                else sb.Append("0 ");
            }

            else if (nameToken.getTokenType() == TokenType.FLOAT)
                sb.Append( nameToken.getValue()+" ");

            else if (nameToken.getTokenType() == TokenType.INT)
                sb.Append( nameToken.getValue()+ " " );

            else if (nameToken.getTokenType() == TokenType.STRING)
                throw new NotImplementedException("strings are not supported");

            else throw new Exception("unknown literal at " + nameToken.locate() + "  "+nameToken.getTokenType());
       
        }
        public override string outputIBTL(int tabCount)
        {
            if (nameToken.getTokenType() == TokenType.BOOL)
                return nameToken.getValue();

            else if (nameToken.getTokenType() == TokenType.FLOAT)
                return nameToken.getValue();

            else if (nameToken.getTokenType() == TokenType.INT)
                return nameToken.getValue();

            else if (nameToken.getTokenType() == TokenType.STRING)
                return "\"" + nameToken.getValue() + "\"";

            else throw new Exception("unknown literal at "+nameToken.locate());
        }

        public override string getReturnType()
        {
            if (nameToken.getTokenType() == TokenType.INT) return "int";
            if (nameToken.getTokenType() == TokenType.STRING) return "string";
            if (nameToken.getTokenType() == TokenType.FLOAT) return "float";
            if (nameToken.getTokenType() == TokenType.BOOL) return "bool";

            throw new Exception("error  literal " + nameToken.locate());
        }
    }
}
