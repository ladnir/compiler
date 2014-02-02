using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public abstract class ExpressionNode : Node
    {
        public abstract string getReturnType();

        public override string outputIBTL(int tabCount)
        {
            return base.outputIBTL(tabCount);
        }
    }
}
