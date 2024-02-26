using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Define
{
    /// <summary>
    /// Application Type
    /// </summary>
    public enum ApplicationTypes
    {
        PcControl,
        Vision,
        Statistic,
        Iot
    }
    /// <summary>
    /// UserLevel List
    /// </summary>
    public enum UserLevelTypes
    {
        [Description("Auto Operator")]
        AutoOperator = 1,

        [Description("Operator")]
        Operator = 2,

        [Description("Engineer")]
        Engineer = 4,

        [Description("Supervisor")]
        Supervisor = 8,

        [Description("Npc")]
        KohYoung = 16,
    }
    /// <summary>
    /// Log View Event Type
    /// </summary>
    public enum LogViewEventTypes
    {
        ShowHide,
        WriteLog,
    }
}
