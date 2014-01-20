using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Parser
    {
        private string[] dataTypes = { "int", "bool", "real", "string", "void" };
        private string[] constructs = { "for", "while","if"};
        
        private Node root;
        private int index,length;
        private Token[] tokens;
        private SymbolTable symbolTable;


        public Node parseTokens(Token[] tokens)
        {
            this.tokens = tokens;
            index = 0;
            length=tokens.Length;
            root = new Node();

            while (index < length)
            {
                Node function = parseFunction();
                root.addChild(function);

            }

            return null;
        }

        private Node parseFunction()
        {
            Token returnType;
            Token functionName;
            LinkedList<ParamNode> parameters;
            LinkedList<Node> children;

            if (!ofType(tokens[index], dataTypes)) 
                throw new Exception("error 1 at token:" + index);
            returnType = tokens[index++];

            if (tokens[index].getType() != TokenType.REF) 
                throw new Exception("error 2 at token:" + index);
            functionName = tokens[index++];

            FunctionNode fn = new FunctionNode(returnType, functionName);

            if (tokens[index].getValue() != "(" ) 
                throw new Exception("error 3 at token:" + index);
            
            parameters = parseParameter();

            if (tokens[index].getValue() != "{") 
                throw new Exception("error 5 at token:" + index);
            index++;

            children = parseStatements();

            if (tokens[index].getValue() != "}") 
                throw new Exception("error 6 at token:" + index);
            index++;

            return null;
        }

        private LinkedList<Node> parseStatements()
        {
            LinkedList<Node> children = new LinkedList<Node>();
            while (tokens[index].getValue() != "}")
            {
                // Parse Declaration
                if (ofType(tokens[index], dataTypes))
                    children.AddLast(parseDeclaration());

                else if (tokens[index].getType() == TokenType.REF)
                {
                    // Parse assignment.      ex:  var = expression;
                    if (tokens[index + 1].getValue() == "=")
                        children.AddLast(parseAssignment() );
                    // Parse function call
                    else if (tokens[index + 1].getValue() == "(")
                        children.AddLast(parseCall() );
                    else throw new Exception("error 7 at token:" + index);
                }
                // Parse construct.   ex: for(;;)
                else if(ofType(tokens[index],constructs)){
                    children.AddLast(parseConstruct() );
                }
                else
                {
                    throw new Exception("error 8 at token:" + index);
                }
                    
                
            }
            return children;
        }

        private Node parseDeclaration()
        {
            Token dataType;
            Token variableName;
            Node value;

            if (!ofType(tokens[index], dataTypes)) throw new Exception("error 9 at token:" + index);
            dataType = tokens[index++];

            if (tokens[index].getType() != TokenType.REF ) throw new Exception("error 10 at token:" + index);
            variableName = tokens[index++];

            if (tokens[index].getValue() == "=")
            {
                index++;
                value = parseExpression();
            }
            else if ( ! tokens[index].getValue() == ";") throw new Exception("error 11 at token:" + index);

            return new DeclarationNode(dataType,variableName,value);
        }

        private LinkedList<ParamNode> parseParameter()
        {
            LinkedList<ParamNode> paramList = new LinkedList<ParamNode>();

            if (tokens[index].getValue() != "(") throw new Exception("error 12 at token:" + index);
            index++;

            while (tokens[index].getValue() != ")")
            {
                if (!ofType(tokens[index], dataTypes)) throw new Exception("error 13 at token:" + index);
                Token paramType = tokens[index++];

                if ( tokens[index].getType() != TokenType.REF ) throw new Exception("error 14 at token:" + index);
                Token paramName = tokens[index++];

                paramList.AddLast(new ParamNode(paramType, paramName));

                if(  tokens[index++].getValue() != "," || tokens[index++].getValue() != ")" )
                    throw new Exception("error 14 at token:" + index);

                index++;
            }

            index++;
            return paramList;

            //throw new NotImplementedException();
        }

        private bool ofType(Token token, string[] set)
        {
            foreach (string s in set)
            {
                if (s == token.getValue()) return true;
            }
            return false;
        }
    }
}
