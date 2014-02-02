using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class Tokenizer
    {
        static string[] Functions = { "sin", "cos"," tan", "stdout", "not", "and" , "or", "^"};
        static string[] dataTypes = { "int", "bool", "float", "string" };
        static string[] constructs = { "while", "if", "let" , "return" }; // for

        int index = 0;
        int length,count=0;
        int line = 1,num=1;
        string source;
        
        public Token[] GetTokens(String p)
        {
            source = p;
            length = source.Length;

            LinkedList<Token> list = new LinkedList<Token>();
            while (index < length)
            {
                Token token = getNextToken();
                if (token != null)
                {
                    list.AddLast(token);
                   // Console.WriteLine("token " + count++ + ":" + token.getValue());
                }
            }


            return list.ToArray() ;
        }

        private Token getNextToken()
        {
            Token token;

            while (isWhiteSpace())
            {
                if (source[index].Equals('\n'))
                {
                    num = 1;
                    line++;
                }
                index++;
                if (index >= length) return null;
            }

            if (isComment())
            {
                skipComment();
                return null;
            }
            else if (isNumber(source[index]))   token = getNumber();
            else if (assignment(source[index])) token = getAssignment();
            else if (isQuote(source[index]))    token = getString();
            else if (isOperator())              token = getOperator();
            else if (isLetter(source[index]))   token = getWord();
            else if (isBrace(source[index]))    token = getBrace();
            else throw new Exception("unknow char at line " + line + " , token " + (num + 1) +" >"+source[index]+"< ");

            num++;
            return token;
        }

        private bool isWhiteSpace()
        {
            return (source[index] == ' ' ||
                source[index].Equals('\r') ||
                source[index].Equals('\n') ||
                source[index].Equals('\t'));
        }

        private Token getAssignment()
        {
            if (source[index++] != ':')
                throw new Exception("error t5: expecting a := at line " + line + " , token " + num+1);
            if (source[index++] != '=')
                throw new Exception("error t6: expecting a := at line " + line + " , token " + num+1);

            return new AssignmentToken( line, num++);
            
        }



        private void skipComment()
        {
            index++;
            if (source[index] == '*')  // multi line comment
            {
                index++;
                while (index+1<length && source[index] != '*' && source[index + 1] != '/')
                {
                    if (source[index] == '\n')
                    {
                        num = 1;
                        line++;
                    }
                    index = index ++;
                }
            }
            else // inline comment
            {
                while (index  < length && source[index] != '\n') index++;

                line++;
                num = 1;
            }
        }


        private Token getString()
        {
            char q = source[index];
            StringBuilder sb = new StringBuilder();
            index++;

            while (index < length && source[index] != q) sb.Append(source[index++]);

            if (index >= length)
                throw new Exception("error t3: end of file while reading a string. add closing quote");
            index++;

            return new StringToken(sb.ToString(), line, num);
        }

        private Token getBrace()
        {
            return new BraceToken(source[index++], line, num);
        }

        private Token getWord()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(source[index]);
            index++;

            while (index < length && (isNumber(source[index]) || isLetter(source[index] ) ))
                sb.Append(source[index++]);

            string word = sb.ToString();

            if (isBool(word))       return new BooleanToken(word, line, num);
            if (isDataType(word))   return new DataTypeToken(word, line, num);
            if (isConstruct(word))  return new ConstructToken(word, line, num);
            if (isFunction(word))   return new FunctionToken(word, line, num);

            return new ReferenceToken(word, line, num);
        }
        
        private Token getOperator()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(source[index]);
            index++;
            if (index < length)
            {
                switch (sb.ToString())
                {
                    case "=":
                        if (source[index] == '=') sb.Append(source[index++]);
                        break;
                    case "!":
                        if (source[index] == '=') sb.Append(source[index++]);
                        break;
                    case "<":
                        if (source[index] == '=') sb.Append(source[index++]);
                        break;
                    case ">":
                        if (source[index] == '=') sb.Append(source[index++]);
                        break;
                    case "+":
                        if (source[index] == '+') sb.Append(source[index++]);
                        break;
                    case "-":
                        if (source[index] == '-') sb.Append(source[index++]);
                        break;
                    default:
                        break;
                }
            }
            return new OperatorToken(sb.ToString(), line, num);
        }

        private Token getNumber()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(source[index]);
            index++;

            while (index < length && isNumber(source[index])) sb.Append(source[index++]);

            if (source[index] == '.')
            {
                sb.Append(source[index++]);
                while (index < length && isNumber(source[index])) sb.Append(source[index++]);

                FloatToken real = new FloatToken(sb.ToString(), line, num);

                if (index < length && isLetter(source[index]))
                    throw new Exception("error t2 at token:" + real.locate());
                return real;
            }
            else
            {
                IntToken mi = new IntToken(sb.ToString(), line, num);
                if (index < length && isLetter(source[index]))
                   throw new Exception("error t1 at token:" + mi.locate() );
                return mi;
            }
        }

        private bool isFunction(string word)
        {
            return isInSet(word, Functions);
        }

        private bool isConstruct(string word)
        {
            return isInSet(word, constructs);
        }

        private bool isDataType(string word)
        {
            return isInSet(word, dataTypes);
        }

        private bool isComma(char p)
        {
            if (source[index] == ',') return true;
            return false;
        }

        private bool isComment()
        {
            if (source[index] == '/' && index +1 < length &&(source[index+1] == '/' ||source[index+1] == '*') ) 
                return true;
            return false;
        }

        private bool assignment(char p)
        {
            if (p == ':') return true;

            return false;
        }

        private bool isSemiColon(char p)
        {
            if (p == ';') return true;
            return false;
        }

        private bool isBool(string word)
        {
            if (word == "true" || word == "false") return true;
            return false;
        }

        private bool isQuote(char p)
        {
            if ( p == '"') return true;
            return false;
        }

        private bool isBrace(char p)
        {
            char[] braces = { '[', ']' };
            return isInSet(p, braces);
        }

        private bool isLetter(char p)
        {
            if ((p >= 'a' && p <= 'z') || (p >= 'A' && p <= 'Z') || p == '_') return true;
            return false;
        }

        private bool isNumber(char p)
        {
            if (p >= '0' && p <= '9')  return true;
            return false;
        }

        private bool isOperator()
        {
            // or, and, not, sin, cos, tan   will be keywords not operators

            char[] operators = { '+','~', '-', '/', '*', '^', '%', '=', '!', '<', '>' };

            if( isInSet(source[index], operators)) return true;

            return false;
        }

        private bool isDeliminator(char p)
        {
            char[] deliminators = {' ','~',';',',','\t' ,'&','|', '+','-', '/','*',
                                      '%','=','!', '<', '>','{','}','[',']' };

            return isInSet(p, deliminators);
        }

        private bool isInSet(char p, char[] types)
        {

            foreach (char c in types)
            {
                if (c == p) return true;
            }
            return false;
        }
        private bool isInSet(string token, string[] set)
        {
            foreach (string s in set)
            {
                if (s == token) return true;
            }
            return false;
        }
    }
}
