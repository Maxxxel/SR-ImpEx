using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR_ImpEx.LogViewer
{
    public class LogEntry : PropertyChangedBase
    {
        public string DateTime { get; set; }
        public int Index { get; set; }
        public string Message { get; set; }
    }

    public class CollapsibleLogEntry : LogEntry
    {
        public List<LogEntry> Contents { get; set; }
    }
}
