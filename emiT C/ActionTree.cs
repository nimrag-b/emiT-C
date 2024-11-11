using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public abstract class Statement
    {
        public abstract eValue Evaluate(Timeline t);
    }

    public abstract class Expression : Statement
    {
    }

    public class CreateTime : Statement
    {
        public string varName;

        public CreateTime(string varName)
        {
            this.varName = varName;
        }

        public override eValue Evaluate(Timeline t)
        {
            t.CreateTimeFrame(varName);
            return null;
        }
    }

    public class KillsStmt : Statement
    {
        public ExprVariable killer;
        public ExprVariable victim;

        public KillsStmt(ExprVariable killer, ExprVariable victim)
        {
            this.killer = killer;
            this.victim = victim;
        }
        public override eValue Evaluate(Timeline t)
        {
            eVariable? var =  t.GetActualVariable(killer.varName);
            if(var != null && var.Alive)
            {
                t.KillVariable(victim.varName);
            }
            return null;
        }
        public override string ToString()
        {
            return $"{killer} kills {victim}";
        }
    }

    public class CreateStmt : Statement
    {
        public string varName;
        public Expression? value;

        public CreateStmt(string varName, Expression? value)
        {
            this.varName = varName;
            this.value = value;
        }

        public override eValue Evaluate(Timeline t)
        {
            t.CreateVariable(varName);
            if(value != null)
            {
                eValue stat = value.Evaluate(t);
                t.SetVariable(varName, stat);
                return stat;
            }
            return null;
        }
        public override string ToString()
        {
            if(value != null)
            {
                return $"create {varName} ({value.ToString})";
            }
            return $"create {varName}";
        }
    }

    public class AssignmentStmt : Statement
    {
        public ExprVariable varName;
        public Expression right;

        public AssignmentStmt(ExprVariable varName, Expression right)
        {
            this.varName = varName;
            this.right = right;
        }

        public override eValue Evaluate(Timeline t)
        {
            eValue r = right.Evaluate(t);
            t.SetVariable(varName.varName, r);
            return r;
        }
        public override string ToString()
        {
            return $"{varName} = {right.ToString()}";
        }
    }
    public abstract class ExprBool : Expression
    {
    }
    public class IsStmt : ExprBool
    {
        public ExprVariable varName;
        public VarProperty property;

        public IsStmt(ExprVariable varName, VarProperty property)
        {
            this.varName = varName;
            this.property = property;
        }

        public override eValue Evaluate(Timeline t)
        {
            return Evaluator.EvaluateIsStmt(this, t);
        }

        public override string ToString()
        {
            return $"{varName} is {property}";
        }
    }

    public class BranchStmt: Statement
    {
        public string timeName;
        public ExprVariable traveler;

        public List<Statement> statements;

        public BranchStmt(string timeName, ExprVariable traveler, List<Statement> statements)
        {
            this.timeName = timeName;
            this.traveler = traveler;
            this.statements = statements;
        }

        public override eValue Evaluate(Timeline t)
        {
            eTime? time = t.GetTime(timeName);
            if(time == null)
            {
                throw new Exception($"Point {timeName} does not exist at this point in time.");
            }

            Timeline newtimeline = t.Branch(time);
            newtimeline.statements.InsertRange(time.SavedTimeIndex, statements); //add the time travellers actions to the new timeline
            newtimeline.CreateVariable(traveler.varName);
            newtimeline.SetVariable(traveler.varName,traveler.Evaluate(t));
            //TODO: add it so that the traveller can wait until something else to do something

            t.Timelines += newtimeline.Run(); //start the new timeline
            return null;
        }
    }

    public class PrintStmt : Statement
    {
        public Expression contents;

        public PrintStmt(Expression contents)
        {
            this.contents = contents;
        }

        public override eValue Evaluate(Timeline t)
        {
            eValue val = contents.Evaluate(t);
            if(val != null)
            {
                Console.WriteLine(contents.Evaluate(t).ToString());
            }
            return val;
        }
    }

    public class IfStmt : Statement
    {
        public ExprBool condition;
        public List<Statement> codeblock;

        public IfStmt(ExprBool condition, List<Statement> codeblock)
        {
            this.condition = condition;
            this.codeblock = codeblock;
        }

        public override eValue Evaluate(Timeline t)
        {
            eValue cond = condition.Evaluate(t);

            if ((bool)cond.value)
            {
                foreach (var item in codeblock)
                {
                    item.Evaluate(t);
                }
            }
            return cond;
        }

        public override string ToString()
        {
            return $"if ({condition})";
        }
    }

    public class UnaryExpr : Expression
    {
        public Expression right;
        public Operand op;

        public UnaryExpr(Expression right, Operand op)
        {
            this.right = right;
            this.op = op;
        }

        public override eValue Evaluate(Timeline t)
        {
            return Evaluator.EvaluateUnaryExpr(this,t);
        }

        public override string ToString()
        {
            return $"({op.ToString()} {right.ToString()})";
        }
    }

    public class BinaryExpr : Expression
    {
        public Expression left;
        public Expression right;
        public Operand op;

        public BinaryExpr(Expression left, Expression right, Operand op)
        {
            this.left = left;
            this.right = right;
            this.op = op;
        }

        public override eValue Evaluate(Timeline t)
        {
            return Evaluator.EvaluateBinaryExpr(this, t);
        }

        public override string ToString()
        {
            return $"({op.ToString()} {left.ToString()} {right.ToString()})";
        }
    }

    public class BooleanExpr : ExprBool
    {
        public Expression left;
        public Expression right;
        public Operand op;

        public BooleanExpr(Expression left, Expression right, Operand op)
        {
            this.left = left;
            this.right = right;
            this.op = op;
        }

        public override eValue Evaluate(Timeline t)
        {
            return Evaluator.EvaluateBooleanExpr(this, t);
        }
        public override string ToString()
        {
            return $"({op.ToString()} {left.ToString()} {right.ToString()})";
        }
    }
    public class NotBoolExpr : Expression
    {
        public Expression left;

        public NotBoolExpr(Expression left)
        {
            this.left = left;
        }

        public override eValue Evaluate(Timeline t)
        {
            eValue val = left.Evaluate(t);
            val.value = !((bool)val.value);
            return val;
        }
        public override string ToString()
        {
            return $"(not {left.ToString()})";
        }
    }
    public class ExprVariable : Expression
    {
        public string varName;

        public ExprVariable(string varName)
        {
            this.varName = varName;
        }

        public override eValue Evaluate(Timeline t)
        {
            if (t.variables.ContainsKey(varName))
            {
                if (t.variables[varName].Alive)
                {
                    return t.variables[varName].value;
                }
            }
            return null;
        }
        public override string ToString()
        {
            return $"{varName}";
        }
    }
    public class ExprLiteral : Expression
    {
        public object value;
        public Type type;

        public ExprLiteral(object value, Type type)
        {
            this.value = value;
            this.type = type;
        }

        public override eValue Evaluate(Timeline t)
        {
            return new eValue(type, value);
        }
        public override string ToString()
        {
            return $"{type.ToString()} {value}";
        }
    }
}
