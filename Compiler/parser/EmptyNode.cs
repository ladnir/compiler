using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    public class EmptyNode : ExpressionNode
    {
        internal override bool isEmpty()
        {
            return true;
        }
    }
}
