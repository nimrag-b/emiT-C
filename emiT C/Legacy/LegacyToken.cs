using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C.Legacy
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

        //Brackets

        OpenBracket,
        CloseBracket,
        OpenParen,
        CloseParen,

        //Semicolon
        SemiColon,

        //Operators LEGACY
        Assign, // =
        BooleanOp, // ==, !=, >, <, <=, >=
        BinaryOp, // +, -, *, /, %


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
