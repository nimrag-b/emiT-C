using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public struct eTime : ICloneable
    {
        public Dictionary<string, eVariable> variables;
        public Dictionary<string, eTime> times;
        public CodeBlockStmt rootContext;
        public int SavedTimeIndex;

        public eTime(Dictionary<string, eVariable> variables, Dictionary<string, eTime> times, CodeBlockStmt rootContext, int savedTimeIndex)
        {
            this.variables = variables;
            this.times = times;
            this.rootContext = rootContext;
            SavedTimeIndex = savedTimeIndex;
        }

        public object Clone()
        {
            return new eTime(
                variables.ToDictionary(entry => entry.Key, entry => (eVariable)entry.Value.Clone()),
                times.ToDictionary(entry => entry.Key, entry => entry.Value), //doesnt need to be a deep copy since eTimes are essentially immutable
                new CodeBlockStmt(rootContext.codeblock.ToList()), //can be even shallower copy
                rootContext.CurTimeIndex
                );
        }
    }
}
