﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    class OpNode : ExpressionNode
    {
        private Token opToken;
        private ExpressionNode leftExpr;
        private ExpressionNode rightExpr;

        public OpNode(Token opToken, ExpressionNode leftExpr, ExpressionNode rightExpr)
        {
            // TODO: Complete member initialization
            this.opToken = opToken;
            this.leftExpr = leftExpr;
            this.rightExpr = rightExpr;
        }

        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            //if (Parser.debug) Console.Write(opToken.getValue() + " ");

            if(isSpecialOper()){ // special cases
                outputSpecialGForth(tabCount, sb);

            }
            else
            {   // general bin or unnary opperators
                leftExpr.outputGForth(tabCount, sb);
                if (rightExpr != null)
                {
                    if (leftExpr.getReturnType() != rightExpr.getReturnType())
                        castTypes(leftExpr, rightExpr, sb);
                    rightExpr.outputGForth(tabCount, sb);
                    if (leftExpr.getReturnType() != rightExpr.getReturnType())
                        castTypes(rightExpr,leftExpr,  sb);

                }
                if(this.getReturnType() == "int")
                    sb.Append(" " + opToken.getValue());
                else if (this.getReturnType() == "float")
                    sb.Append(" f" + opToken.getValue());
                    
            }
        }

        private void outputSpecialGForth(int tabCount, StringBuilder sb)
        {
            if (opToken.getValue() == "%")
                outputModGForth(tabCount, sb);
            else if (opToken.getValue() == "-" && leftExpr.getReturnType() == "bool" && rightExpr == null)
            {
                leftExpr.outputGForth(tabCount, sb);
                sb.Append(" 0= ");
            }


            else  if (opToken.getValue() == "^")
                outputPowerGForth(tabCount, sb);
            
            else if (opToken.getValue() == "sin" ||
                opToken.getValue() == "cos" ||
                opToken.getValue() == "tan")
            {
                leftExpr.outputGForth(tabCount, sb);
                if (leftExpr.getReturnType() == "int")
                    sb.Append("s>f ");
                else if (leftExpr.getReturnType() != "float") throw new Exception("OpNode error, trig functions only work with int ad float values.");
                sb.Append("f"+opToken.getValue() + " ");

            }
            else if (opToken.getValue() == "!=")
            {
                leftExpr.outputGForth(tabCount, sb); 
                rightExpr.outputGForth(tabCount, sb);
                sb.Append(" <>");

            }
            else if (opToken.getValue() == "-" && rightExpr == null)
            {
                sb.Append("-");
                leftExpr.outputGForth(tabCount, sb);
                sb.Append(" ");

            }
            else
            {
                throw new Exception("OpNode error, unknown special oper type" + opToken.locate());
            }
        }

        private void outputPowerGForth(int tabCount, StringBuilder sb)
        {
            if (leftExpr.getReturnType() != "float" && leftExpr.getReturnType() != "int") throw new Exception("OpNode error, exponent base must be an int or float." + opToken.locate());
            if (!(rightExpr is LiteralNode)) throw new Exception("OpNode error, exponent power must be an int." + opToken.locate());
            long power = Convert.ToInt64(((LiteralNode)rightExpr).outputIBTL(tabCount));

            if (power < 0) throw new Exception("OpNode error, exponent power must be non negative." + opToken.locate());

            if (leftExpr.getReturnType() == "int") sb.Append(" 1 ");
            else if (leftExpr.getReturnType() == "float") sb.Append(" 1.0e ");

            for (int i = 0; i < power; i++)
            {
                leftExpr.outputGForth(tabCount, sb);
                sb.Append(" * ");
            }
        }

        private void outputModGForth(int tabCount, StringBuilder sb)
        {
            if (leftExpr == null || rightExpr == null) throw new Exception("OpNode error, mod expects two arguments." + opToken.locate());

            switch (leftExpr.getReturnType())
            {
                case "float":   // algorithm for  (float % float ) or (float % int)
                    // looks like:   11e % 10e = 11e 11e 10e f/ f>s s>f 10e f* f-

                    leftExpr.outputGForth(tabCount, sb);
                    sb.Append(" ");
                    leftExpr.outputGForth(tabCount, sb);
                    sb.Append(" ");
                    rightExpr.outputGForth(tabCount, sb);
                    sb.Append(" ");
                    if (rightExpr.getReturnType() == "int")
                        sb.Append("s>f ");

                    sb.Append("f/ f>s s>f ");
                    rightExpr.outputGForth(tabCount, sb);
                    sb.Append(" ");
                    if (rightExpr.getReturnType() == "int")
                        sb.Append("s>f ");

                    sb.Append(" f* f-");

                    break;
                case "int":
                    if (rightExpr.getReturnType() == "int")
                    {
                        leftExpr.outputGForth(tabCount, sb);
                        sb.Append(" ");
                        rightExpr.outputGForth(tabCount, sb);
                        sb.Append(" /mod ");
                    }
                    else if (rightExpr.getReturnType() == "float")
                    {
                        leftExpr.outputGForth(tabCount, sb);
                        sb.Append(" s>f ");
                        leftExpr.outputGForth(tabCount, sb);
                        sb.Append(" s>f ");
                        rightExpr.outputGForth(tabCount, sb);

                        sb.Append(" f/ f>s s>f ");
                        rightExpr.outputGForth(tabCount, sb);
                        sb.Append(" ");

                        sb.Append(" f* f-");

                    }
                    break;
                default:
                    throw new Exception("OpNode error, mof only works on int and floats." + opToken.locate());
                    break;
            }
        }

        private bool isSpecialOper()
        {
            if (opToken.getValue() == "sin" ||
                opToken.getValue() == "cos" ||
                opToken.getValue() == "tan"||
                opToken.getValue() == "and"||
                opToken.getValue() == "or"||
                opToken.getValue() == "^"||
                opToken.getValue() == "%"||
                opToken.getValue() == "!=") return true;

            if (opToken.getValue() == "-" && leftExpr.getReturnType() == "bool" && rightExpr == null) return true;
            return false;
        }

        private void castTypes(ExpressionNode node,ExpressionNode other,StringBuilder sb)
        {
            
            switch (node.getReturnType()){
                
                case "int":
                    if (other.getReturnType() == "float") sb.Append(" s>f ");
                    else throw new Exception("gforth generation error Assnigment Node."+@" 
int is not compattable with " + other.getReturnType() + "   \n  "+opToken.locate());

                    break;
                case "float":
                    // do nothing? ha
                    break;
                case "bool":
                    if (other.getReturnType() == "float") sb.Append(" s>f");
                    else if (other.getReturnType() == "int") ;
                    else throw new Exception("gforth generation error Assnigment Node." + @" 
bool is not compattable with " + other.getReturnType() + "   \n  " + opToken.locate());

                    break;
                case"string":
                    if (other.getReturnType() != "string") throw new Exception("gforth generation error Assnigment Node." + @" 
string is not compattable with " + other.getReturnType() + "   \n  " + opToken.locate());

                    break;
                default:
                    throw new Exception("gforth generation error Assnigment Node." + @" 
unknown type at " + node.getReturnType() + "   \n  " + opToken.locate());
                    break;
            }
        }



        

        public override string outputIBTL(int tabCount)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[ " + opToken.getValue() + " " + leftExpr.outputIBTL(tabCount) );

            if (rightExpr != null)
            {
                sb.Append(" " + rightExpr.outputIBTL(tabCount));
            }

            sb.Append(" ] ");

            return sb.ToString();
        }

        public override string getReturnType()
        {
            if (opToken.getValue() == "=" ||
                opToken.getValue() == ">" ||
                opToken.getValue() == "<" ||
                opToken.getValue() == "<=" ||
                opToken.getValue() == ">=" ||
                opToken.getValue() == "!=" ||
                opToken.getValue() == "and" ||
                opToken.getValue() == "or" ) return "bool";

            if (opToken.getValue() == "sin" ||
                opToken.getValue() == "cos" ||
                opToken.getValue() == "tan") return "float";
            

            if (rightExpr != null)
            {
                if ((leftExpr.getReturnType() == "string" || rightExpr.getReturnType() == "string")
                    && opToken.getValue() == "+")
                    return "string";

                if (leftExpr.getReturnType() == "float" || rightExpr.getReturnType() == "float")
                    return "float";

                if (leftExpr.getReturnType() == "int" || rightExpr.getReturnType() == "int")
                    return "int";

                
                throw new Exception("OpNode error, reuturn type know know." + opToken.locate());

            } return leftExpr.getReturnType();
        }
    }
}
