using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public abstract class ExpressionNode : Node
    {
        public abstract string getReturnType();


        //public abstract void toCircuit(List<Gate> output, ref int nextGateID, int bitCount);

        public abstract List<Gate> NodeOutGates
        {
            get;
        }
    }
}
