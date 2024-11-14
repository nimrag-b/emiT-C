using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public struct eVariable: ICloneable
    {

        public List<eValueState> Values = new List<eValueState>();

        public int ValuePointer = 0;

        public eValue value => Values[ValuePointer].Value;

        public bool Alive => Values[ValuePointer].Alive;

        public void SetAlive(bool value)
        {
            eValueState state = Values[ValuePointer];
            state.Alive = value;
            Values[ValuePointer] = state;
        }

        public eVariable()
        {
            AddVariable(eValue.Null);
        }

        public eVariable(eValue value)
        {
            AddVariable(value);
        }

        public void AddVariable(eValue value)
        {
            Values.Add(new eValueState(value));
        }

        public void SetVariable(eValue eValue)
        {
            Values[ValuePointer] = new eValueState(eValue);
        }

        public void SetPointer(int pointer)
        {
            ValuePointer = pointer;
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

    public struct eValueState
    {
        public eValue Value;

        public bool Alive;

        public eValueState(eValue Value, bool Alive = true)
        {
            this.Value = Value;
            this.Alive = Alive;
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
