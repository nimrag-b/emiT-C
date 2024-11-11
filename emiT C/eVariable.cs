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

        public eValue value = new eValueNull();

        public eVariable()
        {

        }

        public eVariable(eValue value)
        {
            this.value = value;
        }

        public object Clone()
        {
            return new eVariable((eValue)value.Clone());
        }

        public override string ToString()
        {
            return value.ToString() + "-> " + Alive;
        }
    }

    public class eValueNull : eValue
    {
        public eValueNull() : base(Type.Int, 0)
        {
        }

        public object Clone() //may break with non reference object types. oh well! shouldnt happen anyway
        {
            return new eValueNull();
        }
    }

    public class eValue : ICloneable
    {
        public Type type;

        public object value;

        public eValue(Type type, object value)
        {
            this.type = type;
            this.value = value;
        }

        public object Clone() //may break with non reference object types. oh well! shouldnt happen anyway
        {
            return new eValue(type, value);
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    public enum VarProperty
    {
        Dead,
        Alive,
        Exists,
    }
}
