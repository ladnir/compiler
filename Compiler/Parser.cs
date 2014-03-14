﻿using Compiler.parser;
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
            if (tok.peak().getValue() != "[")
                throw new Exception("error pT1 at token:" + tok.peak().locate() + "\nToken must be an opening bracket.");
            tok.pop();
            while(tok.peak().getValue() !="]"){
                root.addChild( parseS(root));
               
            }

            // make sure the file ends with closing brce.
            if (tok.peak().getValue() != "]")
                throw new Exception("error pT2 at token:" + tok.peak().locate() + "\nToken must be a closing bracket.");

            debugExit("parseT");
            tok.pop();

            try
            {
                tok.pop();
                Console.WriteLine("WARNING, End of file was not reached." + tok.peak().locate());

            }
            catch (EndOfTokensException e) { }
            return root;
        }

        private Node parseS(ILocalScopeNode scope)
        {
            debugEntering("parseS");
            SNode s = new SNode(scope);

           
            while (tok.peak().getValue() != "]") // parse until we see a closing brace for parent
            {
                if (tok.peak().getValue() == "[") 
                    // S' has an open brace. can either be another S or an assigment, binop,unop or stmts
                {
                    tok.pop();           
         
                    // check to see if the open brace belongs to a oper/stmts
                    if (tok.peak().getTokenType() == TokenType.ASSIGNMENT)
                        s.addChild(parseAssignment(scope));
                    else if (tok.peak().getTokenType() == TokenType.CONSTRUCT)
                        s.addChild(parseConstruct(scope));
                    else if (tok.peak().getTokenType() == TokenType.OP)
                        s.addChild(parseOp(scope));
                    else if (tok.peak().getTokenType() == TokenType.REF && scope.funcInScope(tok.peak().getValue()) )
                        s.addChild(parseCall(scope));

                    else // ok, We now know that the open brace belongs to S
                    {
                        //s.setBraces(); // let S know it has braces.
                        SNode child = (SNode)parseS(s); // and recurs
                        child.setBraces();
                        s.addChild(child);

                        if (tok.peak().getValue() != "]")
                            throw new Exception("error pS2 at token:" + tok.peak().locate() + "\nToken must be a opening bracket.");

                        tok.pop();
                    }

                } 
                    // S does not have an openbrace brace. we can assume its wither a refernce or a literal
                else if (tok.peak().getTokenType() == TokenType.REF)
                    s.addChild(parseExpression(scope));
                else if (tok.peak().isLiteral())
                    s.addChild(new LiteralNode(tok.pop()));
                else
                {
                    throw new Exception("error pS1 ,invalid tokens at " + tok.peak().locate() +"\n Only operation or statements can be in an S production");
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
            if(tok.pop().getValue() != "[")
                throw new Exception("error pt1 at token:" + tok.peak().locate() + "\nToken must be a opening bracket.");

            // parse functions. still need to add code for gobal vars.
            while (tok.peak().getValue() == "[")
            {
                throw new NotImplementedException();
                //Node function = parseFunction(root);
                //root.addChild(function);

            }

            debugExit("begin");

            // make sure the file ends with closing brce.
            if (tok.pop().getValue() != "]")
                throw new Exception("error pt2 at token:" + tok.peak().locate() + "\nToken must be a closing bracket.");

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

            if (tok.peak().getTokenType() != TokenType.REF) throw new Exception("error pf1 at token:" + tok.peak().locate() + "\nToken must be a reference.");

            functionName = tok.pop();

            // parse the parameters names. This should return with index pointing to the closing brace.
            parameterNames = parseParameterNames();

            // make sure the token is an closing brace
            if (tok.peak().getValue() != "]") throw new Exception("error pf5 at token:" + tok.peak().locate() + "\nToken must be a closing bracket.");
            tok.pop();

            // make sure the token is an opening brace
            if (tok.peak().getValue() != "[") throw new Exception("error pf6 at token:" + tok.peak().locate() + "\nToken must be a opening bracket.");
            tok.pop();

            // make sure the token is a return type ( i.e. int, bool , ... ) .
            if (tok.peak().getTokenType() != TokenType.DATATYPE) throw new Exception("error pf7 at token:" + tok.peak().locate() + "\nToken must be a data type.");
            returnType = tok.pop();;

            // parse the parameters names. This should return with index pointing to the closing brace.
            parameterTypes = parseParameterTypes();

            fn = new UserFunctionNode(returnType, functionName,parameterNames, parameterTypes,scope);

            // make sure the token is an closing brace to close the data type list
            if (tok.peak().getValue() != "]") throw new Exception("error pf8 at token:" + tok.peak().locate() + "\nToken must be a closing bracket.");
            tok.pop();
            
            // TODO : Need to add support for prototyping. if someone prototype this will throw an error.  <========================================
            // Make sure this function doesnt already exist. 
            if (scope.funcInScope(functionName.getValue())) throw new Exception("error pf9 at token:" + tok.peak().locate() + "\nFunction is already in scope.");
        
          
            // parse the internal function statements. provide the function node as a LocalScope object.
            children = parseStatements(fn);

            // add the children to the function node.
            fn.addChildren(children);

            // Add the new function to the root node.
            scope.addToScope(fn);

            // make sure there is a closing brace.
            if (tok.peak().getValue() != "]") throw new Exception("error 6 at token:" + tok.peak().locate() + "\nToken must be a closing bracket.");
            tok.pop();

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
            while (tok.peak().getValue() != "]")
            {
                // make sure the token has refernce type.
                if (tok.peak().getTokenType() != TokenType.DATATYPE) throw new Exception("error ppn1 at token:" + tok.peak().locate() + "\nToken must be a data type.");

                types.AddLast(tok.peak());

                tok.pop();
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
            while (tok.peak().getValue() != "]")
            {
                // make sure the token has refernce type.
                if (tok.peak().getTokenType() != TokenType.REF) throw new Exception("error ppn1 at token:" + tok.peak().locate() + "\nToken must be a reference.");

                names.AddLast(tok.peak());

                tok.pop();
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
            while (tok.peak().getValue() != "]")
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


            // make sure the token is an opening brace
            if (tok.peak().getValue() == "[")
            {//throw new Exception("error pst1 at token:" + tok.peep().locate());
                tok.pop();

                if (tok.peak().getTokenType() == TokenType.ASSIGNMENT)
                    return parseAssignment(scope);

                if (tok.peak().getTokenType() == TokenType.OP)
                    return parseOp(scope);

                else if (tok.peak().getTokenType() == TokenType.CONSTRUCT)
                    return parseConstruct(scope);

                else if (tok.peak().getTokenType() == TokenType.FUNCTION)
                    return parseCall(scope);

                else if (tok.peak().getTokenType() == TokenType.REF)
                    return parseCall(scope);

            }
            else if (tok.peak().isLiteral())
                return parseExpression(scope);
            else if (tok.peak().getTokenType() == TokenType.REF)
                return parseExpression(scope);

            debugExit("statement");
            // something went wrong if we get here.
            throw new Exception("error ps2 at token:" + tok.peak().locate());
            
        }


        //private Node parseBFunc(ILocalScopeNode scope)
        //{
        //    debugEntering("BFunc");

        //    if (tok.peep().getValue() == "stdout")
        //    {
        //        tok.pop();
        //        ExpressionNode expr = parseExpression(scope);

        //        if(tok.peep().getValue()!= "]") throw new Exception("error, expenting a closing brace after stdout call at"+tok.peep().locate());
        //        tok.pop();

        //        return new StdoutNode(expr);
        //    }
        //    throw new NotImplementedException();

        //    debugExit("bfunc");
        //}

        private ExpressionNode parseOp(ILocalScopeNode scope)
        {
            debugEntering("OP");

            Token opToken = tok.peak();

            tok.pop();

            ExpressionNode leftExpr, rightExpr = null;
            leftExpr = parseExpression(scope);

            if (tok.peak().getValue() != "]")
            {
                if (!isBinOp(opToken) )throw new Exception("error, Looking for a binary operator at " + opToken.locate());
                rightExpr = parseExpression(scope);

            }
            else if (! isUnOp(opToken))  throw new Exception("error, Looking for a unnary operator at " + opToken.locate());

            if (tok.peak().getValue() != "]")
                throw new Exception("error, parseOp1 at token:" + tok.peak().locate() + "\n expecting a ]\n in exprestion "+opToken.getValue());

            tok.pop();
            if (opToken.getValue() == "^") definePowerFunction(scope,leftExpr,rightExpr);
           
            debugExit("op");
            return  new OpNode(opToken, leftExpr, rightExpr);
        }

        private void definePowerFunction(ILocalScopeNode scope,ExpressionNode left, ExpressionNode right)
        {
            if (left.getReturnType() == "float" || right.getReturnType() == "float") scope.defineFunc("f^");
            else  scope.defineFunc("f^");
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
            if (tok.peak().getValue() == "while")
            {
                debugExit("construct");
                return parseWhileLoop(scope);
            }
            else if (tok.peak().getValue() == "stdout")
                return parseStdout(scope, false);
            else if (tok.peak().getValue() == "stdoutnl")
                return parseStdout(scope, true);

            else if (tok.peak().getValue() == "if")
            {
                debugExit("construct");
                return parseIf(scope);
            }
            else if (tok.peak().getValue() == "let")
            {
                debugExit("construct");
                return parseLet(scope);
            }
            else if (tok.peak().getValue() == "return")
            {
                return parseReturn(scope);
            }
            else
                throw new Exception("error, unknow construct at token:" + tok.peak().locate());
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
            Token r = tok.pop();

            ExpressionNode returnExpr = parseExpression(scope);
            UserFunctionNode func;

            try { func = scope.getParentFunc();
            } catch (Exception e) { throw new Exception(e.Message + r.locate()); }

            if (tok.peak().getValue() != "]") throw new Exception("error, parsing stdout. expecting closing brace ]. " + tok.peak().locate());
            tok.pop();

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
            tok.pop();
            ExpressionNode expr = parseExpression(scope);
            if (tok.peak().getValue() != "]") throw new Exception("error, parsing stdout. expecting closing brace ]. " + tok.peak().locate());

            tok.pop();
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
            if (tok.peak().getValue() != "if") throw new Exception("error pi1 at token:" + tok.peak().locate() + "\nToken must be an if.");
            tok.pop();
            
            // parse the evaluation node
            eval = parseExpression( scope);

            // check that the eval node returns a boolean.
            if (eval.getReturnType() != "bool") throw new Exception("error pi4 at token:" + tok.peak().locate() + "\n Return type must be a bool.");

            //if (tok.peep().getValue() != "[") throw new Exception("error pi6 at token:" + tok.peep().locate());
            

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
            if (tok.peak().getValue() != "]")
            {
                ifNode.addElse(parseElse(scope));

            }

            // check for closing brace on the overall if statements.
            if (tok.peak().getValue() != "]") throw new Exception("error pif3 at token:" + tok.peak().locate() + "\nToken must be a closing bracket.");
            tok.pop();

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
           // if (tok.peep().getValue() != "[") throw new Exception("error pel1 at token:" + tok.peep().locate());
            

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
            if (tok.peak().getValue() != "while") throw new Exception("error pw1 at token:" + tok.peak().locate() + "\nToken must be a while.");
            tok.pop();

            // parse the evaluation node
            eval = parseExpression(scope);

            // check that the eval node returns a boolean.
            if (eval.getReturnType() != "bool") throw new Exception("error pw4 at token:" + tok.peak().locate() + "\n Return type must be a bool.");

            
            // create the if node so that the parse statements call will have a local scope to use.
            wl = new WhileLoopNode(eval, scope);

            children = parseStatements(wl);

            // add the children.
            wl.addChildren(children);

            // check for closing brace 
            if (tok.peak().getValue() != "]") throw new Exception("error pw9 at token:" + tok.peak().locate() + "\nToken must be a closing bracket.");
            tok.pop();

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
            if (tok.peak().getValue() != ":=") throw new Exception("error pa1 at token:" + tok.peak().locate() + "\nToken must be a := .");
            Token loc = tok.pop();

            varName = tok.pop();
            if (varName.getTokenType() != TokenType.REF) throw new Exception("LValue must be a variable name. " + varName.locate());
            // check that the token is in scope
            if ( !scope.varInScope(varName.getValue())) throw new Exception("error pa2 at token:" + varName.locate() + "\nVariable is not in scope");

            // parse the expression into Node(s)
            expr = parseExpression( scope);

            //if (scope.varInScope(varName.getValue()))
            //{  //TODO REMOVE THIS ############################################################################################################ 
            varNode = scope.getVarRef(varName.getValue());
            //}
            //else
            //{  //TODO REMOVE THIS ############################################################################################################ 
            //    DeclarationNode dec = new DeclarationNode(null, varName);
            //    varNode = new VariableNode(dec);
            //}

            //// check the that the data types match
            //if ( expr.getReturnType() != varNode.getReturnType()) throw new Exception("error pa3 at token:" + tok.peep().locate()+"\nTypes dont match");

            assignment = new AssignmentNode(varNode, expr,loc);

            // make sure its an assignment
            if (tok.peak().getValue() != "]") throw new Exception("error pa1 at token:" + tok.peak().locate()+@"
After parsing an assignment, an closing brace is expected.");
            tok.pop();


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

            
            // check that its has a valid data type
            if (tok.peak().getValue() != "let") throw new Exception("error pl1 at token:" + tok.peak().locate() + "\nToken must be a let.");
            tok.pop();

            // make sure its a open brace token.
            if (tok.peak().getValue() != "[") throw new Exception("error pl2 at token:" + tok.peak().locate() + "\nToken must be a open bracket.");
            tok.pop();

            // Check to see if this is a variable declarations. 
            if (tok.peak().getValue() == "[")
            {
                while (tok.peak().getValue() == "[")
                {
                    // make sure its a open brace token.
                    if (tok.peak().getValue() != "[") throw new Exception("error pl4 at token:" + tok.peak().locate() + "\nToken must be a opeb bracket.");
                    tok.pop();

                    // make sure its a reference token and get its label.
                    if (tok.peak().getTokenType() != TokenType.REF) throw new Exception("error pl2 at token:" + tok.peak().locate()+"\nToken must be a refernce type.");
                    variableName = tok.pop();

                    // check that its has a valid data type
                    if (tok.peak().getTokenType() != TokenType.DATATYPE) throw new Exception("error pl3 at token:" + tok.peak().locate());
                    dataType = tok.pop();


                    // make sure its not in scope.
                    if (scope.varInImmediateScope(variableName.getValue()) || 
                         (!Program.cStyleScoping && 
                           scope.varInScope(variableName.getValue())
                         )
                       ) throw new Exception("error pl4 at Token " + variableName.locate() +"\n is already in scope.");

                    if (tok.peak().getValue() != "]") throw new Exception("error pl5 at token:" + tok.peak().locate() + "\nToken must be a closing bracket.");
                    tok.pop();

                    // construct the declaration node.
                    DeclarationNode dec = new DeclarationNode(dataType, variableName);

                    // add the variable to local scope.
                    scope.addToScope(dec);

                    let.addChild(dec);
                }

                // make sure its a open brace token.
                if (tok.peak().getValue() != "]") throw new Exception("error pl7 at token:" + tok.peak().locate() + "\nToken must be a closing bracket.");

                tok.pop();
            }
                // chech to see if it is a function declaration.
            else if (tok.peak().getTokenType() == TokenType.REF )
            {
               parseFunction(scope);
               return new BlankNode();

            }else // if its not a variable or function declatation there must be an error.
                throw new Exception("Token in let stament is incorrect...  "+tok.peak().locate());

            if (tok.peak().getValue() != "]") throw new Exception("error pl6 at token:" + tok.peak().locate() + "\nToken must be a closing bracket.");
            tok.pop();

            debugExit("let");
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
        private ExpressionNode parseExpression(ILocalScopeNode scope)
        {

            debugEntering("expr");

            ExpressionNode expr;

            // check for a composite expression.
            if (tok.peak().getValue() == "[")
            {
                tok.pop();
                if (tok.peak().getTokenType() == TokenType.OP)
                    expr =  parseOp(scope);

                //else if (tok.peep().getTokenType() == TokenType.ASSIGNMENT)
                //{
                //    expr = parseAssignment(scope);
                //}
                //else if (tok.peep().getTokenType() == TokenType.CONSTRUCT)
                //{
                //    expr = parseConstruct(scope);
                //}
                else if(scope.funcInScope(tok.peak().getValue())){ 
                    expr = parseCall(scope);
                }
                else throw new Exception("error pex1 at token:" + tok.peak().locate()+@"
Unknown express found, looking for something that evaluates to a value.");

                
            }

            // check for a single literal.
            else if(tok.peak().isLiteral()){
                expr = new LiteralNode(tok.peak());
                tok.pop();
            }

            // check for a local variable.
            else if (scope.varInScope(tok.peak().getValue()))
            {
                expr = scope.getVarRef(tok.peak().getValue()); 
                tok.pop();
            }
            else throw new Exception("error pex2 at token:" + tok.peak().locate()+@"
An unknown token was found. If its a verable, 
make sure it has been initialized and is in scope. 
Otherwise it is probable out of place");


            debugExit("expr");
            return expr;
        }


        private CallNode parseCall(ILocalScopeNode scope)
        {
            debugEntering("call");
            Token functionName =tok.peak();
            if (!scope.funcInScope(functionName.getValue())) 
                throw new Exception("error Function " + tok.peak().getValue() + " does not exist at " + tok.peak().locate());


            IFunctionNode func=null;

            
            func = scope.getFuncRef(tok.peak().getValue());
           
            LinkedList<ExpressionNode> parameters = new LinkedList<ExpressionNode>();
            
            tok.pop();

            //foreach (ParamNode paramLabel in func.getParameters())

            LinkedListNode<ParamNode> cur = null;

            cur = func.getParameters().First;
            while(tok.peak().getValue() != "]")
            {

                ExpressionNode paramExpr = parseExpression(scope);

                // make sure the signature has another param
                if (cur == null) throw new Exception("error parsing function call.\n Too many parameters were given.\n" + tok.peak().locate());
                // make sure the param data type matchs the function signature,
                if ( paramExpr.getReturnType() != cur.Value.getDataType()) throw new Exception("error pex2 at token:" + tok.peak().locate());

                cur = cur.Next;

                parameters.AddLast(paramExpr);
            }
            if (cur != null) throw new Exception("error parsing function call.\n Too few parameters were given.\n" + tok.peak().locate());
               

            CallNode call = new CallNode(func, parameters);

            // make sure its has closing brace
            if (tok.peak().getValue() != "]") throw new Exception("error pcall 1 at token:" + tok.peak().locate() + "\nToken must be a closing bracket.");
            tok.pop();

            debugExit("call");
            return call;
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
                    opToken.getValue() != "or" ||
                    opToken.getValue() != "and") return true;
            return false;

        }
        public void debugEntering(string name)
        {
            if (Program.parserDebug) {
                Console.Write("entering  " + name + " at " );
                Console.WriteLine( tok.peak().locate());
            }
        }
        public void debugExit(string name)
        {
            if (Program.parserDebug) {
                Console.Write("Exiting   " + name + " at " );//+ tokenizer.peep().locate());
                Console.WriteLine( tok.peak().locate());
            }

        }
    }
}
