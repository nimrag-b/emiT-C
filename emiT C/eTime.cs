using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public class eTime: ICloneable
    {
        public Dictionary<string, eVariable> variables;
        public Dictionary<string, eTime> times;
        public List<Statement> statements;
        public int SavedTimeIndex;

        public eTime(Dictionary<string, eVariable> variables, Dictionary<string, eTime> times, List<Statement> statements, int savedTimeIndex)
        {
            this.variables = variables;
            this.times = times;
            this.statements = statements;
            SavedTimeIndex = savedTimeIndex;
        }

        public object Clone()
        {
            eTime time = new eTime(
                variables.ToDictionary(entry => entry.Key, entry => (eVariable)entry.Value.Clone()),
                times.ToDictionary(entry => entry.Key, entry => (eTime)entry.Value.Clone()), //doesnt need to be a deep copy since eTimes are essentially immutable
                new List<Statement>(statements), //can be even shallower copy
                SavedTimeIndex
                );
            return time;
        }
    }
}
