using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public static class StdLib
    {
        public static string GetString(eValue expr)
        {
            switch (expr.type)
            { //capture special types that dont have an innate value
                case Type.Null:
                    return "null";
                case Type.Array:
                    return GetArrayString((eArray)expr.value);
                    
            }

            return expr.ToString();
        }

        public static string GetArrayString(eArray array)
        {
            switch (array.type)
            {
                case Type.Char:
                    return GetCharArrayString(array);
            }

            return array.ToString();
        }

        public static string GetCharArrayString(eArray array)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in array.inner)
            {
                sb.Append((char)item.value);
            }
            return sb.ToString();
        }
    }
}
