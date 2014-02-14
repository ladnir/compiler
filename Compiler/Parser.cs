using Compiler.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class Parser
    {
        public const bool debug = true;
        public const bool symantics = false;
      
        private RootNode root;
        private Tokenizer tok;

        public Node parseT(Tokenizer t)
        {
            tok = t;
            RootNode root = new RootNode();
            debugEntering("parseT");

            // make sure the file starts with an opening brace.
            if (tok.peep().getValue() != "[")
                throw new Exception("error pT1 at token:" + tok.peep().locate());
            tok.pop();
            while(tok.peep().getValue() !="]"){
                root.addChild( parseS(root));
               
            }

            // make sure the file ends with closing brce.
            if (tok.peep().getValue() != "]")
                throw new Exception("error pT2 at token:" + tok.peep().locate());

            debugExit("parseT");
            tok.pop();

            try
            {
                tok.pop();
                Console.WriteLine("WARNING, End of file was not reached." + tok.peep().locate());

            }
            catch (EndOfTokensException e) { }
            return root;
        }

        private Node parseS(ILocalScopeNode scope)
        {
            debugEntering("parseS");
            SNode s = new SNode();

           
            while (tok.peep().getValue() != "]") // parse until we see a closing brace for parent
            {
                if (tok.peep().getValue() == "[") 
                    // S' has an open brace. can either be another S or an assigment, binop,unop or stmts
                {
                    tok.pop();           
         
                    // check to see if the open brace belongs to a oper/stmts
                    if (tok.peep().getTokenType() == TokenType.ASSIGNMENT)
                        s.addChild(parseAssignment(scope));
                    else if (tok.peep().getTokenType() == TokenType.CONSTRUCT)
                        s.addChild(parseConstruct(scope));
                    else if (tok.peep().getTokenType() == TokenType.OP)
                        s.addChild(parseOp(scope));

                    else // ok, We now know that the open brace belongs to S
                    {
                        //s.setBraces(); // let S know it has braces.
                        SNode child = (SNode)parseS(scope); // and recurs
                        child.setBraces();
                        s.addChild(child);

                        if (tok.peep().getValue() != "]")
                            throw new Exception("error pS2 at token:" + tok.peep().locate());

                        tok.pop();
                    }

                } 
                    // S does not have an openbrace brace. we can assume its wither a refernce or a literal
                else if (tok.peep().getTokenType() == TokenType.REF)
                    s.addChild(parseExpression(scope));
                else if (tok.peep().isLiteral())
                    s.addChild(new LiteralNode(tok.pop()));
                else
                {
                    throw new Exception("error pS1 ,invalid tokens at " + tok.peep().locate() +"\n Only operation or statements can be in an S production");
                }
                
            } 
            


            debugExit("parseS");
            return s;
        }

        /// <summary>
        /// Entery point.
        /// 
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
                throw new Exception("error pt1 at token:" + tok.peep().locate());

            // parse functions. still need to add code for gobal vars.
            while (tok.peep().getValue() == "[")
            {
                throw new NotImplementedException();
                //Node function = parseFunction(root);
                //root.addChild(function);

            }

            debugExit("begin");

            // make sure the file ends with closing brce.
            if (tok.pop().getValue() != "]")
                throw new Exception("error pt2 at token:" + tok.peep().locate());

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
        private Node parseFunction(Token functionName,ILocalScopeNode scope)
        {


            debugEntering("func");

            UserFunctionNode fn;
            Token returnType;
            LinkedList<Token> parameterNames;
            LinkedList<Token> parameterTypes;
            LinkedList<Node> children;

            // parse the parameters names. This should return with index pointing to the closing brace.
            parameterNames = parseParameterNames();

            // make sure the token is an closing brace
            if (tok.peep().getValue() != "]") throw new Exception("error pf5 at token:" + tok.peep().locate());
            tok.pop();

            // make sure the token is an opening brace
            if (tok.peep().getValue() != "[") throw new Exception("error pf6 at token:" + tok.peep().locate());
            tok.pop();

            // make sure the token is a return type ( i.e. int, bool , ... ) .
            if ( tok.peep().getTokenType() != TokenType.DATATYPE ) throw new Exception("error pf7 at token:" + tok.peep().locate() );
            returnType = tok.pop();;

            // parse the parameters names. This should return with index pointing to the closing brace.
            parameterTypes = parseParameterTypes();

            fn = new UserFunctionNode(returnType, functionName,parameterNames, parameterTypes,scope);

            // make sure the token is an closing brace to close the data type list
            if (tok.peep().getValue() != "]") throw new Exception("error pf8 at token:" + tok.peep().locate());
            tok.pop();
            
            // TODO : Need to add support for prototyping. if someone prototype this will throw an error.  <========================================
            // Make sure this function doesnt already exist. 
            if (symantics && scope.funcInScope(functionName.getValue())) throw new Exception("error pf9 at token:" + tok.peep().locate());
        
          
            // parse the internal function statements. provide the function node as a LocalScope object.
            children = parseStatements(fn);

            // add the children to the function node.
            fn.addChildren(children);

            // Add the new function to the root node.
            if (symantics) scope.addToScope(fn);

            // make sure there is a closing brace.
            if (tok.peep().getValue() != "]") throw new Exception("error 6 at token:" + tok.peep().locate());
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
            while (tok.peep().getValue() != "]")
            {
                // make sure the token has refernce type.
                if (tok.peep().getTokenType() != TokenType.DATATYPE) throw new Exception("error ppn1 at token:" + tok.peep().locate());

                types.AddLast(tok.peep());

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
            while (tok.peep().getValue() != "]")
            {
                // make sure the token has refernce type.
                if (tok.peep().getTokenType() != TokenType.REF) throw new Exception("error ppn1 at token:" + tok.peep().locate());

                names.AddLast(tok.peep());

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
            while (tok.peep().getValue() != "]")
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
            if (tok.peep().getValue() == "[")
            {//throw new Exception("error pst1 at token:" + tok.peep().locate());
                tok.pop();

                if (tok.peep().getTokenType() == TokenType.ASSIGNMENT)
                    return parseAssignment(scope);

                if (tok.peep().getTokenType() == TokenType.OP)
                    return parseOp(scope);

                else if (tok.peep().getTokenType() == TokenType.CONSTRUCT)
                    return parseConstruct(scope);

                else if (tok.peep().getTokenType() == TokenType.FUNCTION)
                    return parseCall(scope);

                else if (tok.peep().getTokenType() == TokenType.REF)
                    return parseCall(scope);

            }
            else if (tok.peep().isLiteral())
                return parseExpression(scope);
            else if (tok.peep().getTokenType() == TokenType.REF)
                return parseExpression(scope);

            debugExit("statement");
            // something went wrong if we get here.
            throw new Exception("error ps2 at token:" + tok.peep().locate());
            
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

            Token opToken = tok.peep();
            tok.pop();

            ExpressionNode leftExpr, rightExpr = null;
            leftExpr = parseExpression(scope);

            if ( tok.peep().getValue() != "]")
            {
                if (opToken.getValue() == "not" ||
                    opToken.getValue() == "sin" ||
                    opToken.getValue() == "cos" ||
                    opToken.getValue() == "tan" ) throw new Exception("error, not a binary operator. " + opToken.locate());

                rightExpr = parseExpression(scope);

                if (symantics && leftExpr.getReturnType() != rightExpr.getReturnType()) throw new Exception("pererr fix this.");
            }

            if (tok.peep().getValue() != "]")
                throw new Exception("error, parseOp1 at token:" + tok.peep().locate() + "\n expecting a ]\n in exprestion "+opToken.getValue());

            tok.pop();

            debugExit("op");
            return  new OpNode(opToken, leftExpr, rightExpr);
        }



        /// <summary>
        /// <para> - </para>
        /// <para> Expects the local scope that it lives in.             </para>
        /// <para> Expects the index to point at the construct token.    </para>
        /// <para> - </para>
        /// <para> Sets the index AFTER the closing brace.               </para>
        /// <para> Returns a construct node.                             </para>
        /// </summary>
        private ExpressionNode parseConstruct(ILocalScopeNode scope)
        { //TODO: change back to Node

            debugEntering("construct");

            //check the curent token for which kind of construct it is.
            if (tok.peep().getValue() == "while")
            {
                debugExit("construct");
                return parseWhileLoop(scope);
            }
            else if (tok.peep().getValue() == "stdout")
                return parseStdout(scope);
            else if (tok.peep().getValue() == "if")
            {
                debugExit("construct");
                return parseIf(scope);
            }
            else if (tok.peep().getValue() == "let")
            {
                debugExit("construct");
                return parseLet(scope);
            }
            else
                throw new Exception("error, unknow construct at token:" + tok.peep().locate());
        }

        private ExpressionNode parseStdout(ILocalScopeNode scope)
        {// TODO change back to Node
            tok.pop();
            ExpressionNode expr = parseExpression(scope);
            if (tok.peep().getValue() != "]") throw new Exception("error, parsing stdout. expecting closing brace ]. " + tok.peep().locate());

            tok.pop();
            return new StdoutNode(expr);
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
        private ExpressionNode parseIf(ILocalScopeNode scope)
        { //TODO: change back to Node

            debugEntering("if");

            IfNode ifNode;
            ExpressionNode eval;
            LinkedList<Node> children;

            // check for the opening of the if statement.
            if (tok.peep().getValue() != "if") throw new Exception("error pi1 at token:" + tok.peep().locate());
            tok.pop();
            
            // parse the evaluation node
            eval = parseExpression( scope);

            // check that the eval node returns a boolean.
            if (symantics && eval.getReturnType() != "bool") throw new Exception("error pi4 at token:" + tok.peep().locate());

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
            if (tok.peep().getValue() != "]")
            {
                ifNode.addElse(parseElse(scope));

            }

            // check for closing brace on the overall if statements.
            if (tok.peep().getValue() != "]") throw new Exception("error pif3 at token:" + tok.peep().locate());
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
        private ExpressionNode parseWhileLoop(ILocalScopeNode scope)
        { // TODO change back to Node

            debugEntering("while");

            WhileLoopNode wl;
            ExpressionNode eval;
            LinkedList<Node> children;

            // check for the opening of the while statement.
            if (tok.peep().getValue() != "while") throw new Exception("error pw1 at token:" + tok.peep().locate());
            tok.pop();

            // parse the evaluation node
            eval = parseExpression(scope);

            // check that the eval node returns a boolean.
            if (symantics && eval.getReturnType() != "bool") throw new Exception("error pw4 at token:" + tok.peep().locate());

            
            // create the if node so that the parse statements call will have a local scope to use.
            wl = new WhileLoopNode(eval, scope);

            children = parseStatements(wl);

            // add the children.
            wl.addChildren(children);

            // check for closing brace 
            if (tok.peep().getValue() != "]") throw new Exception("error pw9 at token:" + tok.peep().locate());
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
            if (tok.peep().getValue() != ":=") throw new Exception("error pa1 at token:" + tok.peep().locate());
            tok.pop();

            varName = tok.pop();
            if (varName.getTokenType() != TokenType.REF) throw new Exception("LValue must be a variable name. " + varName.locate());
            // check that the token is in scope
            if (symantics && !scope.varInScope(varName.getValue())) throw new Exception("error pa2 at token:" + varName.locate() + "\nVariable is not in scope");

            // parse the expression into Node(s)
            expr = parseExpression( scope);

            if (scope.varInScope(varName.getValue()))
            {  //TODO REMOVE THIS ############################################################################################################ 
                varNode = scope.getVarRef(varName.getValue());
            }
            else
            {  //TODO REMOVE THIS ############################################################################################################ 
                DeclarationNode dec = new DeclarationNode(null, varName);
                varNode = new VariableNode(dec);
            }
            // check the that the data types match
            if (symantics && expr.getReturnType() != varNode.getReturnType()) throw new Exception("error pa3 at token:" + tok.peep().locate()+"\nTypes dont match");

            assignment = new AssignmentNode(varNode, expr);

            // make sure its an assignment
            if (tok.peep().getValue() != "]") throw new Exception("error pa1 at token:" + tok.peep().locate());
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
        private ExpressionNode  parseLet(ILocalScopeNode scope)
        { // TODO change back to Node


            debugEntering("let");

            LetNode let = new LetNode();
            Token dataType;
            Token variableName;

            
            // check that its has a valid data type
            if (tok.peep().getValue() != "let" ) throw new Exception("error pl1 at token:" + tok.peep().locate() );
            tok.pop();

            // make sure its a open brace token.
            if (tok.peep().getValue() != "[") throw new Exception("error pl6 at token:" + tok.peep().locate());
            tok.pop();

            // make sure its a reference token and get its label.
            if (tok.peep().getTokenType() != TokenType.REF) throw new Exception("error pl2 at token:" + tok.peep().locate());
            variableName = tok.pop();

            // Check to see if this is a variable declarations. 
            if (tok.peep().getTokenType() == TokenType.DATATYPE)
            {
                while (true)
                { 
                    // check that its has a valid data type
                    if (tok.peep().getTokenType() != TokenType.DATATYPE) throw new Exception("error pl3 at token:" + tok.peep().locate());
                    dataType = tok.pop();


                    // make sure its not in scope.
                    if (symantics && scope.varInScope(variableName.getValue()))
                        throw new Exception("error pl4 at Token " + variableName.locate() + "  " + variableName.getValue() + " is already in scope.");

                    if (tok.peep().getValue() != "]") throw new Exception("error pl5 at token:" + tok.peep().locate());
                    tok.pop();

                    // construct the declaration node.
                    DeclarationNode dec = new DeclarationNode(dataType, variableName);

                    // add the variable to local scope.
                    if(symantics) scope.addToScope(dec);

                    let.addChild(dec);

                    // This is where we check to see if there is another variable declaration
                    // Its like this because the function declaration diverges from variable declarations.
                    if (tok.peep().getValue() == "]") break; 

                    // make sure its a open brace token.
                    if (tok.peep().getValue() != "[") throw new Exception("error pl6 at token:" + tok.peep().locate());
                    tok.pop();

                    // make sure its a reference token and get its label.
                    if (tok.peep().getTokenType() != TokenType.REF) throw new Exception("error pl2 at token:" + tok.peep().locate());
                    variableName = tok.pop();

                }
            }
                // chech to see if it is a function declaration.
            else if (tok.peep().getTokenType() == TokenType.REF || tok.peep().getValue() == "]")
            {

               
                // TODO fix this back to be just a node
               return (ExpressionNode)parseFunction(variableName,scope);

            }else // if its not a variable or function declatation there must be an error.
                throw new Exception("Token in let stament is incorrect...  "+tok.peep().locate());

            if (tok.peep().getValue() != "]") throw new Exception("error pl6 at token:" + tok.peep().locate());
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
            if (tok.peep().getValue() == "[")
            {
                tok.pop();
                if (tok.peep().getTokenType() == TokenType.OP)
                    expr =  parseOp(scope);

                else if (tok.peep().getTokenType() == TokenType.ASSIGNMENT)
                {
                    expr = parseAssignment(scope);
                }
                else if (tok.peep().getTokenType() == TokenType.CONSTRUCT)
                {
                    expr = parseConstruct(scope);
                }
                else{// if(scope.funcInScope(tok.peep().getValue())){ //TODO: REMOVE THIS ##################################################################
                    expr = parseCall(scope);
                }
                //else throw new Exception("error pex1 at token:" + tok.peep().locate());

                
            }

            // check for a single literal.
            else if(tok.peep().isLiteral()){
                expr = new LiteralNode(tok.peep());
                tok.pop();
            }

            // check for a local variable.
            else{// 
                if (scope.varInScope(tok.peep().getValue()))
                {
                    expr = scope.getVarRef(tok.peep().getValue());
                }
                else
                { // REMOVE THIS #########################################################################################################################
                    DeclarationNode dec = new DeclarationNode(null, tok.peep());
                    expr = new VariableNode(dec);
                }
                tok.pop();
            }
            //else throw new Exception("error pex2 at token:" + tok.peep().locate());


            debugExit("expr");
            return expr;
        }


        private CallNode parseCall(ILocalScopeNode scope)
        {
            debugEntering("call");
            Token functionName =tok.peep();
            if (symantics && !scope.funcInScope(functionName.getValue())) 
                throw new Exception("error Function " + tok.peep().getValue() + " does not exist at " + tok.peep().locate());


            IFunctionNode func=null;

            if (scope.funcInScope(tok.peep().getValue())) 
                func = scope.getFuncRef(tok.peep().getValue());
            else
            {//TODO Remove this ##########################################################################
                func = new UserFunctionNode( functionName);
            }
            LinkedList<ExpressionNode> parameters = new LinkedList<ExpressionNode>();
            
            tok.pop();

            //foreach (ParamNode paramLabel in func.getParameters())

            LinkedListNode<ParamNode> cur;

            if (symantics) cur = func.getParameters().First;
            while(tok.peep().getValue() != "]")
            {

                ExpressionNode paramExpr = parseExpression(scope);

                // make sure the param data type matchs the function signature,
                if (symantics && paramExpr.getReturnType() != cur.Value.getDataType()) throw new Exception("error pex2 at token:" + tok.peep().locate());

                if (symantics) cur = cur.Next;

                parameters.AddLast(paramExpr);
            }
            Console.WriteLine("call");
            CallNode call = new CallNode(func, parameters);

            // make sure its has closing brace
            if (tok.peep().getValue() != "]") throw new Exception("error pcall 1 at token:" + tok.peep().locate());
            tok.pop();

            debugExit("call");
            return call;
        }

        public void debugEntering(string name)
        {
            if (debug) {
                Console.Write("entering  " + name + " at " );
                Console.WriteLine( tok.peep().locate());
            }
        }
        public void debugExit(string name)
        {
            if (debug) {
                Console.Write("Exiting   " + name + " at " );//+ tokenizer.peep().locate());
                Console.WriteLine( tok.peep().locate());
            }

        }
    }
}
