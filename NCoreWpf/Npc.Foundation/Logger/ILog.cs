using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Logger
{
    public interface ILog
    {
        void GenerateLogger();
        void Write(string message, LogTypes logType = LogTypes.NpcInfo);
        void Write(Exception ex, LogTypes logType = LogTypes.NpcInfo);
        void Write(Exception ex, string message, LogTypes logType = LogTypes.NpcInfo);
    }
    public enum LogTypes
    {
        NpcInfo,
        NpcFatal,
        NpcGUI,
        NpcDBConnector,
        NpcDebug
    }
}
