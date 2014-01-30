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
        private static string[] Op1Tpyes = { "==", "!=", "<=", ">=", ">", "<" };
        private static string[] Op2Tpyes = { "+", "-" };
        private static string[] Op3Tpyes = { "*", "/", "%" };

        private static string[] constructs = { "for", "while", "if" };
        
        private RootNode root;
        private int index,length;
        private Token[] tokens;

        /// <summary>
        /// Entery point.
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public Node parseTokens(Token[] tokens)
        {
            this.tokens = tokens;
            index = 0;
            length=tokens.Length;
            root = new RootNode();


            // make sure the file starts with an opening brace.
            if(tokens[index++].getValue() != "[")
                throw new Exception("error pt1 at token:" + tokens[index].locate());

            // parse functions. still need to add code for gobal vars.
            while (index < length - 1)
            {
                Node function = parseFunction(root);
                root.addChild(function);

            }

            // make sure the file ends with closing brce.
            if (tokens[index++].getValue() != "]")
                throw new Exception("error pt2 at token:" + tokens[index].locate());

            return root;
        }

        /// <summary>
        /// <para> - Corrected </para>
        /// <para> Expects index to point to the return type of the function.</para>
        /// <para> -  </para>
        /// <para> Add the function to the symble table.         </para>
        /// <para> sets the index to the token after the }       </para>
        /// <para> Returns a node containing the function.       </para>
        /// </summary>
        /// <returns></returns>
        private Node parseFunction(RootNode root)
        {
            FunctionNode fn;
            Token returnType;
            Token functionName;
            LinkedList<Token> parameterNames;
            LinkedList<Token> parameterTypes;
            LinkedList<Node> children;

            // TODO make sure this let is a function let.
            
            // make sure the token is an opening brace
            if (tokens[index].getValue() != "[") throw new Exception("error pf1 at token:" + tokens[index].locate());
            index++;

            // make sure the token is a let statement.
            if (tokens[index].getValue() != "let") throw new Exception("error pf2 at token:" + tokens[index].locate());
            index++;

            // make sure the token is an opening brace
            if (tokens[index].getValue() != "[") throw new Exception("error pf3 at token:" + tokens[index].locate());
            index++;

            // make sure the next token is a refernce type that can be used as a name.
            if (tokens[index].getTokenType() != TokenType.REF) throw new Exception("error pf4 at token:" + tokens[index].locate());

            // get the name and construct the function node.
            functionName = tokens[index++];

            // parse the parameters names. This should return with index pointing to the closing brace.
            parameterNames = parseParameterNames();

            // make sure the token is an closing brace
            if (tokens[index].getValue() != "]") throw new Exception("error pf5 at token:" + tokens[index].locate());
            index++;

            // make sure the token is an opening brace
            if (tokens[index].getValue() != "[") throw new Exception("error pf6 at token:" + tokens[index].locate());
            index++;

            // make sure the token is a return type ( i.e. int, bool , ... ) .
            if ( tokens[index].getTokenType() != TokenType.DATATYPE ) throw new Exception("error pf7 at token:" + tokens[index].locate() );
            returnType = tokens[index++];

            // parse the parameters names. This should return with index pointing to the closing brace.
            parameterTypes = parseParameterTypes();

            fn = new FunctionNode(returnType, functionName,parameterNames, parameterTypes);

            // make sure the token is an closing brace
            if (tokens[index].getValue() != "]") throw new Exception("error pf8 at token:" + tokens[index].locate());
            index++;
            
            // TODO : Need to add support for prototyping. if someone prototype this will throw an error.  <========================================
            // Make sure this function doesnt already exist. 
            if (root.funcInScope(fn)) throw new Exception("error pf9 at token:" + tokens[index].locate());
            index++;

            // Add the new function to the root node.
            root.addFunction(fn);

            // parse the internal function statements. provide the function node as a LocalScope object.
            children = parseStatements(fn);

            // add the children to the function node.
            fn.addChildren(children);

            // make sure there is a closing brace.
            if (tokens[index].getValue() != "]") throw new Exception("error 6 at token:" + tokens[index].locate());
            index++;
            
            return fn;
        }

        /// <summary>
        /// <para> - Corrected </para>
        /// <para> Expects index to point to the first parameter type   </para>
        /// <para> - </para>
        /// <para> Sets index to point to the closing brace             </para>
        /// <para> Returns a linked list of the parameter types         </para>
        /// </summary>
        /// <returns></returns>
        private LinkedList<Token> parseParameterTypes()
        {
            LinkedList<Token> types = new LinkedList<Token>();

            // loop until we find the closing brace.
            while (tokens[index].getValue() != "]")
            {
                // make sure the token has refernce type.
                if (tokens[index].getTokenType() != TokenType.DATATYPE) throw new Exception("error ppn1 at token:" + tokens[index].locate());

                types.AddLast(tokens[index]);

                index++;
            }
            return types;
        }

        /// <summary>
        /// <para> - Corrected </para>
        /// <para> Expects index to point to the first parameter name   </para>
        /// <para> - </para>
        /// <para> Sets index to point to the closing brace             </para>
        /// <para> Returns a linked list of the parameter names         </para>
        /// </summary>
        /// <returns></returns>
        private LinkedList<Token> parseParameterNames()
        {
            LinkedList<Token> names = new LinkedList<Token>();

            // loop until we find the closing brace.
            while (tokens[index].getValue() != "]")
            {
                // make sure the token has refernce type.
                if (tokens[index].getTokenType() != TokenType.REF) throw new Exception("error ppn1 at token:" + tokens[index].locate());

                names.AddLast(tokens[index]);

                index++;
            }
            return names;
        }

        /// <summary>
        /// <para> - </para>
        /// <para> Parses all statements inside a scope. (i.e. a function, for loop.)   </para>
        /// <para> Will return once a closing brace ] at its level is reached.          </para>
        /// <para> - </para>
        /// <para> Sets the index TO the closing brace ] .                     </para>
        /// <para> Returns a LinkedList of all statements.                              </para>
        /// </summary>
        private LinkedList<Node> parseStatements(LocalScope scope)
        {
            LinkedList<Node> children = new LinkedList<Node>();

            // loop until we see the closing brace. 
            // nested braces will be taken care of be recursive calls to this function. 
            while (tokens[index].getValue() != "]")
            {

                Node stmt = parseStatement(scope);
                children.AddLast(stmt);
            }

            // after the while loop return the statements.
            return children;
        }

        /// <summary>
        /// <para> - </para>
        /// <para> Parses a single statement.                                       </para>
        /// <para> Expects the index to point to the opening brace                  </para>
        /// <para> - </para>
        /// <para> Sets the index AFTER the closing brace ] .                       </para>
        /// <para> Returns a statement.                                             </para>
        /// </summary>
        private Node parseStatement(LocalScope scope)
        {
            // make sure the token is an opening brace
            if (tokens[index].getValue() != "[") throw new Exception("error ps1 at token:" + tokens[index].locate());
            index++;

            if (tokens[index].getTokenType() == TokenType.ASSIGNMENT)
                return parseAssignment(scope);

            else if (tokens[index].getTokenType() == TokenType.CONSTRUCT)
               return parseConstruct(scope);

            else if (tokens[index].getTokenType() == TokenType.FUNCTION)
                return parseCall(scope);

            else if (tokens[index].getTokenType() == TokenType.REF)
                return parseCall(scope);

            
            // something went wrong if we get here.
            throw new Exception("error ps2 at token:" + tokens[index].locate());
            
        }

        /// <summary>
        /// <para> - </para>
        /// <para> Expects the local scope that it lives in.             </para>
        /// <para> Expects the index to point at the construct token.    </para>
        /// <para> - </para>
        /// <para> Sets the index AFTER the closing brace.               </para>
        /// <para> Returns a construct node.                             </para>
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private Node parseConstruct(LocalScope scope)
        {
            //check the curent token for which kind of construct it is.
            if (tokens[index].getValue() == "while")
                return parseWhileLoop(scope);
            //else if (tokens[index].getValue() == "for")
            //    return parseForLoop(scope);
            else if (tokens[index].getValue() == "if")
                return parseIf(scope);
            else if (tokens[index].getValue() == "let")
                return parseLet (scope);
            else
                throw new Exception("error, unknow construct at token:" + tokens[index].locate() );
        }

        /// <summary>
        /// <para> - </para>
        /// <para> Expects the local scope that it lives in.    </para>
        /// <para> Expects the index to point at the if token.  </para>
        /// <para> - </para>
        /// <para> Sets the index AFTER the closing brace.      </para>
        /// <para> Returns an if node.                          </para>
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private Node parseIf(LocalScope scope)
        {
            IfNode ifNode;
            ExpressionNode eval;
            LinkedList<Node> children;

            // check for the opening of the if statement.
            if (tokens[index].getValue() != "if") throw new Exception("error pi1 at token:" + tokens[index].locate());
            index++;
            
            // parse the evaluation node
            eval = parseExpression( scope);

            // check that the eval node returns a boolean.
            if (eval.getReturnType() != "bool") throw new Exception("error pi4 at token:" + tokens[index].locate() );

            if (tokens[index].getValue() != "[") throw new Exception("error pi6 at token:" + tokens[index].locate());
            

            // create the if node so that the parse statements call will have a local scope to use.
            ifNode = new IfNode(eval, scope);

            // Parse multi statement if 
            if (tokens[index + 1].getValue() == "[")
            {
                index++;
                children = parseStatements(ifNode);

                // check for closing brace on the internal if statements.
                if (tokens[index].getValue() != "]") throw new Exception("error pi7 at token:" + tokens[index].locate());
                index++;
            }
            // Parse single statement if
            else
            {
                children = new LinkedList<Node>();
                children.AddLast(parseStatement(ifNode));
            }

            // add the children.
            ifNode.addChildren(children);

            // check for an else statement and add it to the IfNode if it exists.
            if (tokens[index].getValue() == "[")
            {
                ifNode.addElse(parseElse(scope));

            }

            // check for closing brace for the over all if statement.
            if (tokens[index].getValue() != "]") throw new Exception("error pi9 at token:" + tokens[index].locate());
            index++;

            
            return ifNode;
        }

        /// <summary>
        /// <para> - </para>
        /// <para> Expects the local scope that it lives in.                       </para>
        /// <para> Expects the index to point at the opening brace of the else.    </para>
        /// <para> - </para>
        /// <para> Sets the index AFTER the closing brace.                         </para>
        /// <para> Returns an else node.                                           </para>
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private ElseNode parseElse(LocalScope scope)
        {
            ElseNode elseNode = new ElseNode(scope);
            LinkedList<Node> children;

            // check for the opening of the else statement.
            if (tokens[index].getValue() != "[") throw new Exception("error pel1 at token:" + tokens[index].locate());
            index++;

            // Parse multi statement else 
            if (tokens[index].getValue() == "[")
            {
                children = parseStatements(elseNode);

                // check for closing brace.
                if (tokens[index].getValue() != "]") throw new Exception("error pel2 at token:" + tokens[index].locate());
                index++;
            }
            // Parse single statement if
            else
            {
                children = new LinkedList<Node>();
                children.AddLast(parseStatement(elseNode));
            }

            // add the children to the for loop.
            elseNode.addChildren(children);

          
            return elseNode;
        }

        /// <summary>
        /// <para> - </para>
        /// <para> Expects the local scope that it lives in.            </para>
        /// <para> Expects the index to point at the while token.       </para>
        /// <para> - </para>
        /// <para> Sets the index AFTER the closing brace.              </para>
        /// <para> Returns a while loop node.                           </para>
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private Node parseWhileLoop(LocalScope scope)
        {
            WhileLoopNode wl;
            ExpressionNode eval;
            LinkedList<Node> children;

            // check for the opening of the while statement.
            if (tokens[index].getValue() != "while") throw new Exception("error pw1 at token:" + tokens[index].locate());
            index++;

            // parse the evaluation node
            eval = parseExpression(scope);

            // check that the eval node returns a boolean.
            if (eval.getReturnType() != "bool") throw new Exception("error pw4 at token:" + tokens[index].locate());

            if (tokens[index].getValue() != "[") throw new Exception("error pw6 at token:" + tokens[index].locate());
            

            // create the if node so that the parse statements call will have a local scope to use.
            wl = new WhileLoopNode(eval, scope);

            children = parseStatements(wl);

            // add the children.
            wl.addChildren(children);

            // check for closing brace 
            if (tokens[index].getValue() != "]") throw new Exception("error pw9 at token:" + tokens[index].locate());
            index++;

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
        //private Node parseForLoop(LocalScope scope)
        //{
        //    ForLoopNode fl;
        //    Node assignment;
        //    ExpressionNode eval;
        //    ExpressionNode incrementer;
        //    LinkedList<Node> children;

        //    // check for the opening of the for loop
        //    if (tokens[index].getValue() != "for")throw new Exception("error 31 at token:" + index++);
        //    if (tokens[index].getValue() != "(") throw new Exception("error 32 at token:" + index++);
     
        //    // parse the optional assignment
        //    if (tokens[index].getValue() != ";")
        //    {
        //        assignment = parseAssignment(scope);
        //        index--; // so that we can check the semicolon. parseAssignment() increments index over it...
        //    }else assignment = new EmptyNode();

        //    // make sure we have a semicolon after the optional assignment
        //    if (tokens[index].getValue() != ";") throw new Exception("error 33 at token:" + index++);

        //    // get the evaluation tokens and parse them into a boolean node
        //    Token[] exprList = getTokensToSemicolon();
        //    eval = parseExpression(exprList,scope);

        //    // check that the eval node returns a boolean
        //    if (eval.isEmpty())  throw new Exception("error 35 at token:" + tokens[index].locate() );
        //    if(eval.getReturnType() != "bool" )  throw new Exception("error 34 at token:" + tokens[index].locate() );

        //    // get the optional incrementer expression and then parse it
        //    exprList = getClosingTokens();
        //    incrementer = parseExpression(exprList,scope);

        //    //check that we have the closing brace and openning brace to state the forloop body
        //    if (tokens[index].getValue() != ")") throw new Exception("error 36 at token:" + index++);
        //    if (tokens[index].getValue() != "{") throw new Exception("error 37 at token:" + index++);

        //    // create the forloop node so that the parse statements call will have a local scope to use
        //    fl = new ForLoopNode(assignment,eval,incrementer,scope);

        //    // get the for loop's statments and use it as the local scope
        //    children = parseStatements(fl);

        //    // add the children to the for loop
        //    fl.addChildren(children);

        //    // check for closing brace
        //    if (tokens[index].getValue() != "}") throw new Exception("error 38 at token:" + index++);

        //    return fl;
        //}

        ///// <summary>
        ///// <para> - </para>
        ///// <para> Sets index to point to a closing brace ) .                                               </para>
        ///// <para> Returns a series of zero of more tokens starting at index and finishing before  ){ .     </para>
        ///// </summary>
        ///// <returns></returns>
        //private Token[] getClosingTokens()
        //{
        //    LinkedList<Token> tl = new LinkedList<Token>();

        //    while (tokens[index].getValue() != ")" && tokens[index + 1].getValue() != "{")
        //    {
        //        // if something goes wrong catch it. like there is a token between ){ or if one of them is missing
        //        if (tokens[index].getValue() == "{" || tokens[index].getValue() == "}") throw new Exception("error 39 at token:" + tokens[index].locate() );
        //        tl.AddLast(tokens[index++]);
        //    }

        //    return tl.ToArray();
        //}

        /// <summary>
        /// <para> - </para>
        /// <para> Expects the scope that it lives in.                              </para>
        /// <para> Expects index to point to the :=.                                </para>
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

            // make sure its an assignment
            if (tokens[index].getValue() != ":=") throw new Exception("error pa1 at token:" + tokens[index].locate());
            index++;

            varName = tokens[index++];

            // check that the token is in scope
            if (!scope.varInScope(varName)) throw new Exception("error pa2 at token:" + tokens[index].locate() + "  " + varName.getValue() + " is not in scope");

            // parse the expression into Node(s)
            expr = parseExpression( scope);

            VariableNode varNode = scope.getVarRef(varName);

            // check the that the data types match
            if (expr.getReturnType() != varNode.getReturnType() ) throw new Exception("error pa3 at token:" + tokens[index].locate());


            assignment = new AssignmentNode(varNode, expr);

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
        private Node parseLet(LocalScope scope)
        {
            LetNode let = new LetNode();
            Token dataType;
            Token variableName;
            ExpressionNode expr = null;

            
            // check that its has a valid data type
            if (tokens[index].getTokenType() == TokenType.ASSIGNMENT ) throw new Exception("error pl1 at token:" + tokens[index].locate() );
            index++;

            while (tokens[index].getValue() != "]") { 

                // make sure its a reference token and get its label.
                if (tokens[index].getTokenType() != TokenType.REF) throw new Exception("error pl2 at token:" + tokens[index].locate() );
                variableName = tokens[index++];

                 // check that its has a valid data type
                if (tokens[index].getTokenType() == TokenType.DATATYPE ) throw new Exception("error pl3 at token:" + tokens[index].locate() );
                dataType = tokens[index++];


                // make sure its not in scope.
                if (scope.varInScope(variableName))
                    throw new Exception("error pl4 at Token " + tokens[index -2].locate() + "  " + variableName.getValue() + " is already in scope.");

                if (tokens[index].getValue() != "]") throw new Exception("error pl5 at token:" + tokens[index].locate() );
                index++;

                // construct the declaration node.
                DeclarationNode dec = new DeclarationNode(dataType, variableName, expr);
            
                // add the variable to local scope.
                scope.addToScope(dec);

                let.addChild(dec);
            }
            return let;   
        }

        /// <summary>
        /// <para>  - </para>
        /// <para> Expects the scope that it lives in.                                  </para>
        /// <para> Expects index to point to the Open brace, reference, or Literal      </para>
        /// <para>  - </para>   
        /// <para> Sets index AFTER the brace.                                          </para>
        /// <para> Returns an Expression node.                                          </para>
        /// </summary>
        /// <param name="exprList"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        private ExpressionNode parseExpression(LocalScope scope)
        {
            ExpressionNode expr;

            // check for a composite expression.
            if (tokens[index].getValue() == "[")
            {
                index++;

                if(scope.funcInScope(tokens[index])){
                    expr = parseCall(scope);
                }
                else throw new Exception("error pex1 at token:" + tokens[index].locate());
            }

            // check for a single literal.
            else if(tokens[index].isLiteral()){
                expr = new LiteralNode(tokens[index]);
            }

            // check for a local variable.
            else if (scope.varInScope(tokens[index])){
                expr = scope.getVarRef(tokens[index]);

            }
            else throw new Exception("error pex2 at token:" + tokens[index].locate());

            return expr;
        }


        private CallNode parseCall(LocalScope scope)
        {
            FunctionNode func = scope.getFuncRef(tokens[index]);
            LinkedList<ExpressionNode> parameters = new LinkedList<ExpressionNode>();

            foreach (ParamNode paramLabel in func.getParameters())
            {

                ExpressionNode paramExpr = parseExpression(scope);

                // make sure the param data type matchs the function signature,
                if (paramExpr.getReturnType() != paramLabel.getDataType()) throw new Exception("error pex2 at token:" + tokens[index].locate());

                parameters.AddLast(paramExpr);
            }

            CallNode call = new CallNode(func, parameters);

            return call;
        }

    



    }
}
