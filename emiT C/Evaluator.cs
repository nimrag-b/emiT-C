using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public static class Evaluator
    {

        public static eValue EvaluateBinaryExpr(BinaryExpr binaryExpr, Timeline t)
        {
            eValue left = binaryExpr.left.Evaluate(t);

            if (left.value == null)
            {
                throw new Exception("Error: cannot operate Null value ");
            }

            switch (left.type)
            {
                case Type.Int:
                    return EvaluateGenericBinaryExpr<int>(Type.Int,binaryExpr, t, left);
                case Type.Float:
                    return EvaluateGenericBinaryExpr<float>(Type.Float, binaryExpr, t, left);
                default:
                    throw new NotImplementedException();
            }
        }
        static eValue EvaluateGenericBinaryExpr<T>(Type type, BinaryExpr binaryExpr, Timeline t, eValue left) where T : INumber<T>
        {
            eValue right = binaryExpr.right.Evaluate(t);
            switch (binaryExpr.op)
            {
                case Operand.Add:
                    return new eValue(type, (T)left.value + (T)right.value);
                case Operand.Subtract:
                    return new eValue(type, (T)left.value - (T)right.value);
                case Operand.Multiply:
                    return new eValue(type, (T)left.value * (T)right.value);
                case Operand.Divide:
                    return new eValue(type, (T)left.value / (T)right.value);
                case Operand.Modulus:
                    return new eValue(type, (T)left.value % (T)right.value);
                default:
                    throw new NotImplementedException();
            }
        }

        public static eValue EvaluateUnaryExpr(UnaryExpr unaryExpr, Timeline t)
        {
            eValue right = unaryExpr.right.Evaluate(t);

            if (right.value == null)
            {
                throw new Exception("Error: cannot operate Null value ");
            }

            switch (right.type)
            {
                case Type.Int:
                    return EvaluateGenericUnaryExpr<int>(Type.Int, right,unaryExpr.op, t);
                case Type.Float:
                    return EvaluateGenericUnaryExpr<float>(Type.Float, right, unaryExpr.op, t);
                default:
                    throw new NotImplementedException();
            }
        }
        public static eValue EvaluateGenericUnaryExpr<T>(Type type, eValue right, Operand op, Timeline t) where T : INumber<T>
        {
            switch (op)
            {
                case Operand.Add:
                    return new eValue(type, +(T)right.value);
                case Operand.Subtract:
                    return new eValue(type, -(T)right.value);
                default:
                    throw new NotImplementedException();
            }
        }

        public static eValue EvaluateBooleanExpr(BooleanExpr booleanExpr, Timeline t)
        {
            eValue left = booleanExpr.left.Evaluate(t);

            if(left.value == null)
            {
                throw new Exception("Error: cannot compare Null value ");
            }

            switch (left.type)
            {
                case Type.Int:
                    return EvaluateGenericBooleanExpr<int>(booleanExpr, t, left);
                case Type.Float:
                    return EvaluateGenericBooleanExpr<float>(booleanExpr, t, left);
                default:
                    throw new NotImplementedException();
            }
        }

        public static eValue EvaluateGenericBooleanExpr<T>(BooleanExpr booleanExpr, Timeline t, eValue left) where T : IComparable<T>
        {
            eValue right = booleanExpr.right.Evaluate(t);
            switch (booleanExpr.op)
            {
                case Operand.Equals:
                    return new eValue(Type.Bool, ((T)left.value).Equals((T)right.value));
                case Operand.Less:
                    return new eValue(Type.Bool, ((T)left.value).CompareTo((T)right.value) < 0);
                case Operand.Greater:
                    return new eValue(Type.Bool, ((T)left.value).CompareTo((T)right.value) > 0);
                case Operand.LessOrEqual:
                    return new eValue(Type.Bool, ((T)left.value).CompareTo((T)right.value) <= 0);
                case Operand.GreaterOrEqual:
                    return new eValue(Type.Bool, ((T)left.value).CompareTo((T)right.value) >= 0);
                default:
                    throw new NotImplementedException();
            }
        }

        public static eValue EvaluateIsExpr(IsExpr stmt, Timeline t)
        {
            eVariable? var = t.GetActualVariable(stmt.varName.varName);
            if (var == null)
            {
                if(stmt.property == VarProperty.Exists)
                {
                    return new eValue(Type.Bool, false);
                }

                t.CreateParadox($"{stmt.varName} has never existed in this timeline, and so cannot be dead or alive");
                return new eValue(Type.Bool, false);
            }
            switch (stmt.property)
            {
                case VarProperty.Dead:
                    return new eValue(Type.Bool, !var.Value.Alive);
                case VarProperty.Alive:
                    return new eValue(Type.Bool, var.Value.Alive);
                case VarProperty.Exists:
                    return new eValue(Type.Bool, true);
            }

            throw new Exception("Unknown Property: "+ stmt.property);
        }

        public static void EvaluateVarShift(VarShiftExpr expr, Timeline t)
        {
            eVariable? var = t.GetActualVariable(expr.varName);
            int amount = (int)expr.amount.Evaluate(t).value;
            if (var == null)
            {
                throw new Exception();
            }
            switch (expr.op)
            {
                case Operand.ShiftForward:
                    var.Value.SetPointer(var.Value.ValuePointer + amount);
                    return;
                case Operand.ShiftBack:
                    var.Value.SetPointer(var.Value.ValuePointer - amount);
                    return;
                case Operand.SetForward:
                    var.Value.SetPointer(amount);
                    return;
                case Operand.SetBack:
                    var.Value.SetPointer(var.Value.Values.Count - 1 - amount);
                    return;

            }
            throw new NotImplementedException();
        }

    }
}
