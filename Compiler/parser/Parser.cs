using Compiler.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class Parser
    {
        private string[] Op1Tpyes = { "==", "<=", ">=",">","<"};
        private string[] Op2Tpyes = { "+", "-" };
        private string[] Op3Tpyes = { "*", "/", "%" };

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

        /// <summary>
        /// Expects index to point to the return type of the function.
        /// 
        /// Add the function to the symble table.
        /// sets the index to the token after the } .
        /// Returns a node containing the function.
        /// </summary>
        /// <returns></returns>
        private Node parseFunction()
        {
            FunctionNode fn;
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

            fn = new FunctionNode(returnType, functionName);

            if (tokens[index].getValue() != "(" ) 
                throw new Exception("error 3 at token:" + index);
            
            parameters = parseParameter();

            if (tokens[index].getValue() != "{") 
                throw new Exception("error 5 at token:" + index);
            index++;

            children = parseStatements(fn);
            fn.addChildren(children);

            if (tokens[index].getValue() != "}") 
                throw new Exception("error 6 at token:" + index);
            index++;


            return fn;
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

                if (tokens[index].getType() != TokenType.REF) throw new Exception("error 14 at token:" + index);
                Token paramName = tokens[index++];

                paramList.AddLast(new ParamNode(paramType, paramName));

                if (tokens[index++].getValue() != "," || tokens[index++].getValue() != ")")
                    throw new Exception("error 14.1 at token:" + index);

                index++;
            }

            index++;
            return paramList;

        }

        /// <summary>
        /// Parses all statements inside a scope. (i.e. a function, for loop.) 
        /// will return once a closing brace '}' at its level is reached.
        /// returns the index on the closing brace.
        /// 
        /// effects: index
        /// </summary>
        /// <param name="scope"></param>
        /// <returns> a linked list of statements.</returns>
        private LinkedList<Node> parseStatements(LocalScope scope)
        {
            LinkedList<Node> children = new LinkedList<Node>();
            while (tokens[index].getValue() != "}")
            {
                // Parse Declaration
                if (ofType(tokens[index], dataTypes))
                    children.AddLast(parseDeclaration(scope));

                // some sort of reference. either local var or function call
                else if (tokens[index].getType() == TokenType.REF)
                {
                    // Parse assignment.      ex:  var = expression;
                    if (tokens[index + 1].getValue() == "=")
                        children.AddLast(parseAssignment(scope) );

                    // Parse function call
                    else if (tokens[index + 1].getValue() == "(")
                    {
                        Token[] exprList = getTokensToSemicolon();
                        children.AddLast(parseCall(exprList,scope));
                    }
                    else throw new Exception("error 7 at token:" + index);
                }
                // Parse construct.   ex: for(;;)
                else if(ofType(tokens[index],constructs)){
                    children.AddLast(parseConstruct(scope) );
                }
                else
                {
                    throw new Exception("error 8 at token:" + index);
                }
                    
                
            }
            return children;
        }

        /// <summary>
        /// Expects the local scope that it lives in. 
        /// Expects the index to point at the construct token.
        /// 
        /// Sets the index after the closing brace.
        /// Returns a construct node.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private Node parseConstruct(LocalScope scope)
        {
            if (tokens[index].getValue() == "for")
                return parseForLoop(scope);
            else if (tokens[index].getValue() == "while")
                return parseWhileLoop(scope);
            else if (tokens[index].getValue() == "if")
                return parseIf(scope);
            else
                throw new Exception("error, unknow construct at token:" + index);
        }

        /// <summary>
        /// Expects the local scope that it lives in. 
        /// Expects the index to point at the for token.
        /// 
        /// Sets the index after the closing brace.
        /// Returns a for loop node.
        /// </summary>
        /// <param name="scope"></param>
        private Node parseForLoop(LocalScope scope)
        {
            Node assignment;

            // check for the opening of the for loop
            if (tokens[index].getValue() != "for")throw new Exception("error 31 at token:" + index++);
            if (tokens[index].getValue() != "(") throw new Exception("error 32 at token:" + index++);
     
            // parse the optional assignment
            if (tokens[index].getValue() != ";")
            {
                assignment = parseAssignment(scope);
                index--; // so that we can check the semicolon. parseAssignment() increments index over it...
            }else assignment = new EmptyNode();

            // make sure we have a semicolon after the optional assignment
            if (tokens[index].getValue() != ";") throw new Exception("error 33 at token:" + index++);

            // get the evaluation tokens and parse them into a boolean node
            Token[] exprList = getTokensToSemicolon();
            ExpressionNode eval = parseExpression(exprList,scope);

            // check that the eval node returns a boolean
            if (eval.isEmpty())  throw new Exception("error 35 at token:" + index);
            if(eval.getReturnType() != "bool" )  throw new Exception("error 34 at token:" + index);

            // get the optional incrementer expression and then parse it
            exprList = getClosingTokens();
            ExpressionNode incrementer = parseExpression(exprList,scope);

            //check that we have the closing brace and openning brace to state the forloop body
            if (tokens[index].getValue() != ")") throw new Exception("error 36 at token:" + index++);
            if (tokens[index].getValue() != "{") throw new Exception("error 37 at token:" + index++);

            // create the forloop node so that the parse statements call will have a local scope to use
            ForLoopNode fl = new ForLoopNode(assignment,eval,incrementer,scope);

            // get the for loop's statments and use it as the local scope
            LinkedList<Node> children = parseStatements(fl);

            // add the children to the for loop
            fl.addChildren(children);

            // check for closing brace
            if (tokens[index].getValue() != "}") throw new Exception("error 38 at token:" + index++);

            return fl;
        }

        /// <summary>
        /// Sets index to point to a closing brace ) .
        /// Returns a series of zero of more tokens 
        /// starting at index and finishing before  ){ .
        /// </summary>
        /// <returns></returns>
        private Token[] getClosingTokens()
        {
            LinkedList<Token> tl = new LinkedList<Token>();

            while (tokens[index].getValue() != ")" && tokens[index + 1].getValue() != "{")
            {
                // if something goes wrong catch it. like there is a token between ){ or if one of them is missing
                if (tokens[index].getValue() == "{" || tokens[index].getValue() == "}") throw new Exception("error 39 at token:" + index);
                tl.AddLast(tokens[index++]);
            }

            return tl.ToArray();
        }
        
        /// <summary>
        /// Expects the scope that it lives in.
        /// Expects index to point to the lValue.
        /// 
        /// Sets index to point to the node after the closing brace { .
        /// Returns an assingment node.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private AssignmentNode parseAssignment(LocalScope scope)
        {
            AssignmentNode assignment;
            ExpressionNode expr;
            Token varName;

            varName = tokens[index++];

            // check that the token is in scope
            if (!scope.inScope(varName) )
                throw new Exception("token: " + index + "  " + varName.getValue() + " is not in scope");

            // make sure its an assignment
            if (tokens[index].getValue() != "=")
                throw new Exception("error 15 at token:" + index);

            index++;

            // get the expression;
            Token[] exprList = getTokensToSemicolon();

            // parse the expression into Node(s)
            expr = parseExpression(exprList , scope);

            assignment = new AssignmentNode(varName, expr);

            return assignment;
        }

        /// <summary>
        /// Expects the scope that it lives in.
        /// Expects index to point to the variable's type.
        /// Can parse declarations with assignments.
        /// 
        /// Sets index to point to the node after the closing brace { .
        /// Returns an assingment node.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private DeclarationNode parseDeclaration(LocalScope scope)
        {
            DeclarationNode dec;
            Token dataType;
            Token variableName;
            ExpressionNode expr = null;

            // check that its has a valid data type
            if (!ofType(tokens[index], dataTypes)) throw new Exception("error 9 at token:" + index);
            dataType = tokens[index++];

            // make sure its a reference token and get its label.
            if (tokens[index].getType() != TokenType.REF) throw new Exception("error 10 at token:" + index);
            variableName = tokens[index++];

            // make sure its not in scope.
            if (scope.inScope(variableName))
                throw new Exception("Token :" + index + "  " + variableName.getValue() + " is already in scope.");

            // check for an assignment
            if (tokens[index].getValue() == "=")
            {
                index++;
                // get the expression;
                Token[] exprList = getTokensToSemicolon();

                // parse the expression into Node(s)
                expr = parseExpression(exprList , scope);

                // checks to see if the expression is empty
                if (expr.isEmpty() ) throw new Exception("error 40 at token:" + index);
            }
            // make sure that it terminates with a semicolon
            else if (tokens[index].getValue() != ";") throw new Exception("error 11 at token:" + index);


            dec = new DeclarationNode(dataType, variableName, expr);

            // add the variable to local scope.
            scope.addToScope(dec);
            return dec;
        }

        /// <summary>
        /// Sets index to point at a ; .
        /// Returns all nodes until it sees a semicolon. 
        /// </summary>
        /// <returns></returns>
        private Token[] getTokensToSemicolon()
        {
            LinkedList<Token> tokenLL = new LinkedList<Token> ();
            while (tokens[index].getValue() != ";")
                tokenLL.AddLast(tokens[index++]);

            // step over the semicolon
            index++;

            return tokenLL.ToArray();
        }

        /// <summary>
        /// Expects the scope that it lives in.
        /// Expects index to point to the start of an expression (i.e.  3*4+foo()  ) .
        /// 
        /// Sets index to point to the node after the closing brace { .
        /// Returns an possible EMPTY Expression node.
        /// </summary>
        /// <param name="exprList"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        private ExpressionNode parseExpression(Token[] exprList,LocalScope scope)
        {
            if (exprList.Length == 0)
                return new EmptyNode();

            // single literal or variable or function call
            if (isSingleExpression(exprList,scope))
            {
                return parseSingleExpression(exprList, scope);
            }

            // composite expression
            return splitExpression(exprList, scope);
            
        }

        private OperationNode splitExpression(Token[] exprList, LocalScope scope)
        {
            // error checks
            if (isSingleExpression(exprList,scope))
                throw new Exception("error 19 at token:" + index);
            
            if(exprList[0].getType() != TokenType.REF || ! exprList[0].isLiteral())
                throw new Exception("error 18 at token:" + index); 

            int  i = 1;

            // look for a == , <= ,>= , < , > to split the expression on   :  Op1Types
            while (i < exprList.Length && !ofType(exprList[i], Op1Tpyes)) i++;
            if (i < exprList.Length) // we found an == or something equivalent
            {
                return parseBooleanExpression(i,exprList,scope);
            }

            i = 1;
            // look for a +, - to split the expression on                 :  Op2Types
            while (i < exprList.Length && !ofType(exprList[i], Op2Tpyes)) i++;
            if (i < exprList.Length) // we found an + or -
            {
                return parseAdditionExpression(i, exprList, scope);
            } 
            
            i = 1;
            // look for a * , / , % to split the expression on             :  Op3Types
            while (i < exprList.Length && !ofType(exprList[i], Op3Tpyes)) i++;
            if (i < exprList.Length) // we found an  * , / , %
            {
                return parseMultiplicationExpression(i, exprList, scope);
            }


            // shouldnt get here 
            throw new Exception("error 23 at token:" + index); 
        }

        private OperationNode parseMultiplicationExpression(int i, Token[] exprList, LocalScope scope)
        {
            int j = 0;
            while (j < exprList.Length && !ofType(exprList[j], Op1Tpyes) && !ofType(exprList[j], Op2Tpyes)) j++;
            if (j < exprList.Length) // we found an == , + or something equivalent which is bad because they should have taken presidence in the recursion
                throw new Exception("error 24 at token:" + index);

            if (!ofType(exprList[i], Op3Tpyes))
                throw new Exception("error 27 at token:" + index); 

            Token[] left, right;

            left = new Token[i];
            right = new Token[exprList.Length - i - 1];

            for ( j = 0; j < i; j++)
                left[j] = exprList[j];

            for ( j = i; j < exprList.Length; j++)
                right[j] = exprList[j];

            ExpressionNode leftExpr, rightExpr;

            if (isSingleExpression(left,scope))
                leftExpr = parseSingleExpression(left, scope);
            else leftExpr = splitExpression(left, scope);

            if (isSingleExpression(right,scope))
                rightExpr = parseSingleExpression(right, scope);
            else rightExpr = splitExpression(right, scope);

            return new OperationNode(exprList[i], leftExpr, rightExpr);

        }

        private OperationNode parseAdditionExpression(int i, Token[] exprList, LocalScope scope)
        {
            int j = 0;
            while (j < exprList.Length && !ofType(exprList[j], Op1Tpyes)) j++;
            if (j < exprList.Length) // we found an == or something equivalent which is bad because they should have taken presidence in the recursion
                throw new Exception("error 24 at token:" + index); 

            if(!ofType(exprList[i], Op2Tpyes))
                throw new Exception("error 25 at token:" + index); 

            Token[] left, right;

            left = new Token[i];
            right = new Token[exprList.Length - i - 1];

            for ( j = 0; j < i; j++)
                left[j] = exprList[j];

            for ( j = i; j < exprList.Length; j++)
                right[j] = exprList[j];

            ExpressionNode leftExpr, rightExpr;

            if (isSingleExpression(left,scope))
                leftExpr = parseSingleExpression(left, scope);
            else leftExpr = splitExpression(left, scope);

            if (isSingleExpression(right,scope))
                rightExpr = parseSingleExpression(right, scope);
            else rightExpr = splitExpression(right, scope);

            return new OperationNode(exprList[i], leftExpr, rightExpr);
        }
        
        private OperationNode parseBooleanExpression(int i,Token[] exprList, LocalScope scope)
        {

            if (!ofType(exprList[i], Op1Tpyes))
                throw new Exception("error 26 at token:" + index); 

            Token[] left, right;

            left = new Token[i];
            right = new Token[exprList.Length - i - 1];

            for (int j = 0; j < i; j++)
            {
                left[j] = exprList[j];
                if (ofType(left[j], Op1Tpyes))
                    throw new Exception("error 20 at token:" + index);
            }
            for (int j = i; j < exprList.Length; j++)
            {
                right[j] = exprList[j];
                if (ofType(right[j], Op1Tpyes))
                    throw new Exception("error 21 at token:" + index);
            }

            ExpressionNode leftExpr, rightExpr;

            if (isSingleExpression(left,scope))
                leftExpr = parseSingleExpression(left, scope);
            else leftExpr = splitExpression(left, scope);

            if (isSingleExpression(right,scope))
                rightExpr = parseSingleExpression(right, scope);
            else rightExpr = splitExpression(right, scope);

            return new OperationNode(exprList[i], leftExpr, rightExpr);
        }

        private bool isSingleExpression(Token[] tokens , LocalScope scope)
        {
            SymbolTable sym = new SymbolTable ();
            // single literal
            if (tokens[0].isLiteral() && tokens.Length == 1) return true;

            // reference
            Token refName =tokens[0];
            if (refName.getType() == TokenType.REF)
            {
                // single local variable
                if (scope.inScope( refName ) && tokens.Length == 1) return true;

                // cant have a function call without 3 tokens  ex:  REF  (  )
                if(tokens.Length <= 3) return false;

                if (tokens[1].getValue() != "(") return false;

                int i = 2;
                CallNode funcCall = new CallNode(refName);

                // walk over the function params checking the format
                while (i + 1 < tokens.Length && (tokens[i].getType() == TokenType.REF || tokens[i].isLiteral() ))
                {
                    if (tokens[i].getType() != TokenType.REF || !tokens[i].isLiteral())
                        throw new Exception("error 31 at token:" + index);

                    funcCall.addParam(tokens[i]);

                    if (tokens[i + 1].getValue() == ")" && i + 2 == tokens.Length)
                    {
                        if (sym.funcInScope(funcCall))
                        {
                            if (i + 2 == tokens.Length)
                                return true;
                        }
                        else
                            throw new Exception("error 28 at token:" + index);
                    }

                    if(tokens[i+1].getValue() != ","  )
                        throw new Exception("error 27 at token:" + index);

                    i = i + 2;
                }

                throw new Exception("error 29 at token:" + index);
            }
            throw new Exception("error 30 at token:" + index);
        }

        private ExpressionNode parseSingleExpression(Token[] exprList, LocalScope scope)
        {
            if (exprList[0].isLiteral())
                return new LiteralNode(exprList[0]);

            else if (scope.inScope(exprList[0]))
                return new VariableNode(exprList[0]);

            else throw new Exception("error 17 at token:" + index);
            throw new NotImplementedException();
        }

        private CallNode parseCall(Token[] tokens, LocalScope scope)
        {
            SymbolTable sym = new SymbolTable();

            Token nameToken;

            if(tokens[0].getType() != TokenType.REF )
                throw new Exception("error 33 at token:" + index);

            if(tokens[1].getValue() != "(")
                throw new Exception("error 34 at token:" + index);

            nameToken = tokens[0];
            CallNode funcCall = new CallNode(nameToken);

            int i = 2;
            while (i + 1 < tokens.Length && 
                (tokens[i].getType() == TokenType.REF || tokens[i].isLiteral()))
            {
                if (tokens[i].getType() != TokenType.REF || !tokens[i].isLiteral())
                    throw new Exception("error 35 at token:" + index);

                funcCall.addParam(tokens[i]);

                if (tokens[i + 1].getValue() != "," || tokens[i + 1].getValue() != ")" )
                    throw new Exception("error 36 at token:" + index);

                i = i + 2;
            }
            if (tokens[i - 1].getValue() != ")")
                throw new Exception("error 37 at token:" + index);

            if (!sym.funcInScope(funcCall))
                throw new Exception("error 38 at token:" + index);

            return funcCall;
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
