using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class VariableNode : ExpressionNode
    {
        public DeclarationNode dec;
        private List<Gate> mGates;
        private List<Gate> mOutGates;


        public VariableNode(DeclarationNode dec)
        {
            // TODO: Complete member initialization
            this.dec = dec;
        }

        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            //if (Parser.debug) Console.Write(dec.getVarName()+" ");
            
            sb.Append( dec.getVarName() + " ");
        }
        public override string outputIBTL(int tabCount)
        {
            return dec.getVarName();
        }

        public string getVarName()
        {
            return dec.getVarName();
        }

        public override string getReturnType()
        {
            return dec.getDataType();
        }

        internal void SetOutGates(List<Gate> list, ref int nextWireID, List<Gate> output)
        {
            if (mOutGates != null)
            {
                for (int i = 0; i < mOutGates.Count; i++ )
                {
                    mOutGates[i].SetAsInternal();
                }
            }

            mOutGates = list;

            if(dec.let == parser.LetType.Output)
            {
                for(int i = 0; i < mOutGates.Count; i++)
                {
                    mOutGates[i].SetAsOutput(getVarName() + "_" + i);
                }
            }

            var bitCount =  dec.GetBitCount();
            while (mOutGates.Count < bitCount)
            {
                mOutGates.Add(new LiteralWire(nextWireID++, false, output));
            }

            if (mOutGates.Count > bitCount)
                throw new Exception("narrowing scope exception");
        }

        public override List<Gate> NodeOutGates
        {
            get 
            {
                return mOutGates; 
            }
        }

        public override void toCircuit(List<Gate> gates, ref int nextWireID, StringBuilder dot)
        {

        }

        public override string outputC(int tabCount)
        {
            throw new NotImplementedException();
        }

        internal int GetBitCount()
        {
            return dec.GetBitCount();
        }
    }
}
