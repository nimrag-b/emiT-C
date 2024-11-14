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

        //TODO: fix it so that it doesnt capture code from before the branch point - as it means the call list keeps having to be resized
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
                    //times[ind.Key].SavedTimeIndex = ind.Value.SavedTimeIndex + length;
                }
            }
        }

        public void CreateTimeFrame(string name)
        {
            eTime time = new eTime(
                variables.ToDictionary(entry => entry.Key, entry => (eVariable)entry.Value.Clone()),
                times.ToDictionary(entry => entry.Key, entry => entry.Value), //doesnt need to be a deep copy since eTimes are essentially immutable
                new CodeBlockStmt(rootContext.codeblock.GetRange(rootContext.CurTimeIndex,rootContext.codeblock.Count-rootContext.CurTimeIndex)), //can be even shallower copy
                0
                );
            times[name] = time;
        }

        public eTime? GetTime(string name)
        {
            if(times.ContainsKey(name)) return times[name];
            return null;
        }

        public void CreateAltVariable(string varName)
        {
            if (variables.ContainsKey(varName))
            {
                variables[varName].AddVariable(eValue.Null);
                //CreateParadox($"{varName} met itself in the past, two of the same variables cannot exist at the same time.");
            }
            else
            {
                variables.Add(varName, new eVariable());
            }

            SwitchActiveVariable(varName, variables[varName].Values.Count - 1);
        }

        public void CreateVariable(string varName)
        {
            CreateAltVariable(varName);
            return;
            if (variables.ContainsKey(varName))
            {
                if (!variables[varName].Alive)
                {
                    variables[varName].SetAlive(true);
                    return;
                }
                CreateParadox($"{varName} already defined.If you want to redefine, you must time travel.");
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
                variables[varName].SetAlive(false);
            }
        }

        public void SetVariable(string varName, eValue value)
        {
            if (variables.ContainsKey(varName))
            {
                if (variables[varName].Alive)
                {
                    variables[varName].SetVariable(value);
                }

            }
        }

        public void SwitchActiveVariable(string varName, int index)
        {
            if (variables.ContainsKey(varName))
            {
                if (variables[varName].Alive)
                {
                    variables[varName].SetPointer(index);
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
