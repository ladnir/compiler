using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    [FlagsAttribute]
    public enum TokenType : byte { INT, FLOAT, BOOL, STRING, REF, OP, DATATYPE, CONSTRUCT, FUNCTION, BRACE,  ASSIGNMENT };

    abstract public class BaseToken
    {
         protected TokenType type;
        protected string value;
        protected bool literal =true;

        public BaseToken(string p)
        {
            value = p;
        }
        public String getValue() { return value; }
        public TokenType getTokenType() { return type; }
        public bool isLiteral() { return literal; }

        public virtual String toString() { 
            return " " + type.ToString() +
                   " \t " + value ;
        }

    }

    public class Token
    {
        protected int line, num;
        BaseToken baseToken;
        public Token(BaseToken baseToken,int line, int num)
        {
            this.baseToken = baseToken;
            this.line=line;
            this.num=num;
                      
        }
        public String getValue() { return baseToken.getValue(); }
        public TokenType getTokenType() { return baseToken.getTokenType(); }
        public bool isLiteral() { return baseToken.isLiteral(); }

        public virtual String toString()
        {
            return baseToken.toString();
        }

        internal string locate()
        {
            return "  line>" + line + ", token>" + num + "      "+baseToken.getValue();
        }
    }

    class AssignmentToken : BaseToken
    {
        public AssignmentToken()
            : base(":=")
        {
            type = TokenType.ASSIGNMENT;
            literal = false;
        }

    }

    class DataTypeToken : BaseToken
    {
        public DataTypeToken(string p)
            : base(p)
        {
            type = TokenType.DATATYPE;
            literal = false;
        }

    }
    class ConstructToken : BaseToken
    {
        public ConstructToken(string p)
            : base(p)
        {
            type = TokenType.CONSTRUCT;
            literal = false;
        }

    }
    class FunctionToken : BaseToken
    {
        public FunctionToken(string p)
            : base(p)
        {
            type = TokenType.FUNCTION;
            literal = false;
        }

    }
    class IntToken : BaseToken
    {
        public IntToken(string p)
            : base(p)
        {
            type = TokenType.INT;
        }

    }
    class FloatToken : BaseToken
    {
        public FloatToken(string p)
            : base(p)
        {
            type = TokenType.FLOAT;
        }
    }
    class BooleanToken : BaseToken
    {

        public BooleanToken(string p)
            : base(p)
        {
            type = TokenType.BOOL;
        }
        
    }
    class StringToken : BaseToken
    {
        public StringToken(string p)
            : base(p)
        {
            type = TokenType.STRING;
        }
    }
    class OperatorToken : BaseToken
    {
        public OperatorToken(string p)
            : base(p)
        {
            type = TokenType.OP;
            literal = false;
        }
    }
    class ReferenceToken : BaseToken
    {
        public ReferenceToken(string p)
            : base(p)
        {
            type = TokenType.REF;
            literal = false;
        }
    }
    class BraceToken : BaseToken
    {
        public BraceToken(string  brace)
            : base(brace)
        {
            type = TokenType.BRACE;
            literal = false;
        }

    }

}
