using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    [FlagsAttribute]
    public enum TokenType : byte { INT, FLOAT, BOOL, STRING, REF, OP, DATATYPE, CONSTRUCT, FUNCTION, BRACE,  ASSIGNMENT };

    abstract public class Token
    {
        protected int line, num;
        protected TokenType type;
        protected string value;
        protected bool literal =true;

        public Token(string p, int line, int num)
        {
            this.line = line;
            this.num = num;
            value = p;
        }
        public String getValue() { return value; }
        public TokenType getTokenType() { return type; }
        public bool isLiteral() { return literal; }

        public virtual String toString() { 
            return " " + type.ToString() +
                   " \t " + value ;
        }

        internal string locate()
        {
            return "  line>"+line + ", token>" + num +"  # "+value;
        }
    }

    class AssignmentToken : Token
    {
        public AssignmentToken(int line, int num)
            : base(":=", line, num)
        {
            type = TokenType.ASSIGNMENT;
            literal = false;
        }

    }

    class DataTypeToken : Token
    {
        public DataTypeToken(string p, int line, int num)
            : base(p, line, num)
        {
            type = TokenType.DATATYPE;
            literal = false;
        }

    }
    class ConstructToken : Token
    {
        public ConstructToken(string p, int line, int num)
            : base(p, line, num)
        {
            type = TokenType.CONSTRUCT;
            literal = false;
        }

    } 
    class FunctionToken : Token
    {
        public FunctionToken(string p, int line, int num)
            : base(p, line, num)
        {
            type = TokenType.FUNCTION;
            literal = false;
        }

    }
    class IntToken : Token
    {
        public IntToken(string p, int line, int num)
            : base(p, line, num)
        {
            type = TokenType.INT;
        }

    }
    class FloatToken : Token
    {
        public FloatToken(string p, int line, int num)
            : base(p, line, num)
        {
            type = TokenType.FLOAT;
        }
    }
    class BooleanToken : Token
    {

        public BooleanToken(string p, int line, int num)
            : base(p, line, num)
        {
            type = TokenType.BOOL;
        }
        
    }
    class StringToken : Token
    {
        public StringToken(string p, int line, int num)
            : base(p, line, num)
        {
            type = TokenType.STRING;
        }
    }
    class OperatorToken : Token
    {
        public OperatorToken(string p, int line, int num)
            : base(p, line, num)
        {
            type = TokenType.OP;
            literal = false;
        }
    }
    class ReferenceToken : Token
    {
        public ReferenceToken(string p, int line, int num)
            : base(p, line, num)
        {
            type = TokenType.REF;
            literal = false;
        }
    }
    class BraceToken : Token
    {
        public BraceToken(char brace, int line, int num)
            : base("" + brace, line, num)
        {
            type = TokenType.BRACE;
            literal = false;
        }

    }

}
