using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{
    class ElseNode : Node , LocalScope
    {
        private LocalScope scope;

        Dictionary<string, VariableNode> localVars = new Dictionary<string, VariableNode>();

        public ElseNode(LocalScope scope)
        {
            this.scope = scope;
        }
        public bool varInScope(Token name)
        {
            if (localVars.ContainsKey(name.getValue())) return true;
            if (scope.varInScope(name)) return true;

            return false;
        }

        public void addToScope(DeclarationNode dec)
        {
            if(varInScope(dec.getVarName()))
                throw new Exception("error en1 at "+dec.getVarName().getValue() );

            VariableNode newVar = new VariableNode(dec);

            localVars.Add(dec.getVarName().getValue(), newVar);

        }


        public bool funcInScope(CallNode function)
        {
            throw new NotImplementedException();
        }

        public bool funcInScope(FunctionNode fn)
        {
            throw new NotImplementedException();
        }

        public string getDataType(Token varName)
        {
            throw new NotImplementedException();
        }
    }
}
