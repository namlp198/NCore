using NLog;
using Npc.Foundation.Define;
using Npc.Foundation.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Logger
{
    public class NLogger : ILog
    {
        const string DEF_INF = "[ INFO ]";
        const string DEF_FATAL = "[ FATAL ]";
        const string DEF_GUI = "[ GUI ]";
        const string DEF_DBC = "[ DB ]";
        const string DEF_DEB = "[ DEBUG ]";

        Dictionary<LogTypes, NLog.Logger> _loggers = new Dictionary<LogTypes, NLog.Logger>();

        public void GenerateLogger()
        {
            var logTypes = Enum.GetNames(typeof(LogTypes));
            foreach (var item in logTypes)
            {
                LogTypes logType = (LogTypes)Enum.Parse(typeof(LogTypes), item);

                if (this._loggers.ContainsKey(logType) == false)
                {
                    this._loggers.Add(logType, LogManager.GetLogger(item));
                }
            }
        }

        private NLog.Logger FindLogger(LogTypes logType)
        {
            return _loggers.Where(f => f.Key == logType).Select(f => f.Value).FirstOrDefault();
        }

        public void Write(string message, LogTypes logType = LogTypes.NpcInfo)
        {
            var logger = this.FindLogger(logType);
            if (logger != null)
            {
                switch (logType)
                {
                    case LogTypes.NpcInfo:
                        logger.Info($"{DEF_INF} {message}");
                        this.WriteLogViewPopup("INF", message);
                        break;
                    case LogTypes.NpcFatal:
                        logger.Error($"{DEF_FATAL} {message}");
                        this.WriteLogViewPopup("ERR", message);
                        break;
                    case LogTypes.NpcGUI:
                        logger.Info($"{DEF_GUI} {message}");
                        this.WriteLogViewPopup("COM", message);
                        break;
                    case LogTypes.NpcDBConnector:
                        logger.Info($"{DEF_DBC} {message}");
                        this.WriteLogViewPopup("DBC", message);
                        break;
                    case LogTypes.NpcDebug:
#if DEBUG
                        logger.Debug($"{DEF_DEB} {message}");
#endif
                        this.WriteLogViewPopup("DEB", message);
                        break;
                }
            }
        }

        public void Write(Exception ex, LogTypes logType = LogTypes.NpcInfo)
        {
            var logger = this.FindLogger(logType);
            var exLogger = this.FindLogger(LogTypes.NpcFatal);
            if (logger != null)
            {
                switch (logType)
                {
                    case LogTypes.NpcInfo:
                        logger.Error(ex, DEF_INF);
                        exLogger.Error(ex, DEF_FATAL);
                        this.WriteLogViewPopup("INF", ex);
                        break;
                    case LogTypes.NpcFatal:
                        logger.Error(ex, DEF_FATAL);
                        this.WriteLogViewPopup("ERR", ex);
                        break;
                    case LogTypes.NpcGUI:
                        logger.Error(ex, DEF_GUI);
                        exLogger.Error(ex, DEF_FATAL);
                        this.WriteLogViewPopup("COM", ex);
                        break;
                    case LogTypes.NpcDBConnector:
                        logger.Error(ex, DEF_DBC);
                        exLogger.Error(ex, DEF_FATAL);
                        this.WriteLogViewPopup("DBC", ex);
                        break;
                    case LogTypes.NpcDebug:
#if DEBUG
                        logger.Error(ex, DEF_DEB);
                        exLogger.Error(ex, DEF_FATAL);
#endif
                        this.WriteLogViewPopup("DEB", ex);
                        break;
                }
            }
        }

        public void Write(Exception ex, string message, LogTypes logType = LogTypes.NpcInfo)
        {
            // [NCS-2253]
            var logger = this.FindLogger(logType);
            var exLogger = this.FindLogger(LogTypes.NpcFatal);
            if (logger != null)
            {
                switch (logType)
                {
                    case LogTypes.NpcInfo:
                        logger.Info(ex, $"{DEF_INF} {message}");
                        exLogger.Error(ex, DEF_FATAL);
                        this.WriteLogViewPopup("INF", ex, message);
                        break;
                    case LogTypes.NpcFatal:
                        logger.Error(ex, $"{DEF_FATAL} {message}");
                        this.WriteLogViewPopup("ERR", ex, message);
                        break;
                    case LogTypes.NpcGUI:
                        logger.Info(ex, $"{DEF_GUI} {message}");
                        exLogger.Error(ex, DEF_FATAL);
                        this.WriteLogViewPopup("COM", ex, message);
                        break;
                    case LogTypes.NpcDBConnector:
                        logger.Info(ex, $"{DEF_DBC} {message}");
                        exLogger.Error(ex, DEF_FATAL);
                        this.WriteLogViewPopup("DBC", ex, message);
                        break;
                    case LogTypes.NpcDebug:
#if DEBUG
                        logger.Debug(ex, $"{DEF_DEB} {message}");
                        exLogger.Error(ex, DEF_FATAL);
#endif
                        this.WriteLogViewPopup("DEB", ex, message);
                        break;
                }
            }
        }

        public void WriteLogViewPopup(string level, string message)
        {
            if (FoundationEnvironment.Instance.IsShowLogView)
            {
                FoundationEventManager.RequestLogViewEvent(new LogViewEventArgs() { Command = LogViewEventTypes.WriteLog, Level = level, Message = string.Format("<{0}> [ {1} ] {2}", DateTime.Now.ToString("yyyyMMdd HH:mm:ss:fff"), level, message) });
            }
        }

        public void WriteLogViewPopup(string level, Exception ex)
        {
            if (FoundationEnvironment.Instance.IsShowLogView)
            {
                FoundationEventManager.RequestLogViewEvent(new LogViewEventArgs() { Command = LogViewEventTypes.WriteLog, Level = level, Message = string.Format("<{0}> [ {1} ] {2}", DateTime.Now.ToString("yyyyMMdd HH:mm:ss:fff"), level, ex.ToString()) });
            }
        }

        public void WriteLogViewPopup(string level, Exception ex, string message)
        {
            if (FoundationEnvironment.Instance.IsShowLogView)
            {
                FoundationEventManager.RequestLogViewEvent(new LogViewEventArgs() { Command = LogViewEventTypes.WriteLog, Level = level, Message = string.Format("<{0}> [ {1} ] {2} {3}", DateTime.Now.ToString("yyyyMMdd HH:mm:ss:fff"), level, message, ex.ToString()) });
            }
        }

        public void SetRulesTarget(string targetName, bool isUse)
        {
            var config = LogManager.Configuration;
            if (isUse == false)
            {
                foreach (var rule in config.LoggingRules)
                {
                    foreach (var target in rule.Targets.ToList())
                    {
                        if (target.Name == targetName)
                        {
                            rule.Targets.Remove(target);
                        }
                    }
                }
            }
            else
            {
                var nTarget = config.FindTargetByName(targetName);
                foreach (var rule in config.LoggingRules)
                {
                    if (rule.Targets.Any(x => x.Name == targetName) == false)
                    {
                        rule.Targets.Add(nTarget);
                    }
                }
            }
            LogManager.Configuration = config;
        }

        /// <summary>
        /// [NCS-4232]
        /// </summary>
        /// <param name="writeErrorLogOnly"></param>
        public void SetWriteErrorLogOnly(bool writeErrorLogOnly)
        {
            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                rule.DisableLoggingForLevels(NLog.LogLevel.Debug, NLog.LogLevel.Fatal);
                rule.EnableLoggingForLevels(writeErrorLogOnly ? NLog.LogLevel.Error : NLog.LogLevel.Debug, NLog.LogLevel.Fatal);
            }

            LogManager.ReconfigExistingLoggers();
        }
    }
}
