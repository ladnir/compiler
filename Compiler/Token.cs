using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    [FlagsAttribute]
    enum DataType : byte { INT, REAL, BOOL, STRING, REF, OP, KEYWORD, BRACE ,SEMICOLON};

    abstract class Token
    {
        protected DataType type;
        protected string value;
        protected bool literal;

        public Token(string p)
        {
            value = p;
            literal = true;
        }
        public String getValue() { return value; }
        public DataType getType() { return type; }
        public bool isLiteral() { return literal; }

        public virtual String toString() { 
            return " <_  " + type.ToString() +
                   " , " + value + " _> ";
        }
    }

    class MyInt : Token
    {
        public MyInt(string p) : base(p)
        {
            type = DataType.INT;
        }

    }
    class MyReal : Token
    {
        public MyReal(string p) : base(p)
        {
            type = DataType.REAL;
        }
    }
    class MyBoolean : Token
    {
        
        public MyBoolean(string p)  : base(p)
        {
            type = DataType.BOOL;
        }
        
    }
    class MyString : Token
    {
        public MyString(string p) : base(p)
        {
            type = DataType.STRING;
        }
    }
    class MyOperator : Token
    {
        public MyOperator(string p) : base(p)
        {
            type = DataType.OP;
            literal = false;
        }
    }
    class Reference : Token
    {
        public Reference(string p) : base(p)
        {
            type = DataType.REF;
            literal = false;
        }
    }
    class KeyWord : Token
    {
        public KeyWord(string p) : base(p)
        {
            if (!SymbolTable.isKeyWord(p)) throw new Exception("Not a Keyword");
            type = DataType.KEYWORD;
            literal = false;
        }

    }
    class Brace : Token
    {
        public Brace(char brace) : base(""+brace)
        {
            type = DataType.BRACE;
            literal = false;
        }

    }
    class SemiColon : Token
    {
        public SemiColon() : base(";")
        {
           type = DataType.SEMICOLON;
           literal = false;
        }
    }
}
