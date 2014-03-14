﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class DeclarationNode : Node 
    {
        private Token dataType;
        private Token variableName;

        public DeclarationNode(Token dataType, Token variableName)
        {
            // TODO: Complete member initialization
            this.dataType = dataType;
            this.variableName = variableName;

        }
         
        public override string outputIBTL(int tabCount)
        {
            return "[ " + variableName.getValue() + " " + dataType.getValue() + " ]"; 
        }

        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            if (dataType.getValue() == "float")
            {
                sb.Append("0.0e ");
                sb.Append("{ f: " + variableName.getValue() + " }");
            }
            else if (dataType.getValue() == "string")
            {
                sb.Append("s\" \" { d: " + variableName.getValue() + " } ");
                //throw new Exception("havent done string dec yet" + dataType.locate());
            }
            else
            {
                sb.Append("0 ");
                sb.Append("{ " + variableName.getValue() + " }");
            }
        }

        public virtual string getVarName()
        {
            return variableName.getValue();
        }

        internal string getDataType()
        {
            return dataType.getValue();
        }

        internal Token gotToken()
        {
            return variableName;
        }
    }
}

