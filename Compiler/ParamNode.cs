using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class ParamNode : Node
    {
        private Token paramType;
        private Token paramName;

        public ParamNode(Token paramType, Token paramName)
        {
            // TODO: Complete member initialization
            this.paramType = paramType;
            this.paramName = paramName;
        }

        internal string output()
        {
            throw new NotImplementedException();
        }
    }
}
