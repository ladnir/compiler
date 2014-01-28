using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    [FlagsAttribute]
    public enum TokenType : byte { INT, REAL, BOOL, STRING, REF, OP, KEYWORD, BRACE ,SEMICOLON, COMMA};

    abstract public class Token
    {
        protected int line, num;
        protected TokenType type;
        protected string value;
        protected bool literal;

        public Token(string p, int line, int num)
        {
            this.line = line;
            this.num = num;
            value = p;
            literal = true;
        }
        public String getValue() { return value; }
        public TokenType getType() { return type; }
        public bool isLiteral() { return literal; }

        public virtual String toString() { 
            return " " + type.ToString() +
                   " \t " + value ;
        }

        internal string locate()
        {
            return "  line>"+line + ", token>" + num;
        }
    }

    class MyComma : Token
    {
        public MyComma( int line, int num)
            : base(",", line, num)
        {
            type = TokenType.COMMA;
        }

    }
    class MyInt : Token
    {
        public MyInt(string p, int line, int num)
            : base(p, line, num)
        {
            type = TokenType.INT;
        }

    }
    class MyReal : Token
    {
        public MyReal(string p, int line, int num)
            : base(p, line, num)
        {
            type = TokenType.REAL;
        }
    }
    class MyBoolean : Token
    {

        public MyBoolean(string p, int line, int num)
            : base(p, line, num)
        {
            type = TokenType.BOOL;
        }
        
    }
    class MyString : Token
    {
        public MyString(string p, int line, int num)
            : base(p, line, num)
        {
            type = TokenType.STRING;
        }
    }
    class MyOperator : Token
    {
        public MyOperator(string p, int line, int num)
            : base(p, line, num)
        {
            type = TokenType.OP;
            literal = false;
        }
    }
    class Reference : Token
    {
        public Reference(string p, int line, int num)
            : base(p, line, num)
        {
            type = TokenType.REF;
            literal = false;
        }
    }
    class KeyWord : Token
    {
        public KeyWord(string p, int line, int num)
            : base(p, line, num)
        {
            if (!SymbolTable.isKeyWord(p)) throw new Exception("Not a Keyword");
            type = TokenType.KEYWORD;
            literal = false;
        }

    }
    class Brace : Token
    {
        public Brace(char brace, int line, int num)
            : base("" + brace, line, num)
        {
            type = TokenType.BRACE;
            literal = false;
        }

    }
    class MySemiColon : Token
    {
        public MySemiColon(int line, int num)
            : base(";", line, num)
        {
           type = TokenType.SEMICOLON;
           literal = false;
        }
    }
}
