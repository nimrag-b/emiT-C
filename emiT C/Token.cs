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
        public Token(string symbol, TokenType type)
        {
            this.symbol = symbol;
            this.type = type;
        }

        public override string ToString()
        {
            return symbol + " : " + type;
        }
    }

    public enum TokenType
    {
        Symbol,
        Int,
        Float,
        String,
        Bool,

        Type,

        If,
        Is, // - innate properties about a variable, if it alive etc
        Collapse, // - similar to return, but collapses the current timeline;

        OpenBracket,
        CloseBracket,
        OpenParen,
        CloseParen,

        SemiColon,

        Assign,
        BooleanOp,
        BinaryOp,


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
