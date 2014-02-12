using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class EndOfTokensException : Exception
    { public EndOfTokensException(): base("End of file reached without balenced braces.") { } }


    public class Tokenizer
    {
        private Dictionary<string, BaseToken> symbols = new Dictionary<string, BaseToken>();

        static string[] Functions = { "sin", "cos"," tan" };
        static string[] dataTypes = { "int", "bool", "float", "string" };
        static string[] constructs = { "while", "if", "let" , "return","stdout" }; 

       // int index = 0 ;
       // int length;
        int line = 1,num=1;
        //string source;
        Token cur;
        private System.IO.StreamReader stream;

    
        //public Tokenizer(string source){
        //    this.source=source;
        //    length= source.Length;
        //    cur = getNextToken();
        //}

        public Tokenizer(System.IO.StreamReader stream)
        {
            this.stream = stream;
            
            cur = getNextToken();
        }
        public void printSymbolTable()
        {

            foreach (KeyValuePair<string, BaseToken> entry in symbols)
            {
                BaseToken t = entry.Value;
                Console.WriteLine(t.getValue() + "\t\t" + t.getTokenType());

            }
        }

        public Token peep()
        {
            if (cur == null)  throw new EndOfTokensException();
            return cur;
        }

        public Token pop()
        {
            Token r = cur;

            if ( cur == null)
            {
                //throw new Exception();
                throw new EndOfTokensException();
            }

            if (stream.EndOfStream) cur = null;
            else cur = getNextToken();
           
            return r;
        }

        private char charPeep()
        {
            return (char)stream.Peek(); //source[index];
        }

        private char charPop()
        {
            return (char)stream.Read(); //source[index++];
        }

        private bool endOfStream()
        {
            return stream.EndOfStream;
           
        }
        private Token getNextToken()
        {
            //Console.WriteLine("next");
            Token token;

            skipWhiteSpace();
            if (endOfStream()) return null;

            if (charPeep() == '/' && (token = skipComment()) != null) return token;
            else if (isWhiteSpace() || charPeep() == '/') return getNextToken();

            if (endOfStream()) return null;

            if (isNumber(charPeep()))           token = getNumber();
            else if (assignment(charPeep()))    token = getAssignment();
            else if (isQuote(charPeep()))       token = getString();
            else if (isOperator())              token = getOperator();
            else if (isLetter(charPeep()))      token = getWord();
            else if (isBrace(charPeep()))       token = getBrace();
            else throw new Exception("unknow char at line " + line + " , token " + (num + 1) + " >" + charPeep() + "< ");

            num++;

            return token;
        }

        private void skipWhiteSpace()
        {
            while (!endOfStream() && isWhiteSpace())
            {
                if (charPeep().Equals('\n'))
                {
                    num = 1;
                    line++;
                }
                charPop();
            }


        }

        private bool isWhiteSpace()
        {
            return (charPeep() == ' ' ||
                charPeep().Equals('\r') ||
                charPeep().Equals('\n') ||
                charPeep().Equals('\t'));
        }

        private Token getAssignment()
        {
            
            if (charPop() != ':') throw new Exception("expecting assignment 1 at line " + line);
            if (charPop() != '=') throw new Exception("expecting assignment 2 at line " + line);

            if (symbols.ContainsKey(":="))
                return new Token(symbols[":="], line, num);

            BaseToken ass = new AssignmentToken();
            Token tok = new Token(ass,line,num);
            symbols.Add(":=",ass );
            return tok;
            
            
        }



        private Token skipComment()
        {
            bool ending = false;

            charPop();
            if (charPeep() == '*')  // multi line comment
            {

                charPop();

                while ( ! (charPeep() == '/' && ending))
                {

                    if (charPeep() == '*') ending = true;
                    else ending = false;

                    if (charPeep() == '\n')
                    {
                        num = 1;
                        line++;
                    }
                    charPop();
                }
                charPop();
            }
            else if (charPeep() == '/') // inline comment
            {
                while ( ! endOfStream() && charPeep() != '\n') charPop();

                line++;
                num = 1;
            }
            else
            {  // OK nevermind, its just a /
                if (symbols.ContainsKey("/")) return new Token(symbols["/"],line,num++);

                BaseToken b = new OperatorToken("/");
                Token t = new Token(b, line, num++);
                symbols.Add("/", b);
            }
            return null;
        }


        private Token getString()
        {
            char q = charPeep();
            StringBuilder sb = new StringBuilder();
            charPop();

            while ( ! endOfStream() && charPeep() != q) sb.Append(charPop());

            if (endOfStream())
                throw new Exception("error t3: end of file while reading a string. add closing quote");

            charPop();

            string st = sb.ToString();

            if (symbols.ContainsKey(st))
                return new Token(symbols[st], line, num);
            
            BaseToken b = new StringToken(st);
            Token t= new Token(b,line,num);
            symbols.Add(st, b);
            return t;

        }

        private Token getBrace()
        {
            string brace = charPop()+"";
            if (symbols.ContainsKey(brace)) return new Token(symbols[brace], line, num);
            
            BaseToken b = new BraceToken(brace);
            Token t = new Token(b, line, num);
            symbols.Add(brace, b);
            return t;

        }

        private Token getWord()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(charPeep());
            BaseToken r;
            charPop();

            while ( ! endOfStream() && (isNumber(charPeep()) || isLetter(charPeep() ) ))
                sb.Append(charPop());

            string word = sb.ToString();

            if (symbols.ContainsKey(word)) return new Token(symbols[word],line,num);

            if (isBool(word))       r = new BooleanToken(word);
            else if (isDataType(word))   r = new DataTypeToken(word );
            else if (isConstruct(word))  r = new ConstructToken(word);
            else if (isFunction(word))   r = new OperatorToken(word );
            else                         r = new ReferenceToken(word);

            symbols.Add(word, r);
            return new Token( r,line,num);
        }
        
        private Token getOperator()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(charPeep());
            charPop();
            if ( ! endOfStream())
            {
                switch (sb.ToString())
                {
                    case "!":
                        if (charPeep() == '=') sb.Append(charPop());
                        break;
                    case "<":
                        if (charPeep() == '=') sb.Append(charPop());
                        break;
                    case ">":
                        if (charPeep() == '=') sb.Append(charPop());
                        break;
                  
                    default:
                        break;
                }
            }
            string op = sb.ToString();

            if (symbols.ContainsKey(op)) return new Token(symbols[op], line, num) ;


            BaseToken b = new OperatorToken(sb.ToString());
            Token t = new Token(b, line, num);
            symbols.Add(op, b);

            return t;
        }

        private Token getNumber()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(charPeep());
            charPop();

            string number;
            BaseToken b;

            while ( ! endOfStream() && isNumber(charPeep())) sb.Append(charPop());

            if (! endOfStream() && isFloat())
            {
                if (charPeep() == '.') getDot(sb);
                if ( ! endOfStream() && charPeep() == 'e') getE(sb);

                number = sb.ToString();
                b = new FloatToken(number);
               
            }
            else
            {
                number = sb.ToString();
                b = new IntToken(number);
            }

            if (symbols.ContainsKey(number)) return new Token(symbols[number], line, num);

            symbols.Add(number, b);
            return new Token(b, line, num);

        }

        private bool isFloat()
        {
            return (charPeep() == '.' || charPeep() == 'e');
        }

        private void getE(StringBuilder sb)
        {
            
            sb.Append(charPop());

            if (charPeep() == '-')
                sb.Append(charPop());

            while ( ! endOfStream() && isNumber(charPeep())) sb.Append(charPop());
        }

        private void getDot(StringBuilder sb)
        {
            sb.Append(charPop());
            while ( ! endOfStream() && isNumber(charPeep())) sb.Append(charPop());

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

        private bool assignment(char p)
        {
            if (p == ':') return true;

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

            char[] operators = { '+', '-', '/', '*', '^', '%', '=', '!', '<', '>' };

            if( isInSet(charPeep(), operators)) return true;

            return false;
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
