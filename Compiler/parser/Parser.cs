using Compiler.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class Parser
    {
        // TODO add boolean ops    &&, ||
        private string[] Op1Tpyes = { "==","!=","<=", ">=",">","<"};
        private string[] Op2Tpyes = { "+", "-" };
        private string[] Op3Tpyes = { "*", "/", "%" };

        private string[] dataTypes = { "int", "bool", "real", "string", "void" };
        private string[] constructs = { "for", "while","if"};
        
        private Node root;
        private int index,length;
        private Token[] tokens;


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

            return root;
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

            // get the SINGLETON global symbol table.
            SymbolTable sym = new SymbolTable();

            // make sure the token is a return type ( i.e. int, void , ... ) .
            if (!ofType(tokens[index], dataTypes)) throw new Exception("error 1 at token:" + index);

            returnType = tokens[index++];

            // make sure the next token is a refernce type that can be used as a name.
            if (tokens[index].getType() != TokenType.REF)  throw new Exception("error 2 at token:" + index);

            // get the name and construct the function node.
            functionName = tokens[index++];
            fn = new FunctionNode(returnType, functionName);

            // make sure there is an open brace before the parameters are given.
            if (tokens[index].getValue() != "(" )  throw new Exception("error 3 at token:" + index);
            
            // parse the parameters. This should return with index pointing to the closing brace.
            parameters = parseParameter();

            // add parameters to the function node.
            fn.addParameters(parameters);

            // make sure there are proper closing and opening braces.
            if (tokens[index].getValue() != ")") throw new Exception("error 5.1 at token:" + index++);
            if (tokens[index].getValue() != "{") throw new Exception("error 5.2 at token:" + index++);

            // TODO : Need to add support for prototyping. if someone prototype this will throw an error.  <========================================
            // Make sure this function doesnt already exist. 
            if (sym.funcInScope(fn)) throw new Exception("error 58 at token:" + index++);

            // Add the new function to the global symbol table.
            sym.addFunction(fn);

            // parse the internal function statements. provide the function node as a LocalScope object.
            children = parseStatements(fn);

            // add the children to the function node.
            fn.addChildren(children);

            // make sure there is a closing brace.
            if (tokens[index].getValue() != "}") throw new Exception("error 6 at token:" + index++ );
            
            return fn;
        }

        /// <summary>
        /// Usered to parse the paramiters out of a funtion declaration. 
        /// NOT to be misstaken with a function call. 
        /// Expects index to point at the opening brace ( .
        /// 
        /// Sets index to point to the closing brace ) .
        /// returns a linked list of ParamNodes.
        /// </summary>
        /// <returns></returns>
        private LinkedList<ParamNode> parseParameter()
        {
            LinkedList<ParamNode> paramList = new LinkedList<ParamNode>();

            // make sure that index is pointing to the opening brace.
            if (tokens[index].getValue() != "(") throw new Exception("error 12 at token:" + index++);

            // loop until we have no mpre parameters.
            while (tokens[index].getValue() != ")")
            {
                // check that there is a valid data type for the parameter.
                if (!ofType(tokens[index], dataTypes)) throw new Exception("error 13 at token:" + index);
                Token paramType = tokens[index++];

                // check that the parameter has a name.
                if (tokens[index].getType() != TokenType.REF) throw new Exception("error 14 at token:" + index);
                Token paramName = tokens[index++];

                // add the paramter to the parameter list.
                paramList.AddLast(new ParamNode(paramType, paramName));

                // make sure we have either a closing brace 
                if (tokens[index].getValue() != "," || tokens[index].getValue() != ")")
                    throw new Exception("error 14.1 at token:" + index);

                // if there is a comma then increment the pointer to the next token. 
                if (tokens[index++].getValue() == ",") index++;
            }

            return paramList;
        }

        /// <summary>
        /// Parses all statements inside a scope. (i.e. a function, for loop.) 
        /// Will return once a closing brace '}' at its level is reached.
        /// 
        /// Sets the index to point to the closing brace } .
        /// Returns a LinkedList of all statements.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns> a linked list of statements.</returns>
        private LinkedList<Node> parseStatements(LocalScope scope)
        {
            LinkedList<Node> children = new LinkedList<Node>();

            // loop until we see the closing brace. 
            // nested braces will be taken care of be recursive calls to this function. 
            while (tokens[index].getValue() != "}")
            {
                // if we see a data type then assume its a declaration statement.
                if (ofType(tokens[index], dataTypes)) 
                    children.AddLast(parseDeclaration(scope));

                // some sort of reference. either local var or function call.
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
                    // something went wrong if we get here. 
                    else throw new Exception("error 7 at token:" + index);
                }

                // Parse construct.   ex: for(;;)
                else if(ofType(tokens[index],constructs)){
                    children.AddLast(parseConstruct(scope) );
                }
                else
                {
                    // something went wrong if we get here.
                    throw new Exception("error 8 at token:" + index);
                }
                    
            }

            // after the while loop return the statements.
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
            //check the curent token for which kind of construct it is.
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
        /// Expects the index to point at the if token.
        /// 
        /// Sets the index after the closing brace.
        /// Returns a if node.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private Node parseIf(LocalScope scope)
        {
            IfNode ifNode;
            ExpressionNode eval;
            LinkedList<Node> children;

            // check for the opening of the if statement.
            if (tokens[index].getValue() != "if") throw new Exception("error 48 at token:" + index++);
            if (tokens[index].getValue() != "(") throw new Exception("error 49 at token:" + index++);

            // get the evaluation tokens and parse them into a boolean node.
            Token[] exprList = getTokensToSemicolon();
            eval = parseExpression(exprList, scope);

            // check that the eval node returns a boolean.
            if (eval.isEmpty()) throw new Exception("error 50 at token:" + index);
            if (eval.getReturnType() != "bool") throw new Exception("error 51 at token:" + index);

            //check that we have the closing brace and openning brace to state the forloop body.
            if (tokens[index].getValue() != ")") throw new Exception("error 52 at token:" + index++);
            if (tokens[index].getValue() != "{") throw new Exception("error 53 at token:" + index++);

            // create the forloop node so that the parse statements call will have a local scope to use.
            ifNode = new IfNode(eval, scope);

            // get the for loop's statments and use it as the local scope.
            children = parseStatements(ifNode);

            // add the children to the for loop.
            ifNode.addChildren(children);

            // check for closing brace.
            if (tokens[index].getValue() != "}") throw new Exception("error 54 at token:" + index++);

            // check for an else statement and add it to the IfNode if it exists.
            if (tokens[index].getValue() == "else")
                ifNode.addElse(parseElse(scope));

            return ifNode;
        }

        /// <summary>
        /// Expects the local scope that it lives in. 
        /// Expects the index to point at the else token.
        /// 
        /// Sets the index after the closing brace.
        /// Returns an else node.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private ElseNode parseElse(LocalScope scope)
        {
            ElseNode elseNode = new ElseNode(scope);
            LinkedList<Node> children;

            // check for the opening of the else statement.
            if (tokens[index].getValue() != "else") throw new Exception("error 55 at token:" + index++);
            if (tokens[index].getValue() != "{") throw new Exception("error 56 at token:" + index++);

            // get the for loop's statments and use it as the local scope.
            children = parseStatements(elseNode);

            // add the children to the for loop.
            elseNode.addChildren(children);
            
            // check for closing brace.
            if (tokens[index].getValue() != "}") throw new Exception("error 57 at token:" + index++);

            return elseNode;
        }

        /// <summary>
        /// <para> - </para>
        /// <para> Expects the local scope that it lives in.            </para>
        /// <para> Expects the index to point at the while token.       </para>
        /// <para> - </para>
        /// <para> Sets the index after the closing brace.              </para>
        /// <para> Returns a while loop node.                           </para>
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private Node parseWhileLoop(LocalScope scope)
        {
            WhileLoopNode wl;
            ExpressionNode eval;
            LinkedList<Node> children;

            // check for the opening of the for loop
            if (tokens[index].getValue() != "while") throw new Exception("error 41 at token:" + index++);
            if (tokens[index].getValue() != "(") throw new Exception("error 42 at token:" + index++);

            // get the evaluation tokens and parse them into a boolean node
            Token[] exprList = getTokensToSemicolon();
            eval = parseExpression(exprList, scope);

            // check that the eval node returns a boolean
            if (eval.isEmpty()) throw new Exception("error 43 at token:" + index);
            if (eval.getReturnType() != "bool") throw new Exception("error 44 at token:" + index);

            //check that we have the closing brace and openning brace to state the forloop body
            if (tokens[index].getValue() != ")") throw new Exception("error 45 at token:" + index++);
            if (tokens[index].getValue() != "{") throw new Exception("error 46 at token:" + index++);

            // create the forloop node so that the parse statements call will have a local scope to use
            wl = new WhileLoopNode( eval, scope);

            // get the for loop's statments and use it as the local scope
            children = parseStatements(wl);

            // add the children to the for loop
            wl.addChildren(children);

            // check for closing brace
            if (tokens[index].getValue() != "}") throw new Exception("error 47 at token:" + index++);

            return wl;
        }

        /// <summary>
        /// <para> - </para>
        /// <para> Expects the local scope that it lives in.        </para>    
        /// <para> Expects the index to point at the for token.     </para>
        /// <para> - </para>
        /// <para> Sets the index after the closing brace.          </para>
        /// <para> Returns a for loop node.                         </para>
        /// </summary>
        /// <param name="scope"></param>
        private Node parseForLoop(LocalScope scope)
        {
            ForLoopNode fl;
            Node assignment;
            ExpressionNode eval;
            ExpressionNode incrementer;
            LinkedList<Node> children;

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
            eval = parseExpression(exprList,scope);

            // check that the eval node returns a boolean
            if (eval.isEmpty())  throw new Exception("error 35 at token:" + index);
            if(eval.getReturnType() != "bool" )  throw new Exception("error 34 at token:" + index);

            // get the optional incrementer expression and then parse it
            exprList = getClosingTokens();
            incrementer = parseExpression(exprList,scope);

            //check that we have the closing brace and openning brace to state the forloop body
            if (tokens[index].getValue() != ")") throw new Exception("error 36 at token:" + index++);
            if (tokens[index].getValue() != "{") throw new Exception("error 37 at token:" + index++);

            // create the forloop node so that the parse statements call will have a local scope to use
            fl = new ForLoopNode(assignment,eval,incrementer,scope);

            // get the for loop's statments and use it as the local scope
            children = parseStatements(fl);

            // add the children to the for loop
            fl.addChildren(children);

            // check for closing brace
            if (tokens[index].getValue() != "}") throw new Exception("error 38 at token:" + index++);

            return fl;
        }

        /// <summary>
        /// <para> - </para>
        /// <para> Sets index to point to a closing brace ) .                                               </para>
        /// <para> Returns a series of zero of more tokens starting at index and finishing before  ){ .     </para>
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
        /// <para> - </para>
        /// <para> Expects the scope that it lives in.                              </para>
        /// <para> Expects index to point to the lValue.                            </para>
        /// <para> - </para>
        /// <para> Sets index to point to the node after the closing brace { .      </para>
        /// <para> Returns an assingment node.                                      </para>
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
        /// <para> - </para>
        /// <para> Expects the scope that it lives in.                              </para>
        /// <para> Expects index to point to the variable's type.                   </para>
        /// <para> Can parse declarations with assignments.                         </para>
        /// <para> - </para>
        /// <para> Sets index to point to the node after the closing brace { .      </para>
        /// <para> Returns an assingment node.                                      </para>
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

                // parse the expression into a Node.
                expr = parseExpression(exprList , scope);

                // checks to see if the expression is empty.
                if (expr.isEmpty() ) throw new Exception("error 40 at token:" + index);
            }
            // make sure that it terminates with a semicolon.
            else if (tokens[index].getValue() != ";") throw new Exception("error 11 at token:" + index);

            // construct the declaration node.
            dec = new DeclarationNode(dataType, variableName, expr);

            // add the variable to local scope.
            scope.addToScope(dec);
            return dec;
        }

        /// <summary>
        /// <para> - </para>
        /// </para> Sets index to point at a ; .                                        </para>
        /// </para> Returns all nodes until it sees a semicolon.                        </para>
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
        /// <para>  - </para>
        /// <para> Expects the scope that it lives in.                                  </para>
        /// <para> Expects an array of the whole expression (i.e.  3*4+foo()  ) .       </para>
        /// <para>  - </para>   
        /// <para> Sets index to point to the node after the closing brace { .          </para>
        /// <para> Returns an possible EMPTY Expression node.                           </para>
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

        /// <summary>
        /// <para>  - </para>
        /// <para> !!!!  Currently &amp;&amp; and || are not supported.   !!!!              </para>
        /// <para>  - </para>
        /// <para> This function will split an expression into two sub expression.          </para>
        /// <para> It will first look for a boolean evaluation op like == or &lt;.          </para>
        /// <para> Then it will look for a + or - to split the expression on.               </para>
        /// <para> Finally it will look for a * , / , or % to split the expression on.      </para>
        /// <para> It will throw an error if the expression does not have anything to split.</para>
        /// <para> It will also recurrsively split the statement until it has all terminals like literals, variables, and function calls.  </para>
        /// <para>  - </para>
        /// <para> Returns the root Operation node that has a expression tree below it.     </para>
        /// </summary>
        /// <param name="exprList">Expects an array containg the expression to be split.</param>
        /// <param name="scope"></param>
        /// <returns></returns>
        private OperationNode splitExpression(Token[] exprList, LocalScope scope)
        {
            // make sure we arent trying to parse a single expression.
            if (isSingleExpression(exprList,scope)) throw new Exception("error 19 at token:" + index);
            
            // make sure the left most part of the expression is a value.
            if(exprList[0].getType() != TokenType.REF || ! exprList[0].isLiteral())
                throw new Exception("error 18 at token:" + index); 

            int  i = 1;

            // look for a == , <= ,>= , < , > to split the expression on   :  Op1Types
            while (i < exprList.Length && !ofType(exprList[i], Op1Tpyes)) i++;
            if (i < exprList.Length) // we found an == or something equivalent
            {
                return parseOp1Expression(i,exprList,scope);
            }

            i = 1;
            // look for a +, - to split the expression on                 :  Op2Types
            while (i < exprList.Length && !ofType(exprList[i], Op2Tpyes)) i++;
            if (i < exprList.Length) // we found an + or -
            {
                return parseOp2Expression(i, exprList, scope);
            } 
            
            i = 1;
            // look for a * , / , % to split the expression on             :  Op3Types
            while (i < exprList.Length && !ofType(exprList[i], Op3Tpyes)) i++;
            if (i < exprList.Length) // we found an  * , / , %
            {
                return parseOp3Expression(i, exprList, scope);
            }


            // shouldnt get here 
            throw new Exception("error 23 at token:" + index); 
        }

        /// <summary>
        /// <para> - </para>
        /// <para> Expects i to point to the index of the * , / , or % operator. </para>
        /// <para> Expects a array of takens that represent the expression. </para>
        /// <para> Expects the scope that it lives in. </para>
        /// <para> - </para>
        /// <para> returns an Operation Node that is centered on the token located at i. </para>
        /// </summary>
        /// <param name="i"></param>
        /// <param name="exprList"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        private OperationNode parseOp3Expression(int i, Token[] exprList, LocalScope scope)
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

        /// <summary>
        /// <para> - </para>
        /// <para> Expects i to point to the index of the + or - operator. </para>
        /// <para> Expects a array of takens that represent the expression. </para>
        /// <para> Expects the scope that it lives in. </para>
        /// <para> - </para>
        /// <para> returns an Operation Node that is centered on the token located at i. </para>
        /// </summary>
        /// <param name="i"></param>
        /// <param name="exprList"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        private OperationNode parseOp2Expression(int i, Token[] exprList, LocalScope scope)
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

        /// <summary>
        /// <para> - </para>
        /// <para> Expects i to point to the index of a boolean op like == , != , or &lt; . </para>
        /// <para> Expects a array of takens that represent the expression. </para>
        /// <para> Expects the scope that it lives in. </para>
        /// <para> - </para>
        /// <para> returns an Operation Node that is centered on the token located at i. </para>
        /// </summary>
        /// <param name="i"></param>
        /// <param name="exprList"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        private OperationNode parseOp1Expression(int i,Token[] exprList, LocalScope scope)
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
