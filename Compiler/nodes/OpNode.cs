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
            if (Parser.debug) Console.Write(opToken.getValue() + " ");
            
            leftExpr.outputGForth(tabCount, sb);
            if(rightExpr!= null) rightExpr.outputGForth(tabCount, sb);

            sb.Append(" " + opToken.getValue());
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
                opToken.getValue() == "!=") return "bool";

            return leftExpr.getReturnType();
        }
    }
}