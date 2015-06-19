using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.parser
{



    class OpNode : ExpressionNode
    {
        private List<Gate> gates;
        private Token opToken;
        private ExpressionNode expr1;
        private ExpressionNode expr2;
        private ExpressionNode expr3;

        public OpNode(Token opToken, ExpressionNode ex1, ExpressionNode ex2, ExpressionNode ex3)
        {
            // TODO: Complete member initialization
            this.opToken = opToken;
            this.expr1 = ex1;
            this.expr2 = ex2;
            this.expr3 = ex3;
        }


        public override List<Gate> NodeOutGates
        {
            get { return gates; }
        }


        public override void toCircuit(List<Gate> output, ref int nextWireID, StringBuilder dot)
        {
            gates = new List<Gate>();

            if (this.opToken.getValue() == "+")
            {
                nextWireID = CircuitAdd(output, nextWireID, dot);
            }
            else if (this.opToken.getValue() == "-")
            {
                nextWireID = CircuitSubtract(output, nextWireID, dot);
            }
            else if (opToken.getValue() == "*")
            {
                nextWireID = CircuitMultiply(output, nextWireID, dot);
            }
            else if (opToken.getValue() == "^")
            {
                if (expr1.NodeOutGates.Count != expr2.NodeOutGates.Count)
                    throw new Exception();

                for(int i = 0; i < expr1.NodeOutGates.Count; i++)
                {
                    gates.Add(new XorGate(nextWireID++, expr1.NodeOutGates[i], expr2.NodeOutGates[i], output));
                }
            }
            else if (opToken.getValue() == "#")
            {

                VariableNode var = (VariableNode)expr1;
                int startIdx = Int32.Parse(((LiteralNode)expr2).litToken.getValue());
                int bitCount = Int32.Parse(((LiteralNode)expr3).litToken.getValue());

                NodeOutGates.AddRange(var.NodeOutGates.GetRange(startIdx, bitCount).ToList());
            }
            else if (opToken.getValue() == "$")
            {
                VariableNode var1 = (VariableNode)expr1;
                VariableNode var2 = (VariableNode)expr1;

                NodeOutGates.AddRange(var1.NodeOutGates);
                NodeOutGates.AddRange(var2.NodeOutGates);
            }
            else
                throw new NotImplementedException();
        }

        private int CircuitMultiply(List<Gate> output, int nextWireID, StringBuilder dot)
        {
            if (expr1.NodeOutGates.Count != expr2.NodeOutGates.Count)
                throw new Exception();

            //List<Gate> temp = new List<Gate>();
            List<Gate> runningTotal = new List<Gate>();

            for (int j = 0; j < expr2.NodeOutGates.Count; j++)
            {
                runningTotal.Add(new AndGate(nextWireID++, expr1.NodeOutGates[0], expr2.NodeOutGates[j], output));
            }

            for (int i = 1; i < expr1.NodeOutGates.Count; i++)
            {
                Gate carry = null;
                for (int j = 0; j < expr2.NodeOutGates.Count - i; j++)
                {
                    var b  = new AndGate(nextWireID++, expr1.NodeOutGates[i], expr2.NodeOutGates[j], output);
                    var a = runningTotal[i + j];

                    var abXOR = new XorGate(nextWireID++, a, b, output);

                    if(j == 0)
                    {
                        runningTotal[i + j] = abXOR;
                        carry = new AndGate(nextWireID++, a, b, output);
                    }
                    else
                    {

                    }
                    
                }

                
            }

                return nextWireID;
        }

        private int CircuitSubtract(List<Gate> output, int nextWireID, StringBuilder dot)
        {
            if (expr1.NodeOutGates.Count != expr2.NodeOutGates.Count)
                throw new Exception();

            expr1.toCircuit(output, ref nextWireID, dot);
            expr2.toCircuit(output, ref nextWireID, dot);

            Gate a = expr1.NodeOutGates[0];
            Gate b = expr2.NodeOutGates[0];

            Gate sum = new XorGate(nextWireID++, a, b, output);
            NodeOutGates.Add(sum);

            if (expr1.NodeOutGates.Count > 1)
            {
                Gate bor = new AndNot2Gate(nextWireID++, a, b, output);

                for (int i = 1; i < expr1.NodeOutGates.Count; i++)
                {
                    a = expr1.NodeOutGates[i];
                    b = expr2.NodeOutGates[i];

                    Gate abXOR = new XorGate(nextWireID++, a, b, output);
                    sum = new XorGate(nextWireID++, abXOR, bor, output);

                    NodeOutGates.Add(sum);

                    // carry gates
                    if (i < expr1.NodeOutGates.Count - 1)
                    {
                        Gate abANDNot2 = new AndNot2Gate(nextWireID++, a, b, output);
                        Gate abXORcANDNot2 = new AndNot2Gate(nextWireID++, bor, abXOR, output);
                        bor = new OrGate(nextWireID++, abANDNot2, abXORcANDNot2, output);
                    }
                }
            }
            return nextWireID;
        }

        private int CircuitAdd(List<Gate> output, int nextWireID, StringBuilder dot)
        {
            //dot.append("subgraph cluster_" + opToken.locateShort() + " {\n label=\"add_" + opToken.locateShort() + "\";\n");

            if (expr1.NodeOutGates.Count != expr2.NodeOutGates.Count)
                throw new Exception();

            expr1.toCircuit(output, ref nextWireID, dot);
            expr2.toCircuit(output, ref nextWireID, dot);

            Gate a = expr1.NodeOutGates[0];
            Gate b = expr2.NodeOutGates[0];

            Gate sum = new XorGate(nextWireID++, a, b, output);
            NodeOutGates.Add(sum);

            //dot.append("g" + sum.mID + "[label=\"sum0\"];\n");
            //dot.append("g" + a.mID + " -> g" + sum.mID + ";\n");
            //dot.append("g" + b.mID + " -> g" + sum.mID + ";\n");

            if (expr1.NodeOutGates.Count > 1)
            {
                Gate carry = new AndGate(nextWireID++, a, b, output);

                //dot.append("g" + a.mID + " -> g" + carry.mID + ";\n");
                //dot.append("g" + b.mID + " -> g" + carry.mID + ";\n");

                for (int i = 1; i < expr1.NodeOutGates.Count; i++)
                {
                    a = expr1.NodeOutGates[i];
                    b = expr2.NodeOutGates[i];

                    Gate abXOR = new XorGate(nextWireID++, a, b, output);

                    //dot.append("g" + a.mID + " -> g" + abXOR.mID + ";\n");
                    //dot.append("g" + b.mID + " -> g" + abXOR.mID + ";\n");

                    sum = new XorGate(nextWireID++, abXOR, carry, output);

                    //dot.append("g" + abXOR.mID + " -> g" + sum.mID + ";\n");
                    //dot.append("g" + carry.mID + " -> g" + sum.mID + ";\n");
                    //dot.append("g" + sum.mID + "[label=\"sum"+i+"\"];\n");

                    NodeOutGates.Add(sum);

                    // carry gates
                    if (i < expr1.NodeOutGates.Count - 1)
                    {
                        Gate abAND = new AndGate(nextWireID++, a, b, output);

                        //dot.append("g" + a.mID + " -> g" + abAND.mID + ";\n");
                        //dot.append("g" + b.mID + " -> g" + abAND.mID + ";\n");

                        Gate abXORcAND = new AndGate(nextWireID++, abXOR, carry, output);

                        //dot.append("g" + abXOR.mID + " -> g" + abXORcAND.mID + ";\n");
                        //dot.append("g" + carry.mID + " -> g" + abXORcAND.mID + ";\n");

                        carry = new OrGate(nextWireID++, abAND, abXORcAND, output);

                        //dot.append("g" + abAND.mID + " -> g" + carry.mID + ";\n");
                        //dot.append("g" + abXORcAND.mID + " -> g" + carry.mID + ";\n");
                    }
                }
            }

            //dot.append("}");
            return nextWireID;
        }

        public override void outputGForth(int tabCount, StringBuilder sb)
        {
            //if (Parser.debug) Console.Write(opToken.getValue() + " ");

            if(isSpecialOper()){ // special cases
                outputSpecialGForth(tabCount, sb);

            }
            else
            {   // general bin or unnary opperators
                expr1.outputGForth(tabCount, sb);
                if (expr2 != null)
                {
                    if (expr1.getReturnType() != expr2.getReturnType())
                        castTypes(expr1, expr2, sb);
                    expr2.outputGForth(tabCount, sb);
                    if (expr1.getReturnType() != expr2.getReturnType())
                        castTypes(expr2,expr1,  sb);

                }
                if(this.getReturnType() == "int" || this.getReturnType() == "bool")
                    sb.Append(" " + opToken.getValue());
                else if (this.getReturnType() == "float")
                    sb.Append(" f" + opToken.getValue());
                
                    
            }
        }

        private void outputSpecialGForth(int tabCount, StringBuilder sb)
        {
            if (opToken.getValue() == "%")
                outputModGForth(tabCount, sb);
            else if (opToken.getValue() == "and" || opToken.getValue() == "or")
            {
                if (expr1.getReturnType() != "int" && expr1.getReturnType() != "bool")
                    throw new Exception("casting error on '" + opToken.getValue() + "'. return type must be bool or int.\n" + opToken.locate());
                if (expr2.getReturnType() != "int" && expr2.getReturnType() != "bool")
                    throw new Exception("casting error on '" + opToken.getValue() + "'. return type must be bool or int.\n" + opToken.locate());

                expr1.outputGForth(tabCount, sb);
                sb.Append(" ");
                expr2.outputGForth(tabCount, sb);
                sb.Append(" "+opToken.getValue() +" ");
                
            }
            else if (opToken.getValue() == "-" && expr1.getReturnType() == "bool" && expr2 == null)
            {
                expr1.outputGForth(tabCount, sb);
                sb.Append(" 0= ");
            }

            else  if (opToken.getValue() == "+" &&
                expr2 != null &&
                expr2.getReturnType() == "string" &&
                expr1.getReturnType() == "string")
            {
                outputStringConcatination(tabCount, sb);
            }

            else if (opToken.getValue() == "^")
                outputPowerGForth(tabCount, sb);

            else if (opToken.getValue() == "sin" ||
                opToken.getValue() == "cos" ||
                opToken.getValue() == "tan")
            {
                expr1.outputGForth(tabCount, sb);
                if (expr1.getReturnType() == "int")
                    sb.Append("s>f ");
                else if (expr1.getReturnType() != "float") throw new Exception("OpNode error, trig functions only work with int ad float values.");
                sb.Append("f" + opToken.getValue() + " ");

            }
            else if (opToken.getValue() == "!=")
            {
                expr1.outputGForth(tabCount, sb);
                expr2.outputGForth(tabCount, sb);
                sb.Append(" <>");

            }
            else if (opToken.getValue() == "-" && expr2 == null)
            {
                expr1.outputGForth(tabCount, sb);
                if (expr1.getReturnType() == "float") sb.Append(" fnegate ");
                else
                    sb.Append(" negate ");

            }
            else if ((expr1.getReturnType() == "float" ||
                      expr2.getReturnType() == "float") 
                    &&
                    (opToken.getValue() == "<" ||
                    opToken.getValue() == "<=" ||
                    opToken.getValue() == ">"  ||
                    opToken.getValue() == ">=" ||
                    opToken.getValue() == "="  ||
                    opToken.getValue() == "!=" ) ){

                        outputFloatBoolGForth(tabCount, sb);
            }
            else
            {
                throw new Exception("OpNode error, unknown special oper type" + opToken.locate());
            }
        }

        private void outputFloatBoolGForth(int tabCount, StringBuilder sb)
        {
            expr1.outputGForth(tabCount, sb);
            if(expr1.getReturnType() != "float"){
                if(expr1.getReturnType() == "string") throw new Exception("strings can not be used in boolean operation."+opToken.locate());
                sb.Append("s>f ");
            }
            expr2.outputGForth(tabCount,sb);
            if(expr2.getReturnType() != "float"){
                if (expr2.getReturnType() == "string") throw new Exception("strings can not be used in boolean operation." + opToken.locate());
                sb.Append("s>f ");
            }

            sb.Append("f"+opToken.getValue()+" ");
        }

        private void outputStringConcatination(int tabCount, StringBuilder sb)
        {
            expr1.outputGForth(tabCount, sb);
            sb.Append(" pad place ");
            expr2.outputGForth(tabCount, sb);
            sb.Append(" pad +place pad count ");
        }

        private void outputPowerGForth(int tabCount, StringBuilder sb)
        {
            if (expr1.getReturnType() != "float" && expr1.getReturnType() != "int") throw new Exception("OpNode error, exponent base must be an int or float." + opToken.locate());
            if (expr2.getReturnType() != "int") throw new Exception("OpNode error, exponent power must be an int." + opToken.locate());
    
            expr1.outputGForth(tabCount, sb);
            sb.Append(" ");
            expr2.outputGForth(tabCount, sb);

            if (expr1.getReturnType() == "int") sb.Append( " ^");
            else sb.Append(" f^");
            
        }

        private void outputModGForth(int tabCount, StringBuilder sb)
        {
            if (expr1 == null || expr2 == null) throw new Exception("OpNode error, mod expects two arguments." + opToken.locate());

            switch (expr1.getReturnType())
            {
                case "float":   // algorithm for  (float % float ) or (float % int)
                    // looks like:   11e % 10e = 11e 11e 10e f/ f>s s>f 10e f* f-

                    expr1.outputGForth(tabCount, sb);
                    sb.Append(" ");
                    expr2.outputGForth(tabCount, sb);
                    if (expr2.getReturnType() == "int")
                        sb.Append(" s>f");
                    sb.Append("  fmod");
                    

                    break;
                case "int":
                    if (expr2.getReturnType() == "int")
                    {
                        expr1.outputGForth(tabCount, sb);
                        sb.Append(" ");
                        expr2.outputGForth(tabCount, sb);
                        sb.Append(" mod ");
                    }
                    else if (expr2.getReturnType() == "float")
                    {
                        expr1.outputGForth(tabCount, sb);
                        sb.Append(" s>f ");
                        expr2.outputGForth(tabCount, sb);
                        sb.Append(" fmod ");

                    }
                    break;
                default:
                    throw new Exception("OpNode error, mof only works on int and floats." + opToken.locate());
                    break;
            }
        }

        private bool isSpecialOper()
        {
            if (opToken.getValue() == "sin" ||
                opToken.getValue() == "cos" ||
                opToken.getValue() == "tan" ||
                opToken.getValue() == "and" ||
                opToken.getValue() == "or"  ||
                opToken.getValue() == "^"   ||
                opToken.getValue() == "%"   ||
                opToken.getValue() == "!="   ) return true;

            if (expr1.getReturnType() == "float" || 
                (expr2 != null && expr2.getReturnType() == "float"))
            {
                if( opToken.getValue() == "<"  ||
                    opToken.getValue() == "<=" ||
                    opToken.getValue() == ">"  ||
                    opToken.getValue() == ">=" ||
                    opToken.getValue() == "="  ||
                    opToken.getValue() == "!="  ) return true;
            }

            if (opToken.getValue() == "-"   && 
                expr2 == null)          return true;

            if (opToken.getValue() == "+"               && 
                expr2 != null                       && 
                expr2.getReturnType() == "string"   && 
                expr1.getReturnType() == "string")   return true;

            return false;
        }

        private void castTypes(ExpressionNode node,ExpressionNode other,StringBuilder sb)
        {
            
            switch (node.getReturnType()){
                
                case "int":
                    if (other.getReturnType() == "float") sb.Append(" s>f ");
                    else throw new Exception("gforth generation error: CASTING."+@" 
int is not compattable with " + other.getReturnType() + "   \n  "+opToken.locate());

                    break;
                case "float":
                    // do nothing? ha
                    break;
                case "bool":
                    if (other.getReturnType() == "float") sb.Append(" s>f ");
                    else if (other.getReturnType() == "int") ;
                    else throw new Exception("gforth generation error: CASTING." + @" 
bool is not compattable with " + other.getReturnType() + "   \n  " + opToken.locate());

                    break;
                case"string":
                    if (other.getReturnType() != "string") throw new Exception("gforth generation error: CASTING." + @" 
string is not compattable with " + other.getReturnType() + "   \n  " + opToken.locate());

                    break;
                default:
                    throw new Exception("gforth generation error: CASTING." + @" 
unknown type at " + node.getReturnType() + "   \n  " + opToken.locate());
                    break;
            }
        }



        

        public override string outputIBTL(int tabCount)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[ " + opToken.getValue() + " " + expr1.outputIBTL(tabCount) );

            if (expr2 != null)
            {
                sb.Append(" " + expr2.outputIBTL(tabCount));
            }

            sb.Append(" ] ");

            return sb.ToString();
        }

        public override string getReturnType()
        {
            if (opToken.getValue() == "=" ||
                opToken.getValue() == ">" ||
                opToken.getValue() == "<" ||
                opToken.getValue() == "<=" ||
                opToken.getValue() == ">=" ||
                opToken.getValue() == "!=" ||
                opToken.getValue() == "and" ||
                opToken.getValue() == "or" ) return "bool";

            if (opToken.getValue() == "sin" ||
                opToken.getValue() == "cos" ||
                opToken.getValue() == "tan") return "float";
            

            if (expr2 != null)
            {
                if ((expr1.getReturnType() == "string" || expr2.getReturnType() == "string")
                    && opToken.getValue() == "+")
                    return "string";

                if (expr1.getReturnType() == "float" || expr2.getReturnType() == "float")
                    return "float";

                if (expr1.getReturnType() == "int" || expr2.getReturnType() == "int")
                    return "int";

                
                throw new Exception("OpNode error, reuturn type know know." + opToken.locate());

            } return expr1.getReturnType();
        }

        public override string outputC(int tabCount)
        {
            throw new NotImplementedException();
        }
    }
}
