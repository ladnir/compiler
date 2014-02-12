using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class VariableNode : ExpressionNode
    {
        private DeclarationNode dec;


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
    }
}
