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
        ANDNOT2,
        Print,
        NXOR,
        NAND,
        NOR,
        Input,
        Output
    }

    public abstract class Gate
    {
        public int mID;
        public bool Value;
        private bool mIsOutput = false;
        public string mOutLabel;

        public Gate(int id, List<Gate> output)
        {
            mID = id;
            output.Add(this);
        }

        public abstract void outputCircuit(StringBuilder sb);

        internal abstract void Evalutate();

        public void SetAsOutput(string label)
        {
            mIsOutput = true;
            mOutLabel = label;
        }

        public void SetAsInternal()
        {
            mIsOutput = false;
            mOutLabel = "";
        }
    }

    public class InputWire :Gate
    {
        string mLabel;

        public InputWire(int id, string label, List<Gate> output)
            : base(id, output)
        {
            mLabel = label;
        }
        
        public override void outputCircuit(StringBuilder sb)
        {
            sb.Append("[" + GateType.Input+ "," + mLabel +"]");
        }


        internal override void Evalutate()
        {
        }
    }


    //public class OutWire : Gate
    //{
    //    string mLabel;

    //    public OutWire(int id, string label, List<Gate> output)
    //        : base(id, output)
    //    {
    //        mLabel = label;
    //    }

    //    public override void outputCircuit(StringBuilder sb)
    //    {
    //        sb.Append("[" + GateType.Output + "," + mLabel + "]");
    //    }


    //    internal override void Evalutate()
    //    {
    //    }
    //}

    public class PrintGate :Gate
    {
        List<Gate> mGates;
        public PrintGate(int id, List<Gate> gates, List<Gate> output) : base(id,output)
        {
            mGates = gates;
        }
                
        public override void outputCircuit(StringBuilder sb)
        {
            sb.Append("[" + GateType.Print );

            foreach(var gate in mGates)
            {
                sb.Append("," + gate.mID);
            }

            sb.Append("]");
        }


        internal override void Evalutate()
        {
            UInt64 num=0, mask =1;

            if (mGates.Count > 64)
                throw new Exception();

            foreach(var gate in mGates)
            {
                if(gate.Value)
                {
                    num |= mask;
                }
                mask <<= 1;
            }

            Console.WriteLine(num);
        }

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

        internal override void Evalutate()
        {
            Value = mLeft.Value & mRight.Value;
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

        internal override void Evalutate()
        {
            Value = mLeft.Value && ! mRight.Value;
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

        internal override void Evalutate()
        {
            Value = mLeft.Value || mRight.Value;
        }
    }
        public class NorGate : Gate
    {
        Gate mLeft, mRight;

        public NorGate(int id, Gate left, Gate right, List<Gate> output)
            : base(id, output)
        {
            mLeft = left;
            mRight = right;
        }

        public override void outputCircuit(StringBuilder sb)
        {
            sb.Append("[" + GateType.NOR + "," + mLeft.mID + "," + mRight.mID + "]");
        }

        internal override void Evalutate()
        {
            Value = !(mLeft.Value || mRight.Value);
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

        internal override void Evalutate()
        {
            Value = mLeft.Value ^ mRight.Value;
        }
    }


    public class NXorGate : Gate
    {
        Gate mLeft, mRight;

        public NXorGate(int id, Gate left, Gate right, List<Gate> output)
            : base(id, output)
        {
            mLeft = left;
            mRight = right;
        }

        public override void outputCircuit(StringBuilder sb)
        {
            sb.Append("[" + GateType.NXOR + "," + mLeft.mID + "," + mRight.mID + "]");
        }

        internal override void Evalutate()
        {
            Value =! (mLeft.Value ^ mRight.Value);
        }
    }

    public class LiteralWire : Gate
    {
        public LiteralWire(int id, bool wireValue, List<Gate> output)
            : base(id, output) 
        {
            Value = wireValue;
        }

        public override void outputCircuit(StringBuilder sb)
        {
            sb.Append("[" + GateType.LITERAL + "," + Value + "]");
        }

        internal override void Evalutate()
        {
            
        }
    }

}
