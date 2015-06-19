using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class AssignmentNode : Node 
    {
        private ExpressionNode expr;
        private VariableNode varNode;
        private Token location;

        public AssignmentNode(VariableNode varNode, ExpressionNode expr,Token location)
        {
            // TODO: Complete member initialization
            this.location = location;
            this.varNode = varNode;
            this.expr = expr;

            //varNode.SetOutGates( expr.NodeOutGates);
        }

        public override string outputIBTL(int tabCount)
        {
            string output = "[ := " + varNode.outputIBTL(tabCount) + " " + expr.outputIBTL(tabCount) + " ]";

            return output;
        }

        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            //if (Parser.debug) Console.Write(" { " + varNode.getVarName() + " } \n");

            expr.outputGForth(tabCount, sb);
            if(varNode.getReturnType() != expr.getReturnType()){
                castType(sb);
            }
            sb.Append(" TO "+varNode.getVarName());
        }


        private void castType(StringBuilder sb)
        {
            switch (varNode.getReturnType())
                {
                    case "int":
                        if (expr.getReturnType() == "float") sb.Append(" f>s ");
                        else throw new Exception("gforth generation error Assnigment Node."+location.locate()+@" 
int is not compattable with "+expr.getReturnType());

                        break;
                    case "float":
                        if (expr.getReturnType() == "int") sb.Append(" s>f ");
                        else throw new Exception("gforth generation error Assnigment Node." + location.locate() + @" 
float is not compattable with " + expr.getReturnType());

                        break;
                    case "bool":
                        if (expr.getReturnType() == "float") sb.Append(" f>s ");
                        else if (expr.getReturnType() == "int");
                        else throw new Exception("gforth generation error Assnigment Node." + location.locate() + @" 
bool is not compattable with " + expr.getReturnType());

                        break;
                    case"string":
                        if (expr.getReturnType() != "string") throw new Exception("gforth generation error Assnigment Node." + location.locate() + @" 
string is not compattable with " + expr.getReturnType());

                        break;
                    default:
                        throw new Exception("gforth generation error Assnigment Node." + location.locate() + @" 
unknown type at " + varNode.getReturnType());
                        break;
                }
        }


        public override void toCircuit(List<Gate> gates, ref int nextWireID, StringBuilder dot)
        {

            if (expr is Compiler.parser.LiteralNode)
            {
                var lit = (Compiler.parser.LiteralNode)expr;

                lit.SetBits(varNode.GetBitCount());
            }

            expr.toCircuit(gates, ref nextWireID, dot);
            varNode.SetOutGates(expr.NodeOutGates, ref nextWireID, gates);
            //throw new NotImplementedException();
        }

        public override string outputC(int tabCount)
        {
            throw new NotImplementedException();
        }
    }
}
