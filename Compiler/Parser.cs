using Compiler.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class Parser
    {
        //public const bool parserDebug = false;
        //public const bool symantics = false;
      
        private RootNode root;
        private Tokenizer tok;

        public Node parseT(Tokenizer t)
        {
            tok = t;
            RootNode root = new RootNode();
            debugEntering("parseT");

            // make sure the file starts with an opening brace.
            if (TokenValue != "[")
                throw new Exception("error pT1 at token:" + tok.peep().locate() + "\nToken must be an opening bracket.");
            Pop();
            while(TokenValue !="]"){
                root.addChild( parseS(root));
               
            }

            // make sure the file ends with closing brce.
            if (TokenValue != "]")
                throw new Exception("error pT2 at token:" + tok.peep().locate() + "\nToken must be a closing bracket.");

            debugExit("parseT");
            Pop();

            try
            {
                Pop();
                Console.WriteLine("WARNING, End of file was not reached." + tok.peep().locate());

            }
            catch (EndOfTokensException e) { }
            return root;
        }

        private Node parseS(ILocalScopeNode scope)
        {
            debugEntering("parseS");
            SNode s = new SNode(scope);

           
            while (TokenValue != "]") // parse until we see a closing brace for parent
            {
                if (TokenValue == "[") 
                    // S' has an open brace. can either be another S or an assigment, binop,unop or stmts
                {
                    Pop();           
         
                    // check to see if the open brace belongs to a oper/stmts
                    if (GetTokenType == TokenType.ASSIGNMENT)
                        s.addChild(parseAssignment(scope));
                    else if (GetTokenType == TokenType.CONSTRUCT)
                        s.addChild(parseConstruct(scope));
                    else if (GetTokenType == TokenType.OP)
                        s.addChild(parseOp(scope));
                    else if (GetTokenType == TokenType.REF && scope.funcInScope(TokenValue) )
                        s.addChild(parseCall(scope));

                    else // ok, We now know that the open brace belongs to S
                    {
                        //s.setBraces(); // let S know it has braces.
                        SNode child = (SNode)parseS(s); // and recurs
                        child.setBraces();
                        s.addChild(child);

                        if (TokenValue != "]")
                            throw new Exception("error pS2 at token:" + tok.peep().locate() + "\nToken must be a opening bracket.");

                        Pop();
                    }

                } 
                    // S does not have an openbrace brace. we can assume its wither a refernce or a literal
                else if (GetTokenType == TokenType.REF)
                    s.addChild(parseExpression(scope));
                else if (tok.peep().isLiteral())
                    s.addChild(new LiteralNode(Pop()));
                else
                {
                    throw new Exception("error pS1 ,invalid tokens at " + tok.peep().locate() +"\n Only operation or statements can be in an S production");
                }
                
            } 
            debugExit("parseS");
            return s;
        }

        /// <summary>
        /// Entery point for function only programs.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private Node parseTokens(Tokenizer t)
        {

            this.tok = t;

            debugEntering("begin");


            root = new RootNode();


            // make sure the file starts with an opening brace.
            if(Pop().getValue() != "[")
                throw new Exception("error pt1 at token:" + tok.peep().locate() + "\nToken must be a opening bracket.");

            // parse functions. still need to add code for gobal vars.
            while (TokenValue == "[")
            {
                throw new NotImplementedException();
                //Node function = parseFunction(root);
                //root.addChild(function);

            }

            debugExit("begin");

            // make sure the file ends with closing brce.
            if (Pop().getValue() != "]")
                throw new Exception("error pt2 at token:" + tok.peep().locate() + "\nToken must be a closing bracket.");

            return root;
        }

        /// <summary>
        /// <para> - Corrected </para>
        /// <para> Expects index to point to the return type of the function.</para>
        /// <para> -  </para>
        /// <para> Add the function to the symble table.         </para>
        /// <para> sets the index after the closing brace        </para>
        /// <para> Returns a node containing the function.       </para>
        /// </summary>
        /// <returns></returns>
        private Node parseFunction(ILocalScopeNode scope)
        {


            debugEntering("func");
            Token functionName;
            Token returnType;

            UserFunctionNode fn;
            LinkedList<Token> parameterNames;
            LinkedList<Token> parameterTypes;
            LinkedList<Node> children;

            if (GetTokenType != TokenType.REF) throw new Exception("error pf1 at token:" + tok.peep().locate() + "\nToken must be a reference.");

            functionName = Pop();

            // parse the parameters names. This should return with index pointing to the closing brace.
            parameterNames = parseParameterNames();

            // make sure the token is an closing brace
            if (TokenValue != "]") throw new Exception("error pf5 at token:" + tok.peep().locate() + "\nToken must be a closing bracket.");
            Pop();

            // make sure the token is an opening brace
            if (TokenValue != "[") throw new Exception("error pf6 at token:" + tok.peep().locate() + "\nToken must be a opening bracket.");
            Pop();

            // make sure the token is a return type ( i.e. int, bool , ... ) .
            if (GetTokenType != TokenType.DATATYPE) throw new Exception("error pf7 at token:" + tok.peep().locate() + "\nToken must be a data type.");
            returnType = Pop();;

            // parse the parameters names. This should return with index pointing to the closing brace.
            parameterTypes = parseParameterTypes();

            fn = new UserFunctionNode(returnType, functionName,parameterNames, parameterTypes,scope);

            // make sure the token is an closing brace to close the data type list
            if (TokenValue != "]") throw new Exception("error pf8 at token:" + tok.peep().locate() + "\nToken must be a closing bracket.");
            Pop();
            
            // TODO : Need to add support for prototyping. if someone prototype this will throw an error.  <========================================
            // Make sure this function doesnt already exist. 
            if (scope.funcInScope(functionName.getValue())) throw new Exception("error pf9 at token:" + tok.peep().locate() + "\nFunction is already in scope.");
        
          
            // parse the internal function statements. provide the function node as a LocalScope object.
            children = parseStatements(fn);

            // add the children to the function node.
            fn.addChildren(children);

            // Add the new function to the root node.
            scope.addToScope(fn);

            // make sure there is a closing brace.
            if (TokenValue != "]") throw new Exception("error 6 at token:" + tok.peep().locate() + "\nToken must be a closing bracket.");
            Pop();

            debugExit("func");
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

            debugEntering("param type");

            LinkedList<Token> types = new LinkedList<Token>();

            // loop until we find the closing brace.
            while (TokenValue != "]")
            {
                // make sure the token has refernce type.
                if (GetTokenType != TokenType.DATATYPE) throw new Exception("error ppn1 at token:" + tok.peep().locate() + "\nToken must be a data type.");

                types.AddLast(tok.peep());

                Pop();
            }

            debugExit("param types");
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

            debugEntering("param names");

            LinkedList<Token> names = new LinkedList<Token>();

            // loop until we find the closing brace.
            while (TokenValue != "]")
            {
                // make sure the token has refernce type.
                if (GetTokenType != TokenType.REF) throw new Exception("error ppn1 at token:" + tok.peep().locate() + "\nToken must be a reference.");

                names.AddLast(tok.peep());

                Pop();
            }

            debugExit("param names");
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
        private LinkedList<Node> parseStatements(ILocalScopeNode scope)
        {

            debugEntering("statements");           

            LinkedList<Node> children = new LinkedList<Node>();

            // loop until we see the closing brace. 
            // nested braces will be taken care of be recursive calls to this function. 
            while (TokenValue != "]")
            {

                Node stmt = parseStatement(scope);
                children.AddLast(stmt);
            }


            debugExit("statements");
            // after the while loop return the statements.
            return children;
        }

        /// <summary>
        /// <para> - </para>
        /// <para> Parses a single statement.                                       </para>
        /// <para> Expects the index to point to the first statement                </para>
        /// <para> - </para>
        /// <para> Sets the index AFTER the closing brace ] .                       </para>
        /// <para> Returns a statement.                                             </para>
        /// </summary>
        private Node parseStatement(ILocalScopeNode scope)
        {

            debugEntering("statement");

            var ss = TokenValue;
            // make sure the token is an opening brace
            if ( ss == "[")
            {//throw new Exception("error pst1 at token:" + tok.peep().locate());
                Pop();

                if (GetTokenType == TokenType.ASSIGNMENT)
                    return parseAssignment(scope);

                if (GetTokenType == TokenType.OP)
                    return parseOp(scope);

                else if (GetTokenType == TokenType.CONSTRUCT)
                    return parseConstruct(scope);

                else if (GetTokenType == TokenType.FUNCTION)
                    return parseCall(scope);

                else if (GetTokenType == TokenType.REF)
                    return parseCall(scope);

            }
            else if (tok.peep().isLiteral())
                return parseExpression(scope);
            else if (GetTokenType == TokenType.REF)
                return parseExpression(scope);

            debugExit("statement");
            // something went wrong if we get here.
            throw new Exception("error ps2 at token:" + tok.peep().locate());
            
        }


        //private Node parseBFunc(ILocalScopeNode scope)
        //{
        //    debugEntering("BFunc");

        //    if (TokenValue == "stdout")
        //    {
        //        Pop();
        //        ExpressionNode expr = parseExpression(scope);

        //        if(TokenValue!= "]") throw new Exception("error, expenting a closing brace after stdout call at"+tok.peep().locate());
        //        Pop();

        //        return new StdoutNode(expr);
        //    }
        //    throw new NotImplementedException();

        //    debugExit("bfunc");
        //}

        private ExpressionNode parseOp(ILocalScopeNode scope)
        {
            debugEntering("OP");

            Token opToken = tok.peep();

            Pop();

            ExpressionNode expr1, expr2 = null, expr3 = null;
            expr1 = parseExpression(scope);

            if (TokenValue == "]")
            {
                if (!isUnOp(opToken)) throw new Exception("error, Looking for a unnary operator at " + opToken.locate());
            }
            else
            {
                expr2 = parseExpression(scope);

                if (isTriOp(opToken))
                {
                    expr3 = parseExpression(scope);
                }else if(! isBinOp(opToken)) throw new Exception("error, Looking for a binary operator at " + opToken.locate());
            }

            if (TokenValue != "]")
                throw new Exception("error, parseOp1 at token:" + tok.peep().locate() + "\n expecting a ]\n in exprestion "+opToken.getValue());

            Pop();
            //if (opToken.getValue() == "^") definePowerFunction(scope,expr1,expr2);
           
            debugExit("op");
            return  new OpNode(opToken, expr1, expr2, expr3);
        }

        private void definePowerFunction(ILocalScopeNode scope,ExpressionNode left, ExpressionNode right)
        {
            if (left.getReturnType() == "float" || right.getReturnType() == "float") scope.defineFunc("f^");
            else  scope.defineFunc("^");
        }

        /// <summary>
        /// <para> - </para>
        /// <para> Expects the local scope that it lives in.             </para>
        /// <para> Expects the index to point at the construct token.    </para>
        /// <para> - </para>
        /// <para> Sets the index AFTER the closing brace.               </para>
        /// <para> Returns a construct node.                             </para>
        /// </summary>
        private Node parseConstruct(ILocalScopeNode scope)
        {

            debugEntering("construct");

            //check the curent token for which kind of construct it is.
            if (TokenValue == "while")
            {
                debugExit("construct");
                return parseWhileLoop(scope);
            }
            else if (TokenValue == "stdout")
                return parseStdout(scope, false);
            else if (TokenValue == "stdoutnl")
                return parseStdout(scope, true);

            else if (TokenValue == "if")
            {
                debugExit("construct");
                return parseIf(scope);
            }
            else if (TokenValue == "let" || TokenValue == "let_in" || TokenValue == "let_out")
            {
                debugExit("construct");
                return parseLet(scope);
            }
            else if (TokenValue == "return")
            {
                return parseReturn(scope);
            }
            else
                throw new Exception("error, unknow construct at token:" + tok.peep().locate());
        }

        /// <summary>
        /// <para> - </para>
        /// <para> Expects the local scope that it lives in.        </para>
        /// <para> Expects the index to point at the return token.  </para>
        /// <para> - </para>
        /// <para> Sets the index AFTER the closing brace.          </para>
        /// <para> Returns an returnNode node.                      </para>
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private Node parseReturn(ILocalScopeNode scope)
        {
            Token r = Pop();

            ExpressionNode returnExpr = parseExpression(scope);
            UserFunctionNode func;

            try { func = scope.getParentFunc();
            } catch (Exception e) { throw new Exception(e.Message + r.locate()); }

            if (TokenValue != "]") throw new Exception("error, parsing stdout. expecting closing brace ]. " + tok.peep().locate());
            Pop();

            return new ReturnNode(returnExpr,r,func);
        }
        /// <summary>
        /// <para> - </para>
        /// <para> Expects the local scope that it lives in.    </para>
        /// <para> Expects the index to point at the stdout token.  </para>
        /// <para> - </para>
        /// <para> Sets the index AFTER the closing brace.      </para>
        /// <para> Returns an stdNode node.                          </para>
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private Node parseStdout(ILocalScopeNode scope,bool isln)
        {
            Pop();
            ExpressionNode expr = parseExpression(scope);
            if (TokenValue != "]") throw new Exception("error, parsing stdout. expecting closing brace ]. " + tok.peep().locate());

            Pop();
            return new StdoutNode(expr,isln);
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
        private Node parseIf(ILocalScopeNode scope)
        { 

            debugEntering("if");

            IfNode ifNode;
            ExpressionNode eval;
            LinkedList<Node> children;

            // check for the opening of the if statement.
            if (TokenValue != "if") throw new Exception("error pi1 at token:" + tok.peep().locate() + "\nToken must be an if.");
            Pop();
            
            // parse the evaluation node
            eval = parseExpression( scope);

            // check that the eval node returns a boolean.
            if (eval.getReturnType() != "bool") throw new Exception("error pi4 at token:" + tok.peep().locate() + "\n Return type must be a bool.");

            //if (TokenValue != "[") throw new Exception("error pi6 at token:" + tok.peep().locate());
            

            // create the if node so that the parse statements call will have a local scope to use.
            ifNode = new IfNode(eval, scope);

            //// Parse multi statement if 
            //if (tokens[index + 1].getValue() == "[")
            //{
            //    tokenizer.pop();
            //    children = parseStatements(ifNode);

            //    // check for closing brace on the internal if statements.
            //    if (tokenizer.peep().getValue() != "]") throw new Exception("error pi7 at token:" + tokenizer.peep().locate());
            //    tokenizer.pop();
            //}
            //// Parse single statement if
            //else
            //{
                children = new LinkedList<Node>();
                children.AddLast(parseStatement(ifNode));
            //}

            // add the children.
            ifNode.addChildren(children);

            // check for an else statement and add it to the IfNode if it exists.
            if (TokenValue != "]")
            {
                ifNode.addElse(parseElse(scope));

            }

            // check for closing brace on the overall if statements.
            if (TokenValue != "]") throw new Exception("error pif3 at token:" + tok.peep().locate() + "\nToken must be a closing bracket.");
            Pop();

            debugExit("if");
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
        private ElseNode parseElse(ILocalScopeNode scope)
        {
            debugEntering("else");

            ElseNode elseNode = new ElseNode(scope);
            LinkedList<Node> children;

            // check for the opening of the else statement.
           // if (TokenValue != "[") throw new Exception("error pel1 at token:" + tok.peep().locate());
            

            //// Parse multi statement else 
            //if (tokens[index + 1].getValue() == "[")
            //{
            //    tokenizer.pop();
            //    children = parseStatements(elseNode);

            //    // check for closing brace.
            //    if (tokenizer.peep().getValue() != "]") throw new Exception("error pel2 at token:" + tokenizer.peep().locate());
            //    tokenizer.pop();
            //}
            //// Parse single statement if
            //else
            //{
                children = new LinkedList<Node>();
                children.AddLast(parseStatement(elseNode));
            //}

            // add the children to the for loop.
            elseNode.addChildren(children);


            debugExit("else");
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
        private Node parseWhileLoop(ILocalScopeNode scope)
        { 

            debugEntering("while");

            WhileLoopNode wl;
            ExpressionNode eval;
            LinkedList<Node> children;

            // check for the opening of the while statement.
            if (TokenValue != "while") throw new Exception("error pw1 at token:" + tok.peep().locate() + "\nToken must be a while.");
            Pop();

            // parse the evaluation node
            eval = parseExpression(scope);

            // check that the eval node returns a boolean.
            if (eval.getReturnType() != "bool") throw new Exception("error pw4 at token:" + tok.peep().locate() + "\n Return type must be a bool.");

            
            // create the if node so that the parse statements call will have a local scope to use.
            wl = new WhileLoopNode(eval, scope);

            children = parseStatements(wl);

            // add the children.
            wl.addChildren(children);

            // check for closing brace 
            if (TokenValue != "]") throw new Exception("error pw9 at token:" + tok.peep().locate() + "\nToken must be a closing bracket.");
            Pop();

            debugExit("while");
            return wl;
        }


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
        private AssignmentNode parseAssignment(ILocalScopeNode scope)
        {

            debugEntering("assignment");

            VariableNode varNode=null;
            AssignmentNode assignment;
            ExpressionNode expr;
            Token varName;

            // make sure its an assignment
            if (TokenValue != ":=") throw new Exception("error pa1 at token:" + tok.peep().locate() + "\nToken must be a := .");
            Token loc = Pop();

            varName = Pop();
            if (varName.getTokenType() != TokenType.REF) throw new Exception("LValue must be a variable name. " + varName.locate());
            // check that the token is in scope
            if ( !scope.varInScope(varName.getValue())) throw new Exception("error pa2 at token:" + varName.locate() + "\nVariable is not in scope");

            // parse the expression into Node(s)
            expr = parseExpression( scope);

            varNode = scope.getVarRef(varName.getValue());

            assignment = new AssignmentNode(varNode, expr,loc);

            // make sure its an assignment
            if (TokenValue != "]") throw new Exception("error pa1 at token:" + tok.peep().locate()+@"
After parsing an assignment, an closing brace is expected.");
            Pop();


            debugExit("assign");
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
        private Node  parseLet(ILocalScopeNode scope)
        { 


            debugEntering("let");

            LetNode let = new LetNode();
            Token dataType;
            Token variableName;
            LetType type;

            // check that its has a valid data type
            if (TokenValue == "let")
                type = LetType.Local;
            else if(TokenValue == "let_in")
                type = LetType.Input;
            else if(TokenValue == "let_out")
                type = LetType.Output;
            else throw new Exception("error pl1 at token:" + tok.peep().locate() + "\nToken must be a let.");
            
            Pop();

            // make sure its a open brace token.
            if (TokenValue != "[") throw new Exception("error pl2 at token:" + tok.peep().locate() + "\nToken must be a open bracket.");
            Pop();

            // Check to see if this is a variable declarations. 
            if (TokenValue == "[")
            {
                while (TokenValue == "[")
                {
                    // make sure its a open brace token.
                    if (TokenValue != "[") throw new Exception("error pl4 at token:" + tok.peep().locate() + "\nToken must be a opeb bracket.");
                    Pop();

                    // make sure its a reference token and get its label.
                    if (GetTokenType != TokenType.REF) throw new Exception("error pl2 at token:" + tok.peep().locate()+"\nToken must be a refernce type.");
                    variableName = Pop();

                    // check that its has a valid data type
                    if (GetTokenType != TokenType.DATATYPE) throw new Exception("error pl3 at token:" + tok.peep().locate());
                    dataType = Pop();


                    // make sure its not in scope.
                    if (scope.varInImmediateScope(variableName.getValue()) || 
                         (!Program.cStyleScoping && 
                           scope.varInScope(variableName.getValue())
                         )
                       ) throw new Exception("error pl4 at Token " + variableName.locate() +"\n is already in scope.");

                    if (TokenValue != "]") throw new Exception("error pl5 at token:" + tok.peep().locate() + "\nToken must be a closing bracket.");
                    Pop();

                    // construct the declaration node.
                    DeclarationNode dec = new DeclarationNode(dataType, variableName, type);


                    // add the variable to local scope.
                    scope.addToScope(dec);

                    let.addChild(dec);
                }

                // make sure its a open brace token.
                if (TokenValue != "]") throw new Exception("error pl7 at token:" + tok.peep().locate() + "\nToken must be a closing bracket.");

                Pop();
            }
                // chech to see if it is a function declaration.
            else if (GetTokenType == TokenType.REF )
            {
               parseFunction(scope);
               return new BlankNode();

            }else // if its not a variable or function declatation there must be an error.
                throw new Exception("Token in let stament is incorrect...  "+tok.peep().locate());

            if (TokenValue != "]") throw new Exception("error pl6 at token:" + tok.peep().locate() + "\nToken must be a closing bracket.");
            Pop();

            debugExit("let");
            return let;   
        }

        private TokenType GetTokenType
        {
            get { return tok.peep().getTokenType(); }
        }

        private string TokenValue
        {
            get { return tok.peep().getValue(); }
        }

        private Token Pop()
        {
            return tok.pop();
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
        private ExpressionNode parseExpression(ILocalScopeNode scope)
        {

            debugEntering("expr");

            ExpressionNode expr;

            // check for a composite expression.
            if (TokenValue == "[")
            {
                Pop();
                if (GetTokenType == TokenType.OP)
                    expr =  parseOp(scope);

                //else if (GetTokenType == TokenType.ASSIGNMENT)
                //{
                //    expr = parseAssignment(scope);
                //}
                //else if (GetTokenType == TokenType.CONSTRUCT)
                //{
                //    expr = parseConstruct(scope);
                //}
                else if(scope.funcInScope(TokenValue)){ 
                    expr = parseCall(scope);
                }
                else throw new Exception("error pex1 at token:" + tok.peep().locate()+@"
Unknown express found, looking for something that evaluates to a value.");

                
            }

            // check for a single literal.
            else if(tok.peep().isLiteral()){
                expr = new LiteralNode(tok.peep());
                Pop();
            }

            // check for a local variable.
            else if (scope.varInScope(TokenValue))
            {
                expr = scope.getVarRef(TokenValue); 
                Pop();
            }
            else throw new Exception("error pex2 at token:" + tok.peep().locate()+@"
An unknown token was found. If its a verable, 
make sure it has been initialized and is in scope. 
Otherwise it is probable out of place");


            debugExit("expr");
            return expr;
        }


        private CallNode parseCall(ILocalScopeNode scope)
        {
            debugEntering("call");
            Token functionName =tok.peep();
            if (!scope.funcInScope(functionName.getValue())) 
                throw new Exception("error Function " + TokenValue + " does not exist at " + tok.peep().locate());


            //IFunctionNode func=null;


            if (TokenValue == "stdout" || TokenValue == "stdoutnl")
            {
                throw new NotImplementedException();
            }
            else
            {

            
                UserFunctionNode func = scope.getFuncRef(TokenValue);
           
                List<ExpressionNode> callParams = new List<ExpressionNode>();
            
                Pop();

                //foreach (ParamNode paramLabel in func.getParameters())
                var funcParam =  func.getParameters();
                //LinkedListNode<ParamNode> cur = null;
                int i = 0;

                while(TokenValue != "]")
                {

                    ExpressionNode paramExpr = parseExpression(scope);

                    // make sure the signature has another param
                    if (i == funcParam.Count) throw new Exception("error parsing function call.\n Too many parameters were given.\n" + tok.peep().locate());
                    // make sure the param data type matchs the function signature,
                    if (paramExpr.getReturnType() != funcParam[i].getDataType()) throw new Exception("error pex2 at token:" + tok.peep().locate());

                    i++;
                    callParams.Add(paramExpr);
                }
                if (callParams.Count != funcParam.Count) throw new Exception("error parsing function call.\n Too few parameters were given.\n" + tok.peep().locate());
               

                CallNode call = new CallNode(func, callParams);

                // make sure its has closing brace
                if (TokenValue != "]") throw new Exception("error pcall 1 at token:" + tok.peep().locate() + "\nToken must be a closing bracket.");
                Pop();

                debugExit("call");
                return call;
            }
        }
        private bool isUnOp(Token opToken)
        {
            if (opToken.getValue() != "not" ||
                    opToken.getValue() != "sin" ||
                    opToken.getValue() != "cos" ||
                    opToken.getValue() != "tan" ||
                    opToken.getValue() != "-") return true;
            return false;
        }

        private bool isBinOp(Token opToken)
        {
            if (opToken.getValue() != "+" ||
                    opToken.getValue() != "-" ||
                    opToken.getValue() != "*" ||
                    opToken.getValue() != "/" ||
                    opToken.getValue() != "%" ||
                    opToken.getValue() != "^" ||
                    opToken.getValue() != "=" ||
                    opToken.getValue() != ">" ||
                    opToken.getValue() != ">=" ||
                    opToken.getValue() != "<" ||
                    opToken.getValue() != "<=" ||
                    opToken.getValue() != "!=" ||
                    opToken.getValue() != "$" ||
                    opToken.getValue() != "or" ||
                    opToken.getValue() != "and") return true;
            return false;

        }

        private bool isTriOp(Token opToken)
        {

            if (opToken.getValue() == "@") return true;
            return false;
        }
        
        public void debugEntering(string name)
        {
            if (Program.parserDebug) {
                Console.Write("entering  " + name + " at " );
                Console.WriteLine( tok.peep().locate());
            }
        }
        public void debugExit(string name)
        {
            if (Program.parserDebug) {
                Console.Write("Exiting   " + name + " at " );//+ tokenizer.peep().locate());
                Console.WriteLine( tok.peep().locate());
            }

        }
    }
}
