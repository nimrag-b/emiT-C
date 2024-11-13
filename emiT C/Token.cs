using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public struct Token
    {
        public string symbol;
        public TokenType type;
        public object value;
        public int line;
        public Token(string symbol, TokenType type, object value, int line)
        {
            this.symbol = symbol;
            this.type = type;
            this.value = value;
            this.line = line;
        }

        public override string ToString()
        {
            return symbol + "::" + type;
        }
    }

    public enum TokenType
    {
        Symbol,
        CharLiteral,
        IntLiteral,
        FloatLiteral,
        StringLiteral,
        BoolLiteralTrue,
        BoolLiteralFalse,

        Type,

        //Brackets

        OpenBracket,
        CloseBracket,
        OpenParen,
        CloseParen,

        //Semicolon
        SemiColon,

        //Operators

        //One/Two char tokens
        Assign, // =
        Not, // !

        Add, // +
        Subtract, // -
        Multiply, // *
        Divide, // /
        Modulus, // %

        Equals, // ==
        NotEquals, // !=
        Greater, // <
        Less, // >
        GreaterOrEqual, // <=
        LessOrEqual, // >=



        //Keywords

        If,
        Is, // - innate properties about a variable, if it alive etc
        Collapse, // - similar to return, but collapses the current timeline;
        Create,
        Kills,
        Time,
        Warps,
        Waits,
        Dead,
        Alive,
        Exists,

        Print,

        EOF
    }
}
