﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class ParamNode : VariableNode
    {
        

        public ParamNode(Token paramType, Token paramName)
            : base(new DeclarationNode(paramType,paramName, parser.LetType.Local))
        {
            
        }
        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            //sb.Append("{ ");
            base.outputGForth(tabCount, sb);

            //sb.Append(" } ");
        }
        public override string outputIBTL(int tabCount)
        {
            return base.outputIBTL(tabCount);
        }


        internal string getDataType()
        {
            return base.getReturnType();
        }
    }
}
