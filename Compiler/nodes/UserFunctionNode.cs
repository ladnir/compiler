﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.parser;

namespace Compiler
{
    public class UserFunctionNode : Node , ILocalScopeNode ,IFunctionNode
    {
        private LinkedList<ParamNode> parameters;

        private Token returnType;
        private Token functionName;
        private Dictionary<string, VariableNode> localVars = new Dictionary<string, VariableNode>();
        private RootNode root;

        public UserFunctionNode(Token returnType, Token functionName, LinkedList<Token> parameterNames, LinkedList<Token> parameterTypes,RootNode root)
        {
            //UserFunctionNode std = new StdoutNode();
            this.parameters = new LinkedList<ParamNode>();

            this.root = root;
            this.returnType = returnType;
            this.functionName = functionName;

            // check param counts
            if (parameterNames.Count != parameterTypes.Count)
                throw new Exception("error fn1, paramiter name count doesnt match paramter type count for function "+functionName.getValue() );

            // add the parameters to the function.
            //LinkedList<Token>.Enumerator types = parameterTypes.GetEnumerator();
            Token[] hack = parameterTypes.ToArray<Token>();
            int i = 0;
            foreach (Token name in parameterNames)
            {
                
                ParamNode param = new ParamNode(hack[i++], name);
              //  types.MoveNext();

                parameters.AddLast(param);
                localVars.Add(name.getValue(), param);
            }
        }

        //============================================================================
        // output function
        //============================================================================

        override public string outputIBTL(int tabCount)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[ let [ ");
            sb.Append(functionName.getValue());
            sb.Append(" ");

            LinkedListNode<ParamNode> cur = parameters.First;
            for (int i = 0; i < parameters.Count; i++)
            {
                sb.Append(cur.Value.getVarName() + " ");

                cur = cur.Next;
            }

            sb.Append("] [ ");

            sb.Append(returnType.getValue() + " ");
            cur = parameters.First;
            for (int i = 0; i < parameters.Count; i++)
            {
                sb.Append(cur.Value.getReturnType() + " ");

                cur = cur.Next;
            }

            sb.Append("] \n");

            LinkedListNode<Node> child = children.First;
            for (int i = 0; i < children.Count; i++)
            {
                sb.Append(Node.getTabs(tabCount) + child.Value.outputIBTL(tabCount + 1) + "\n");
                child = child.Next;
            }

            sb.Append(Node.getTabs(tabCount-1) + "]");

            return sb.ToString();
        }

        override public string outputC(int tabCount)
        {
            throw new NotImplementedException();

            StringBuilder sb = new StringBuilder();

            sb.Append(returnType.getValue());
            sb.Append(" ");
            sb.Append(functionName.getValue());
            sb.Append("(");

            LinkedListNode<ParamNode> cur = parameters.First;
            for (int i = 0; i < parameters.Count; i++)
            {
                if (i != 0) sb.Append(",");
                sb.Append(cur.Value.outputIBTL(tabCount));

                cur = cur.Next;
            }

            sb.Append("){ \n");

            LinkedListNode<Node> child = children.First;
            for (int i = 0; i < children.Count; i++)
            {
                sb.Append(child.Value.outputIBTL(tabCount));
            }

            sb.Append("}\n");

            return sb.ToString();
        }

        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            if (Parser.debug) Console.Write(functionName.getValue());

            sb.Append(": " + functionName.getValue() + " { ");

            foreach (ParamNode p in parameters)
            {
                
                sb.Append(p.getVarName() + " ");
            }

            sb.Append("}\n");
            foreach (Node n in children)
            {

                if (Parser.debug) Console.Write(" t ");

                sb.Append(Node.getTabs(tabCount + 1));
                n.outputGForth(tabCount+1,sb);
                sb.Append("\n");
            }

            sb.Append(" ; \n");
        }

        //============================================================================
        // FunctionNode function
        //============================================================================

        IEnumerable<ParamNode> IFunctionNode.getParameters()
        {
            return parameters;
        }

        string IFunctionNode.getName()
        {
            return functionName.getValue();
        }

        string IFunctionNode.getReturnType()
        {
            return returnType.getValue();
        }
        //============================================================================
        // Scoping function
        //============================================================================
        
        public bool varInScope(string name)
        {
            if (localVars.ContainsKey(name)) return true;
            //if (root.varInScope(name)) return true;

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
            else return root.getVarRef(token);
        }

        public bool funcInScope(string token)
        {
            return root.funcInScope(token);
        }

        public IFunctionNode getFuncRef(string token)
        {
            return root.getFuncRef(token);
        }

        public void addToScope(UserFunctionNode func)
        {
            root.addToScope(func);
        }


    }
}