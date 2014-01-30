using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class ParamNode : Node
    {
        private Token paramType;
        private Token paramName;

        public ParamNode(Token paramType, Token paramName)
        {
            // TODO: Complete member initialization
            this.paramType = paramType;
            this.paramName = paramName;
        }

        public override string outputIBTL()
        {
            throw new NotImplementedException();
        }

        internal string label()
        {
            return paramName.getValue();
            throw new NotImplementedException();
        }

        internal string getDataType()
        {
            throw new NotImplementedException();
        }
    }
}
