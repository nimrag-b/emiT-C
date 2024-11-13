using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public class eVariable: ICloneable
    {
        public bool Alive = true;

        public eValue value;

        public eVariable()
        {

        }

        public eVariable(eValue value)
        {
            this.value = value;
        }

        public object Clone()
        {
            return new eVariable(value);
        }

        public override string ToString()
        {
            return value.ToString() + "-> " + Alive;
        }
    }

    public struct eValue
    {
        public Type type;

        public object value;

        public static eValue Null => new eValue(Type.Null, 0);

        public eValue(Type type, object value)
        {
            this.type = type;
            this.value = value;
        }

        public override string ToString()
        {
            if(type == Type.Null)
            {
                return "Null";
            }
            return value.ToString();
        }
    }

    public class eArray : ICloneable
    {
        public Type type;
        public eValue[] inner;

        public int Length => inner.Length;

        public eArray(Type type, int length)
        {
            this.type= type;
            inner = new eValue[length];
        }

        public eArray(Type type, eValue[] array)
        {
            this.type = type;
            inner = array;
        }


        public object Clone()
        {
            eArray other = new eArray(type, (eValue[])inner.Clone());
            return other;
        }
    }

    public enum VarProperty
    {
        Dead,
        Alive,
        Exists,
    }
}
