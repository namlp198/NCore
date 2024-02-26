using Npc.Foundation.Define;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Events
{
    public class LogViewEvent : PubSubEvent<LogViewEventArgs>
    {
    }

    public class LogViewEventArgs
    {
        public LogViewEventTypes Command;

        public bool IsShow { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }

        public LogViewEventArgs()
        {
        }
    }
}
