using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    public class LiteralNode : ExpressionNode
    {
        public Token litToken;
        private List<Gate> gates;
        private int mBitCount;

        public LiteralNode(Token nameToken)
        {
            // TODO: Complete member initialization
            this.litToken = nameToken;
            mBitCount = -1;
        }

        public override List<Gate> NodeOutGates
        {
            get { return gates; }
        }

        public override void toCircuit(List<Gate> output, ref int nextWireID, StringBuilder dot)
        {
            if (gates != null)
                throw new Exception();

            gates = new List<Gate>();

            if(litToken.getTokenType() == TokenType.INT)
            {
                dot.Append("subgraph cluster_" + litToken.locateShort() + " {\n label=\"literal_" + litToken.locateShort() + "\";\n");

                UInt64 value = UInt64.Parse( litToken.getValue());
                UInt64 mask = 1;

                while (value > 0)
                {
                    dot.Append("g" + nextWireID+";\n");
                    gates.Add(new LiteralWire(nextWireID++, (value & mask) != 0, output));
                    value >>= 1;
                }

                while (gates.Count < mBitCount)
                {
                    dot.Append("g" + nextWireID + ";\n");
                    gates.Add(new LiteralWire(nextWireID++, false, output));
                }

                dot.Append("}\n");
            }
            else
            {
                throw new NotImplementedException();
            }
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

        public override string outputC(int tabCount)
        {
            throw new NotImplementedException();
        }

        internal void SetBits(int p)
        {
            mBitCount = p;
        }
    }
}
