﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class Tokenizer
    {
        int index = 0;
        int length;
        int count = 0;
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

            while (source[index] == ' ')
            {
                index++;
                if (index >= length) return null;
            }

            if (isNumber(source[index])) token = getNumber();
            else if (isQuote(source[index])) token = getString();
            else if (isOperator(source[index])) token = getOperator();
            else if (isLetter(source[index])) token = getWord();
            else if (isBrace(source[index])) token = getBrace();
            else if (isSemiColon(source[index])) token = getSemiColon();
            else if (isComment(source[index]))
            {
                skipComment();
                return null;
            }
            else throw new Exception("aw shit:" + source[index]);


            return token;
        }

        private void skipComment()
        {
            index++;
            if (source[index] == '*')  // multi line comment
            {
                index++;
                while (source[index] != '*' && source[index + 1] != '/') 
                    index++;
            }
            else // inline comment
            {
                while (source[index] != '\n') index++;
            }
        }

        private Token getSemiColon()
        {
            index++;
            return new SemiColon();
        }

        private Token getString()
        {
            char q = source[index];
            StringBuilder sb = new StringBuilder();
            index++;

            while (source[index] != q) sb.Append(source[index++]);

            index++;

            return new MyString(sb.ToString());
        }

        private Token getBrace()
        {
            return new Brace(source[index]);
        }

        private Token getWord()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(source[index]);
            index++;

            while (isNumber(source[index]) || isLetter(source[index] ) )
                sb.Append(source[index++]);

            string word = sb.ToString();
            if (isBool(word)) return new MyBoolean(word);
            if (SymbolTable.isKeyWord(word)) return new KeyWord(word);
            
            return new Reference(word);
        }

        private Token getOperator()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(source[index]);
            index++;

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

            return new MyOperator(sb.ToString());
        }

        private Token getNumber()
        {
            //Console.WriteLine("makking number");
            StringBuilder sb = new StringBuilder();
            sb.Append(source[index]);
            index++;

            while (isNumber(source[index])) sb.Append(source[index++]);

            if (source[index] == '.')
            {
                sb.Append(source[index++]);
                while (isNumber(source[index])) sb.Append(source[index++]);

                MyReal real = new MyReal(sb.ToString());
                return real;
            }
            else
            {
                return new MyInt(sb.ToString());
            }
        }

        private bool isComment(char p)
        {
            if (source[index] == '/') return true;
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
            if ((p >= 'a' && p <= 'z') || (p >= 'A' && p <= 'Z')) return true;
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