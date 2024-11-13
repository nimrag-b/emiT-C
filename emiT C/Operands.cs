using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public enum Operand
    {
        //binary
        Add,
        Subtract, 
        Multiply, 
        Divide,
        Modulus,

        //boolean
        Equals,
        Greater,
        Less,
        LessOrEqual,
        GreaterOrEqual
    }

    public enum Type : byte
    {
        Null,
        Int,
        Float,
        String,
        Bool
    }
}
