using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class DeclarationNode : Node 
    {
        private Token dataType;
        private Token variableName;

        public DeclarationNode(Token dataType, Token variableName)
        {
            // TODO: Complete member initialization
            this.dataType = dataType;
            this.variableName = variableName;

        }

        public override string outputIBTL(int tabCount)
        {
            return "[ " + variableName.getValue() + " " + dataType.getValue() + " ]"; 
        }

        public virtual string getVarName()
        {
            return variableName.getValue();
        }
    }
}

