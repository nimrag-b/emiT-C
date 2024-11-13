using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public abstract class Statement
    {
        public abstract IEnumerable Evaluate(Timeline t);
    }

    /// <summary>
    /// A statement that does an action, ie captures the next block. does not always need to be ended with a semicolon
    /// </summary>
    public interface IActorStmt
    {
    }

    public abstract class Expression
    {
        public abstract eValue Evaluate(Timeline t);
    }

    public class CreateTime : Statement
    {
        public string varName;

        public CreateTime(string varName)
        {
            this.varName = varName;
        }

        public override IEnumerable Evaluate(Timeline t)
        {
            t.CreateTimeFrame(varName);
            yield return null;
        }
        public override string ToString()
        {
            return $"time {varName}";
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
        public override IEnumerable Evaluate(Timeline t)
        {
            eVariable? var =  t.GetActualVariable(killer.varName);
            if (var != null && var.Alive)
            {
                t.KillVariable(victim.varName);
            }
            yield return null;
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

        public override IEnumerable Evaluate(Timeline t)
        {

            t.CreateVariable(varName);
            if(value != null)
            {
                eValue stat = value.Evaluate(t);
                t.SetVariable(varName, stat);
                yield return stat;
            }
            yield return null;
        }
        public override string ToString()
        {
            if(value != null)
            {
                return $"create {varName} ({value})";
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

        public override IEnumerable Evaluate(Timeline t)
        {
            eValue r = right.Evaluate(t);
            t.SetVariable(varName.varName, r);
            yield return null;
        }
        public override string ToString()
        {
            return $"{varName} = {right.ToString()}";
        }
    }
    public abstract class ExprBool : Expression
    {
    }
    public class IsExpr : ExprBool
    {
        public ExprVariable varName;
        public VarProperty property;

        public IsExpr(ExprVariable varName, VarProperty property)
        {
            this.varName = varName;
            this.property = property;
        }

        public override eValue Evaluate(Timeline t)
        {
             return Evaluator.EvaluateIsExpr(this, t);
        }

        public override string ToString()
        {
            return $"{varName} is {property}";
        }
    }

    public class BranchStmt: Statement, IActorStmt
    {
        public string timeName;
        public ExprVariable traveler;

        public BranchStmt(string timeName, ExprVariable traveler)
        {
            this.timeName = timeName;
            this.traveler = traveler;
        }

        public override IEnumerable Evaluate(Timeline t)
        {
            eTime? time = t.GetTime(timeName);
            if(time == null)
            {
                throw new Exception($"Point {timeName} does not exist at this point in time.");
            }

            Timeline newtimeline = t.Branch(time);

            Statement action = t.Peek(); //get the next statement
            if(action == null)
            {
                throw new Exception($"No statement found to branch from");
            }

            newtimeline.rootContext.codeblock.Insert(time.SavedTimeIndex+1, action); //add the time travellers actions to the new timeline
            newtimeline.rootContext.codeblock.Insert(time.SavedTimeIndex+1, new CreateStmt(traveler.varName, traveler)); //create the traveller in the new timeline
            //TODO: add it so that the traveller can wait until something else to do somethings
            newtimeline.RecalculateTimeIndexes(time.SavedTimeIndex+1, 1);

            //Console.WriteLine("New Timeline:");
            //foreach (Statement v in newtimeline.rootContext.codeblock)
            //{
            //    Console.WriteLine(v);
            //}
            t.multiverse.ChangeTimeline(newtimeline);
            //t.Timelines +=
            //newtimeline.Run(); //start the new timeline
            yield return null;
        }
    }

    public class PrintStmt : Statement
    {
        public Expression contents;

        public PrintStmt(Expression contents)
        {
            this.contents = contents;
        }

        public override IEnumerable Evaluate(Timeline t)
        {
            eValue val = contents.Evaluate(t);
            string str = val.ToString();
            if(val.type != Type.Null)
            {
                Console.WriteLine(str);
            }
            yield return null;
        }
    }

    public class IfStmt : Statement, IActorStmt
    {
        public ExprBool condition;

        public IfStmt(ExprBool condition)
        {
            this.condition = condition;
        }

        public override IEnumerable Evaluate(Timeline t)
        {
            eValue cond = condition.Evaluate(t);

            if (!(bool)cond.value) //if condution is false, skip next statement
            {
                t.context.CurTimeIndex++;
            }
            yield return null;
        }

        public override string ToString()
        {
            StringBuilder cond = new StringBuilder($"if ({condition})");
            return cond.ToString();
        }
    }

    public class CollapseStmt : Statement
    {
        public override IEnumerable Evaluate(Timeline t)
        {
            t.Destabilize();
            yield return null;
        }
    }

    public class CodeBlockStmt : Statement
    {
        public List<Statement> codeblock;

        public int CurTimeIndex = 0; //this is causing the problem, basically its getting stuck at the execution point, and the statement is not copies to the next timeline, only referenced, which is fine for every other statement but this one. also, the timeline needs a way to navigate exactly back to the point it is at befpre.

        public CodeBlockStmt(List<Statement> codeblock)
        {
            this.codeblock= codeblock;
        }
        public override IEnumerable Evaluate(Timeline t)
        {
            int InitialTimeIndex = CurTimeIndex; //saved context time for jumps
            //CurTimeIndex = 0;
            CodeBlockStmt savedContext = t.context;

            t.context = this;
            //Console.WriteLine("    Entering Block with count " + codeblock.Count);
            //Console.WriteLine("    CurTime: " + CurTimeIndex);

            while (CurTimeIndex < codeblock.Count && !t.Unstable)
            {
                //Console.WriteLine($"    {CurTimeIndex}::{codeblock[CurTimeIndex]}");
                IEnumerator block =  codeblock[CurTimeIndex].Evaluate(t).GetEnumerator();
                while(block.MoveNext())
                {
                    yield return null;
                }
                CurTimeIndex++;
                //Console.WriteLine("    " + CurTimeIndex + "::" + codeblock[CurTimeIndex] + "-> " + val);
            }
            //Console.WriteLine("    Exiting Block");
            CurTimeIndex = InitialTimeIndex;
            t.context = savedContext;
        }
        public override string ToString()
        {
            StringBuilder cond = new StringBuilder();
            cond.AppendLine("\n    [");
            foreach (var item in codeblock)
            {
                cond.AppendLine("        "+ item.ToString());
            }
            cond.AppendLine("    ]");
            return cond.ToString();
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
            return eValue.Null;
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
