using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    enum GateType
    {
        AND,
        OR,
        XOR,
        LITERAL,
        ANDNOT2
    }

    public abstract class Gate
    {
        public int mID;

        public Gate(int id, List<Gate> output)
        {
            mID = id;
            output.Add(this);
        }

        public abstract void outputCircuit(StringBuilder sb);
    }


    public class AndGate : Gate
    {
        Gate mLeft, mRight;

        public AndGate(int id, Gate left, Gate right, List<Gate> output)
            : base(id, output)
        {
            mLeft = left;
            mRight = right;
        }

        public override void outputCircuit(StringBuilder sb)
        {
            sb.Append("[" + GateType.AND + "," + mLeft.mID + "," + mRight.mID + "]");
        }
    }


    /// mRight is negated
    public class AndNot2Gate : Gate
    {
        Gate mLeft, mRight;

        public AndNot2Gate(int id, Gate left, Gate right, List<Gate> output)
            : base(id, output)
        {
            mLeft = left;
            mRight = right;
        }

        public override void outputCircuit(StringBuilder sb)
        {
            sb.Append("[" + GateType.ANDNOT2 + "," + mLeft.mID + "," + mRight.mID + "]");
        }
    }

    public class OrGate : Gate
    {
        Gate mLeft, mRight;

        public OrGate(int id, Gate left, Gate right, List<Gate> output)
            : base(id, output)
        {
            mLeft = left;
            mRight = right;
        }

        public override void outputCircuit(StringBuilder sb)
        {
            sb.Append("[" + GateType.OR + "," + mLeft.mID + "," + mRight.mID + "]");
        }
    }

    public class XorGate : Gate
    {
        Gate mLeft, mRight;

        public XorGate(int id, Gate left, Gate right, List<Gate> output)
            : base(id, output)
        {
            mLeft = left;
            mRight = right;
        }

        public override void outputCircuit(StringBuilder sb)
        {
            sb.Append("[" + GateType.XOR + "," + mLeft.mID + "," + mRight.mID + "]");
        }
    }

    public class LiteralWire : Gate
    {
        bool mWireValue;

        public LiteralWire(int id, bool wireValue, List<Gate> output)
            : base(id, output) 
        {
            mWireValue = wireValue;
        }

        public override void outputCircuit(StringBuilder sb)
        {
            sb.Append("[" + GateType.LITERAL + "," + mWireValue + "]");
        }
    }

}
