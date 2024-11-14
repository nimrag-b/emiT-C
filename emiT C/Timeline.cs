using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public class Timeline
    {
        public Multiverse multiverse;
        public Dictionary<string, eVariable> variables;
        public Dictionary<string, eTime> times;

        public IEnumerator Enumerator;

        public CodeBlockStmt rootContext;

        public CodeBlockStmt context;

        public int depth;

        /// <summary>
        /// unstable timelines will collapse.
        /// </summary>
        public bool Unstable;


        public Timeline(Multiverse multiverse, Dictionary<string, eVariable> variables, Dictionary<string, eTime> times, CodeBlockStmt rootContext, int curTimeIndex, int depth)
        {
            this.multiverse = multiverse;
            this.variables = variables;
            this.times = times;
            this.rootContext = rootContext;
            rootContext.CurTimeIndex = curTimeIndex;
            this.depth = depth;
        }

        public Timeline Branch(eTime time)
        {
            return new Timeline(multiverse,time.variables, time.times, time.rootContext, time.SavedTimeIndex, depth+1);
        }

        public void RecalculateTimeIndexes(int point, int length)
        {
            foreach (var ind in times)
            {
                if(ind.Value.SavedTimeIndex > point)
                {
                    times[ind.Key].SavedTimeIndex = ind.Value.SavedTimeIndex + length;
                }
            }
        }

        public void CreateTimeFrame(string name)
        {
            eTime time = new eTime(
                variables.ToDictionary(entry => entry.Key, entry => (eVariable)entry.Value.Clone()),
                times.ToDictionary(entry => entry.Key, entry => (eTime)entry.Value.Clone()), //doesnt need to be a deep copy since eTimes are essentially immutable
                new CodeBlockStmt(rootContext.codeblock.ToList()), //can be even shallower copy
                rootContext.CurTimeIndex
                );
            times[name] = time;
        }

        public eTime? GetTime(string name)
        {
            if(times.ContainsKey(name)) return times[name];
            return null;
        }

        public void CreateVariable(string varName)
        {
            if (variables.ContainsKey(varName))
            {
                if (!variables[varName].Alive)
                {
                    variables[varName].Alive = true;
                    return;
                }
                CreateParadox($"{varName} met itself in the past, two of the same variables cannot exist at the same time.");
            }
            else
            {

                variables.Add(varName, new eVariable());
            }

        }

        public eVariable? GetActualVariable(string varName)
        {
            if (variables.ContainsKey(varName))
            {
                return variables[varName];
            }
            return null;
        }

        public eValue? GetVariable(string varName)
        {
            if (variables.ContainsKey(varName))
            {
                if (variables[varName].Alive)
                {
                    return variables[varName].value;
                }
            }
            return null;
        }

        public void KillVariable(string varName)
        {
            if (variables.ContainsKey(varName))
            {
                variables[varName].Alive = false;
            }
        }

        public void SetVariable(string varName, eValue value)
        {
            if (variables.ContainsKey(varName))
            {
                if (variables[varName].Alive)
                {
                    variables[varName].value = value;
                }

            }
        }

        public void SetVariableIndex(string varName, eValue value, int index)
        {
            if (variables.ContainsKey(varName))
            {
                if (variables[varName].Alive)
                {
                    ((eArray)(variables[varName].value.value)).inner[index] = value;
                }

            }
        }
        public Statement Peek()
        {
            return context.codeblock[context.CurTimeIndex+1];
        }
        public Statement At()
        {
            return context.codeblock[context.CurTimeIndex];
        }

        public void CreateParadox(string paradox)
        {
            Console.WriteLine("Paradox Created: "+paradox);
            Destabilize();
        }

        public void Destabilize()
        {
            Unstable = true;
        }

        public void SetEnumerator()
        {
            Enumerator = rootContext.Evaluate(this).GetEnumerator();
        }

        public void Run()
        {
            Console.WriteLine("Starting");

            rootContext.Evaluate(this);

            //while(CurTimeIndex < rootContext.codeblock.Count && !Unstable)
            //{
            //    eValue val = At().Evaluate(this);
            //    //Console.WriteLine(CurTimeIndex + "::" + At() + "-> "+ val);
            //    CurTimeIndex++;;
            //}
            //Console.WriteLine("Timeline Collapsed");
        }
    }
}
