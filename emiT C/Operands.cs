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

        //boolean
        Equals,
        Greater,
        Less,
    }

    public enum Type
    {
        Int,
        Float,
        Bool
    }
}
