using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class ForLoopNode : Node , ILocalScopeNode
    {
        private AssignmentNode assignment;
        private ExpressionNode eval;
        private ExpressionNode incrementer;
        private ILocalScopeNode scope;
        private Node assignment1;

        private Dictionary<string, VariableNode> localVars = new Dictionary<string, VariableNode>();

        public ForLoopNode(Node assignment1, ExpressionNode eval, ExpressionNode incrementer, ILocalScopeNode parentScope)
        {
            // TODO: Complete member initialization
            this.assignment1 = assignment1;
            this.eval = eval;
            this.incrementer = incrementer;
            this.scope = parentScope;
        }

        public bool varInScope(string name)
        {
            if (localVars.ContainsKey(name)) return true;
            if (scope.varInScope(name)) return true;

            return false;
        }

        public void addToScope(DeclarationNode dec)
        {
            if (varInScope(dec.getVarName()))
                throw new Exception("error fln1 at " + dec.getVarName());

            VariableNode newVar = new VariableNode(dec);

            localVars.Add(dec.getVarName(), newVar);

        }

        public VariableNode getVarRef(string token)
        {
            if (localVars.ContainsKey(token))
                return localVars[token];
            else return scope.getVarRef(token);
        }

        public bool funcInScope(string token)
        {
            return scope.funcInScope(token);
        }

        public IFunctionNode getFuncRef(string token)
        {
            return scope.getFuncRef(token);
        }

        public void addToScope(UserFunctionNode func)
        {
            scope.addToScope(func);
        }

        public void defineFunc(string name)
        {
            scope.defineFunc(name);
        }
        public UserFunctionNode getParentFunc()
        {
            return scope.getParentFunc();
        }
    }
}
