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
      
        private RootNode root;
        private int index,length;
        private TokenWrapper tokens;

        /// <summary>
        /// Entery point.
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public Node parseTokens(Token[] tokens)
        {

            this.tokens = new TokenWrapper(tokens);

            debugEntering("begin");

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

            debugExit("begin");

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


            debugEntering("func");

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

            fn = new FunctionNode(returnType, functionName,parameterNames, parameterTypes,root);

            // make sure the token is an closing brace to close the data type list
            if (tokens[index].getValue() != "]") throw new Exception("error pf8 at token:" + tokens[index].locate());
            index++;
            
            // TODO : Need to add support for prototyping. if someone prototype this will throw an error.  <========================================
            // Make sure this function doesnt already exist. 
            if (root.funcInScope(functionName.getValue() )) throw new Exception("error pf9 at token:" + tokens[index].locate());
        
          
            // parse the internal function statements. provide the function node as a LocalScope object.
            children = parseStatements(fn);

            // add the children to the function node.
            fn.addChildren(children);

            // Add the new function to the root node.
            root.addToScope(fn);

            // make sure there is a closing brace.
            if (tokens[index].getValue() != "]") throw new Exception("error 6 at token:" + tokens[index].locate());
            index++;

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
            while (tokens[index].getValue() != "]")
            {
                // make sure the token has refernce type.
                if (tokens[index].getTokenType() != TokenType.DATATYPE) throw new Exception("error ppn1 at token:" + tokens[index].locate());

                types.AddLast(tokens[index]);

                index++;
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
            while (tokens[index].getValue() != "]")
            {
                // make sure the token has refernce type.
                if (tokens[index].getTokenType() != TokenType.REF) throw new Exception("error ppn1 at token:" + tokens[index].locate());

                names.AddLast(tokens[index]);

                index++;
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
        private LinkedList<Node> parseStatements(LocalScope scope)
        {

            debugEntering("statements");           

            LinkedList<Node> children = new LinkedList<Node>();

            // loop until we see the closing brace. 
            // nested braces will be taken care of be recursive calls to this function. 
            while (tokens[index].getValue() != "]")
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
        /// <para> Expects the index to point to the opening brace                  </para>
        /// <para> - </para>
        /// <para> Sets the index AFTER the closing brace ] .                       </para>
        /// <para> Returns a statement.                                             </para>
        /// </summary>
        private Node parseStatement(LocalScope scope)
        {

            debugEntering("statement");
            

            // make sure the token is an opening brace
            if (tokens[index].getValue() != "[") throw new Exception("error ps1 at token:" + tokens[index].locate());
            index++;

            if (tokens[index].getTokenType() == TokenType.ASSIGNMENT)
                return parseAssignment(scope);

            else if (tokens[index].getTokenType() == TokenType.CONSTRUCT)
               return parseConstruct(scope);

            else if (tokens[index].getTokenType() == TokenType.FUNCTION)
                return parseBFunc(scope);

            else if (tokens[index].getTokenType() == TokenType.REF)
                return parseCall(scope);


            debugExit("statement");
            // something went wrong if we get here.
            throw new Exception("error ps2 at token:" + tokens[index].locate());
            
        }

        private Node parseBFunc(LocalScope scope)
        {


            debugEntering("BFunc");
            throw new NotImplementedException();

            debugExit("bfunc");
        }

        private ExpressionNode parseOp(LocalScope scope)
        {


            debugEntering("OP");

            //if(tokens[index].getValue() != "[")
            //    throw new Exception("error, parseOp1 at token:" + tokens[index].locate());
            Token opToken = tokens[index];
            index++;

            ExpressionNode leftExpr, rightExpr = null;
            leftExpr = parseExpression(scope);

            if (isBinOp(opToken))
            {
                rightExpr = parseExpression(scope);

                if (leftExpr.getReturnType() != rightExpr.getReturnType()) throw new Exception("pererr fix this.");
            }



            if (tokens[index].getValue() != "]")
                throw new Exception("error, parseOp1 at token:" + tokens[index].locate());
            index++;

            debugExit("op");
            return  new OpNode(opToken, leftExpr, rightExpr);
        }

        private bool isBinOp(Token opToken)
        {


            debugEntering("isBinOp");

            string[] binops = { "+", "-", "/", "%", "=", ">", ">=", "<", "<=", "!=" };
            foreach (string s in binops)
            {
                if (opToken.getValue() == s)
                {
                    debugExit("construct");
                    return true;
                }
            }

            debugExit("isBinOp");
            return false;
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

            debugEntering("construct");

            //check the curent token for which kind of construct it is.
            if (tokens[index].getValue() == "while")
            {
                debugExit("construct");
                return parseWhileLoop(scope);
            }
            //else if (tokens[index].getValue() == "for")
            //    return parseForLoop(scope);
            else if (tokens[index].getValue() == "if")
            {
                debugExit("construct");
                return parseIf(scope);
            }
            else if (tokens[index].getValue() == "let")
            {
                debugExit("construct");
                return parseLet(scope);
            }
            else
                throw new Exception("error, unknow construct at token:" + tokens[index].locate());
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

            debugEntering("if");

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

            // check for closing brace on the overall if statements.
            if (tokens[index].getValue() != "]") throw new Exception("error pif3 at token:" + tokens[index].locate());
            index++;

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
        private ElseNode parseElse(LocalScope scope)
        {
            debugEntering("else");

            ElseNode elseNode = new ElseNode(scope);
            LinkedList<Node> children;

            // check for the opening of the else statement.
            if (tokens[index].getValue() != "[") throw new Exception("error pel1 at token:" + tokens[index].locate());
            

            // Parse multi statement else 
            if (tokens[index + 1].getValue() == "[")
            {
                index++;
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
        private Node parseWhileLoop(LocalScope scope)
        {

            debugEntering("while");

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
        private AssignmentNode parseAssignment(LocalScope scope)
        {

            debugEntering("assignment");

            AssignmentNode assignment;
            ExpressionNode expr;
            Token varName;

            // make sure its an assignment
            if (tokens[index].getValue() != ":=") throw new Exception("error pa1 at token:" + tokens[index].locate());
            index++;

            varName = tokens[index++];

            // check that the token is in scope
            if (!scope.varInScope(varName.getValue())) throw new Exception("error pa2 at token:" + varName.locate() + " is not in scope");

            // parse the expression into Node(s)
            expr = parseExpression( scope);

            VariableNode varNode = scope.getVarRef(varName.getValue() );

            // check the that the data types match
            if (expr.getReturnType() != varNode.getReturnType() ) throw new Exception("error pa3 at token:" + tokens[index].locate());


            assignment = new AssignmentNode(varNode, expr);

            // make sure its an assignment
            if (tokens[index].getValue() != "]") throw new Exception("error pa1 at token:" + tokens[index].locate());
            index++;


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
        private Node parseLet(LocalScope scope)
        {


            debugEntering("let");

            LetNode let = new LetNode();
            Token dataType;
            Token variableName;

            
            // check that its has a valid data type
            if (tokens[index].getTokenType() == TokenType.ASSIGNMENT ) throw new Exception("error pl1 at token:" + tokens[index].locate() );
            index++;

            while (tokens[index].getValue() != "]") {

                // make sure its a open brace token.
                if (tokens[index].getValue() != "[") throw new Exception("error pl6 at token:" + tokens[index].locate());
                variableName = tokens[index++];

                // make sure its a reference token and get its label.
                if (tokens[index].getTokenType() != TokenType.REF) throw new Exception("error pl2 at token:" + tokens[index].locate());
                variableName = tokens[index++];

                 // check that its has a valid data type
                if (tokens[index].getTokenType() != TokenType.DATATYPE ) throw new Exception("error pl3 at token:" + tokens[index].locate() );
                dataType = tokens[index++];


                // make sure its not in scope.
                if (scope.varInScope(variableName.getValue()))
                    throw new Exception("error pl4 at Token " + tokens[index -2].locate() + "  " + variableName.getValue() + " is already in scope.");

                if (tokens[index].getValue() != "]") throw new Exception("error pl5 at token:" + tokens[index].locate() );
                index++;

                // construct the declaration node.
                DeclarationNode dec = new DeclarationNode(dataType, variableName); 
            
                // add the variable to local scope.
                scope.addToScope(dec);

                let.addChild(dec);
            }

            if (tokens[index].getValue() != "]") throw new Exception("error pl6 at token:" + tokens[index].locate());
            index++;

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
        private ExpressionNode parseExpression(LocalScope scope)
        {

            debugEntering("expr");

            ExpressionNode expr;

            // check for a composite expression.
            if (tokens[index].getValue() == "[")
            {
                index++;
                if (tokens[index].getTokenType() == TokenType.OP)
                    expr =  parseOp(scope);

                else if(scope.funcInScope(tokens[index].getValue())){
                    expr = parseCall(scope);
                }
                else throw new Exception("error pex1 at token:" + tokens[index].locate());

                
            }

            // check for a single literal.
            else if(tokens[index].isLiteral()){
                expr = new LiteralNode(tokens[index]);
                index++;
            }

            // check for a local variable.
            else if (scope.varInScope(tokens[index].getValue())){
                expr = scope.getVarRef(tokens[index].getValue());
                index++;
            }
            else throw new Exception("error pex2 at token:" + tokens[index].locate());


            debugExit("expr");
            return expr;
        }


        private CallNode parseCall(LocalScope scope)
        {
            debugEntering("call");
            
            if (!scope.funcInScope(tokens[index].getValue())) 
                throw new Exception("error Function " + tokens[index].getValue() + " does not exist at " + tokens[index].locate());
           
            FunctionNode func = scope.getFuncRef(tokens[index].getValue());
            LinkedList<ExpressionNode> parameters = new LinkedList<ExpressionNode>();
            
            index++;

            foreach (ParamNode paramLabel in func.getParameters())
            {

                ExpressionNode paramExpr = parseExpression(scope);

                // make sure the param data type matchs the function signature,
                if (paramExpr.getReturnType() != paramLabel.getDataType()) throw new Exception("error pex2 at token:" + tokens[index].locate());

                parameters.AddLast(paramExpr);
            }

            CallNode call = new CallNode(func, parameters);

            // make sure its has closing brace
            if (tokens[index].getValue() != "]") throw new Exception("error pcall 1 at token:" + tokens[index].locate());
            index++;

            debugExit("call");
            return call;
        }

        public void debugEntering(string name)
        {
            if (debug) {
                Console.Write("entering  " + name + " at " );
                Console.WriteLine( tokens[index].locate());
            }
        }
        public void debugExit(string name)
        {
            if (debug) {
                Console.Write("Exiting   " + name + " at " );//+ tokens[index].locate());
                Console.WriteLine( tokens[index].locate());
            }

        }
    }
}
