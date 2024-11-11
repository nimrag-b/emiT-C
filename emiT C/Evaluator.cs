using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public static class Evaluator
    {

        public static eValue EvaluateBinaryExpr(BinaryExpr binaryExpr, Timeline t)
        {
            eValue left = binaryExpr.left.Evaluate(t);

            switch (left.type)
            {
                case Type.Int:
                    return EvaluateIntBinaryExpr(binaryExpr, t, left);
                case Type.Float:
                    return EvaluateFloatBinaryExpr(binaryExpr, t, left);
                default:
                    throw new NotImplementedException();
            }
        }

        static eValue EvaluateIntBinaryExpr(BinaryExpr binaryExpr, Timeline t, eValue left)
        {
            eValue right = binaryExpr.right.Evaluate(t);
            switch (binaryExpr.op)
            {
                case Operand.Add:
                    return new eValue(Type.Int, (int)left.value + (int)right.value);
                case Operand.Subtract:
                    return new eValue(Type.Int, (int)left.value - (int)right.value);
                case Operand.Multiply:
                    return new eValue(Type.Int, (int)left.value * (int)right.value);
                case Operand.Divide:
                    return new eValue(Type.Int, (int)left.value / (int)right.value);
                default:
                    throw new NotImplementedException();
            }
        }
        static eValue EvaluateFloatBinaryExpr(BinaryExpr binaryExpr, Timeline t, eValue left)
        {
            eValue right = binaryExpr.right.Evaluate(t);
            switch (binaryExpr.op)
            {
                case Operand.Add:
                    return new eValue(Type.Float, (float)left.value + (float)right.value);
                case Operand.Subtract:
                    return new eValue(Type.Float, (float)left.value - (float)right.value);
                case Operand.Multiply:
                    return new eValue(Type.Float, (float)left.value * (float)right.value);
                case Operand.Divide:
                    return new eValue(Type.Float, (float)left.value / (float)right.value);
                default:
                    throw new NotImplementedException();
            }
        }

        public static eValue EvaluateUnaryExpr(UnaryExpr unaryExpr, Timeline t)
        {
            eValue right = unaryExpr.right.Evaluate(t);

            switch (right.type)
            {
                case Type.Int:
                    return EvaluateIntUnaryExpr(right,unaryExpr.op, t);
                case Type.Float:
                    return EvaluateFloatUnaryExpr(right, unaryExpr.op, t);
                default:
                    throw new NotImplementedException();
            }
        }
        public static eValue EvaluateIntUnaryExpr(eValue right, Operand op, Timeline t)
        {
            switch (op)
            {
                case Operand.Add:
                    return new eValue(Type.Int, +(int)right.value);
                case Operand.Subtract:
                    return new eValue(Type.Int, -(int)right.value);
                default:
                    throw new NotImplementedException();
            }
        }

        public static eValue EvaluateFloatUnaryExpr(eValue right, Operand op, Timeline t)
        {
            switch (op)
            {
                case Operand.Add:
                    return new eValue(Type.Float, +(float)right.value);
                case Operand.Subtract:
                    return new eValue(Type.Float, -(float)right.value);
                default:
                    throw new NotImplementedException();
            }
        }

        public static eValue EvaluateBooleanExpr(BooleanExpr booleanExpr, Timeline t)
        {
            eValue left = booleanExpr.left.Evaluate(t);

            switch (left.type)
            {
                case Type.Int:
                    return EvaluateIntBooleanExpr(booleanExpr, t, left);
                case Type.Float:
                    return EvaluateFloatBooleanExpr(booleanExpr, t, left);
                default:
                    throw new NotImplementedException();
            }
        }

        public static eValue EvaluateIntBooleanExpr(BooleanExpr booleanExpr, Timeline t, eValue left)
        {
            eValue right = booleanExpr.right.Evaluate(t);
            switch (booleanExpr.op)
            {
                case Operand.Equals:
                    return new eValue(Type.Bool, (int)left.value == (int)right.value);
                case Operand.Less:
                    return new eValue(Type.Bool, (int)left.value > (int)right.value);
                case Operand.Greater:
                    return new eValue(Type.Bool, (int)left.value < (int)right.value);
                default:
                    throw new NotImplementedException();
            }
        }

        public static eValue EvaluateFloatBooleanExpr(BooleanExpr booleanExpr, Timeline t, eValue left)
        {
            eValue right = booleanExpr.right.Evaluate(t);
            switch (booleanExpr.op)
            {
                case Operand.Equals:
                    return new eValue(Type.Bool, (float)left.value == (float)right.value);
                case Operand.Less:
                    return new eValue(Type.Bool, (float)left.value > (float)right.value);
                case Operand.Greater:
                    return new eValue(Type.Bool, (float)left.value < (float)right.value);
                default:
                    throw new NotImplementedException();
            }
        }

        public static eValue EvaluateIsStmt(IsStmt stmt, Timeline t)
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
                    return new eValue(Type.Bool, !var.Alive);
                case VarProperty.Alive:
                    return new eValue(Type.Bool, var.Alive);
                case VarProperty.Exists:
                    return new eValue(Type.Bool, true);
            }

            throw new Exception("Unknown Property: "+ stmt.property);
        }

    }
}
