using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class ExpressionNode : Node
    {
        internal string getReturnType()
        {
            throw new NotImplementedException();
        }

        internal virtual bool isEmpty()
        {
            return false;
        }
    }
}
