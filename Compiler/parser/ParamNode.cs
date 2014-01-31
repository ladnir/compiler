using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class ParamNode : VariableNode
    {
        

        public ParamNode(Token paramType, Token paramName)
            : base(new DeclarationNode(paramType,paramName))
        {
            // TODO: Complete member initialization
        }

        public override string outputIBTL()
        {
            throw new NotImplementedException();
        }

        internal string getVarName()
        {
            return base.getVarName();
        }

        internal string getDataType()
        {
            return base.getReturnType();
        }
    }
}
