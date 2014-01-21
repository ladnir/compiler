using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    class EmptyNode : ExpressionNode
    {
        internal override bool isEmpty()
        {
            return true;
        }
    }
}
