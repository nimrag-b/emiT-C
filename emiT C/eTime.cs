using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public class eTime
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
    }
}
