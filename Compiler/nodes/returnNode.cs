using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class ReturnNode :Node
    {
        private ExpressionNode returnExpr;
        private Token r;
        private UserFunctionNode func;

        public ReturnNode(ExpressionNode returnExpr, Token r, UserFunctionNode func)
        {
            // TODO: Complete member initialization
            this.returnExpr = returnExpr;
            this.r = r;
            this.func = func;
        }
        override public void outputGForth(int tabCount, StringBuilder sb)
        {
            returnExpr.outputGForth(tabCount, sb);
            if (returnExpr.getReturnType() != func.getReturnType())
            {
                switch (func.getReturnType())
                {
                    case "int" :
                        if (returnExpr.getReturnType() == "float")
                            sb.Append(" f>s ");
                        else if (returnExpr.getReturnType() == "bool") ;
                        else if (returnExpr.getReturnType() == "string") throw new Exception("Can not type cast from string to int.\n" + r.locate());
                        else throw new Exception("unknown return type.\n" + r.locate());
                        break;
                    case "float" :
                        if (returnExpr.getReturnType() == "int")
                            sb.Append(" s>f ");
                        else if (returnExpr.getReturnType() == "bool") 
                            sb.Append(" s>f ");
                        else if (returnExpr.getReturnType() == "string") throw new Exception("Can not type cast from string to float.\n" + r.locate());
                        else throw new Exception("unknown return type.\n" + r.locate());
                        break;
                    case "bool" :
                        if (returnExpr.getReturnType() == "float")
                            sb.Append(" f>s ");
                        else if (returnExpr.getReturnType() == "int") ;
                        else if (returnExpr.getReturnType() == "string") throw new Exception("Can not type cast from string to bool.\n" + r.locate());
                        else throw new Exception("unknown return type.\n" + r.locate());
                        break;
                    case "string" :
                        if (returnExpr.getReturnType() == "float") throw new Exception("Can not type cast from float to string.\n" + r.locate());
                        else if (returnExpr.getReturnType() == "bool") throw new Exception("Can not type cast from bool to string.\n" + r.locate());
                        else if (returnExpr.getReturnType() == "int") throw new Exception("Can not type cast from int to string.\n" + r.locate());
                        else throw new Exception("unknown return type.\n" + r.locate());
                        break;
                    default: throw new Exception("unknown return type.\n"+r.locate());
                }
            }
            sb.Append("\n" + Node.getTabs(tabCount) + "exit \n");

        }
   
    }
}
