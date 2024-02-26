using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Logger
{
    public static class LogHub
    {
        public static ILog Logger { get; private set; }
        public static void SetLogger(ILog logger)
        {
            Logger = logger;
        }
        public static void Write(object message)
        {
            if (Logger != null && message != null)
            {
                Logger.Write(message.ToString());
            }
        }
        public static void Write(string message)
        {
            if (Logger != null && message != null)
            {
                Logger.Write(message);
            }
        }
        public static void Write(string format, params object[] args)
        {
            if (Logger != null && format != null)
            {
                Logger.Write(string.Format(format, args));
            }
        }
        public static void Write(Exception ex)
        {
            if (Logger != null && ex != null)
            {
                Logger.Write(ex);
            }
        }
        public static void Write(string message, LogTypes logType)
        {
            if (Logger != null && message != null)
            {
                Logger.Write(message, logType);
            }
        }
        public static void Write(string format, LogTypes logType, params object[] args)
        {
            if (Logger != null && format != null)
            {
                Logger.Write(string.Format(format, args), logType);
            }
        }
        public static void Write(Exception ex, LogTypes logType)
        {
            if (Logger != null && ex != null)
            {
                Logger.Write(ex, logType);
            }
        }
        public static void Write(Exception ex, string message, LogTypes logType)
        {
            if (Logger != null && ex != null)
            {
                Logger.Write(ex, message, logType);
            }
        }
    }
}
