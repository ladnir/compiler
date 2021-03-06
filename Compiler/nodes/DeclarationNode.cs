﻿using Compiler.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class DeclarationNode : Node 
    {
        private Token dataType;
        private Token variableName;
        public LetType let;
        public List<Gate> mDecGates;

        public DeclarationNode(Token dataType, Token variableName, LetType type)
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


        public override void toCircuit(List<Gate> gates, ref int nextWireID, StringBuilder dot)
        {
            if (let == LetType.Input)
            {
                mDecGates = new List<Gate>();

                for (int i = 0; i < GetBitCount(); i++ )
                    mDecGates.Add( new InputWire(nextWireID++, variableName.getValue() + "_"+i, gates));
            }
            //if (let == LetType.Output)
            //    new OutputWire(nextWireID++, GetBitCount(), variableName.getValue(), gates);
        }

        public override string outputC(int tabCount)
        {
            throw new NotImplementedException();
        }

        internal int GetBitCount()
        {
            return ((DataTypeToken)dataType.baseToken).length;
        }
    }
}

