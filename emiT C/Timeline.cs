using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public class Timeline
    {
        public Dictionary<string, eVariable> variables;
        public Dictionary<string, eTime> times;

        public List<Statement> statements;

        public int CurTimeIndex;

        public int Timelines;

        /// <summary>
        /// unstable timelines will collapse.
        /// </summary>
        public bool Unstable;

        public Timeline(Dictionary<string, eVariable> variables, Dictionary<string, eTime> times,List<Statement> statements, int curTimeIndex)
        {
            this.variables = variables;
            this.times = times;
            this.statements = statements;
            CurTimeIndex = curTimeIndex;
        }

        public Timeline Branch(eTime time)
        {
            return new Timeline(time.variables, time.times, time.statements, time.SavedTimeIndex);
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
                new List<Statement>(statements), //can be even shallower copy
                CurTimeIndex
                );
            times.Add(name, time);
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

        public void CreateParadox(string paradox)
        {
            Console.WriteLine("Paradox Created: "+paradox);
            Destabilize();
        }

        public void Destabilize()
        {
            Unstable = true;
        }

        public int Run()
        {
            Timelines++;
            while(CurTimeIndex < statements.Count && !Unstable)
            {
                eValue val = statements[CurTimeIndex].Evaluate(this);
                //Console.WriteLine(CurTimeIndex + "::" + statements[CurTimeIndex] + "-> "+ val);
                CurTimeIndex++;;
            }
            //Console.WriteLine("Timeline Collapsed");
            return Timelines;
        }

    }
}
