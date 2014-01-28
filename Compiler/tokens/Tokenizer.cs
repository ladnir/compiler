using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class Tokenizer
    {

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
                    Console.WriteLine("token " + count++ + ":" + token.getValue());
                }
            }


            return list.ToArray() ;
        }

        private Token getNextToken()
        {
            Token token;

            while (source[index] == ' ' || source[index] == '\n')
            {
                if (source[index] == '\n')
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
            }else if (isNumber(source[index])) token = getNumber();
            else if (isQuote(source[index])) token = getString();
            else if (isOperator(source[index])) token = getOperator();
            else if (isLetter(source[index])) token = getWord();
            else if (isBrace(source[index])) token = getBrace();
            else if (isSemiColon(source[index])) token = getSemiColon();
            else if (isComma(source[index])) token = getComma();
            else throw new Exception("unknow char: " + source[index]);

            num++;
            return token;
        }

        private Token getComma()
        {
            index++;
            return new MyComma(line,num);
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

        private Token getSemiColon()
        {
            index++;
            return new MySemiColon(line,num);
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

            return new MyString(sb.ToString(), line, num);
        }

        private Token getBrace()
        {
            return new Brace(source[index++], line, num);
        }

        private Token getWord()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(source[index]);
            index++;

            while (index < length && (isNumber(source[index]) || isLetter(source[index] ) ))
                sb.Append(source[index++]);

            string word = sb.ToString();
            if (isBool(word)) return new MyBoolean(word, line, num);
            if (SymbolTable.isKeyWord(word)) return new KeyWord(word, line, num);

            return new Reference(word, line, num);
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
            return new MyOperator(sb.ToString(), line, num);
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

                MyReal real = new MyReal(sb.ToString(), line, num);

                if (index < length && isLetter(source[index]))
                    throw new Exception("error t2 at token:" + real.locate());
                return real;
            }
            else
            {
                MyInt mi = new MyInt(sb.ToString(), line, num);
                if (index < length && isLetter(source[index]))
                   throw new Exception("error t1 at token:" + mi.locate() );
                return mi;
            }
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
            if (p == '\'' || p == '"') return true;
            return false;
        }

        private bool isBrace(char p)
        {
            char[] braces = { '{', '}', '[', ']', '(', ')' };
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

        private bool isOperator(char p)
        {
            char[] operators = { '+', '-', '/', '*', '&', '|','!', '%', '=', '<', '>' };

            return isInSet(p, operators);
        }

        private bool isDeliminator(char p)
        {
            char[] deliminators = {' ',';',',','\t' ,'&','|', '+','-', '/','*',
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
    }
}
