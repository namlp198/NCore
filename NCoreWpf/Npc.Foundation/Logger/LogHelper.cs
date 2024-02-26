using NpcCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Npc.Foundation.Logger
{
    public static class LogHelper
    {
        private static LoggerWrapper generalLogger = new LoggerWrapper(null);
        private static Dictionary<string, LoggerWrapper> loggers = new Dictionary<string, LoggerWrapper>();

        private static readonly List<string> ignoreExceptions = new List<string>() { nameof(TaskCanceledException) };

        #region Fatal/Debug/Info/Warn log methods

        public static void ChangeLogLevel(string machineName, LogLevel level)
        {
            if (string.IsNullOrEmpty(machineName))
                return;

            LoggerWrapper logger = GetLogger(machineName);
            logger.Level = level;
        }

        /// <summary>
        /// Remove the logger by machine name.
        /// </summary>
        /// <param name="machineName">Name of the machine.</param>
        public static void Resign(string machineName)
        {
            if (string.IsNullOrEmpty(machineName))
                return;

            if (loggers.TryGetValue(machineName, out LoggerWrapper logger))
                logger.Dispose();

            loggers.Remove(machineName);
        }

        public static void Fatal(Exception exception)
        {
            if (ignoreExceptions.Contains(exception.GetType().Name))
                return;

            Log(LogLevel.Fatal, exception, exception.Message);
        }

        public static void Fatal(Exception exception, string messageTemplate)
        {
            Log(LogLevel.Fatal, exception, messageTemplate);
        }

        public static void Fatal(string messageTemplate, params object[] propertyValues)
        {
            Log(LogLevel.Fatal, messageTemplate, propertyValues);
        }

        public static void Fatal(string messageTemplate)
        {
            Log(LogLevel.Fatal, messageTemplate);
        }

        public static void Debug(Exception exception, string messageTemplate)
        {
            Log(LogLevel.Debug, exception, messageTemplate);
        }

        public static void Debug(string messageTemplate, params object[] propertyValues)
        {
            Log(LogLevel.Debug, messageTemplate, propertyValues);
        }

        public static void Debug(string messageTemplate)
        {
            Log(LogLevel.Debug, messageTemplate);
        }

        public static void Info(Exception exception, string messageTemplate)
        {
            Log(LogLevel.Info, exception, messageTemplate);
        }

        public static void Info(string messageTemplate, params object[] propertyValues)
        {
            Log(LogLevel.Info, messageTemplate, propertyValues);
        }

        public static void Info(string messageTemplate)
        {
            Log(LogLevel.Info, messageTemplate);
        }

        public static void Warn(Exception exception, string messageTemplate)
        {
            Log(LogLevel.Warn, exception, messageTemplate);
        }

        public static void Warn(string messageTemplate, params object[] propertyValues)
        {
            Log(LogLevel.Warn, messageTemplate, propertyValues);
        }

        public static void Warn(string messageTemplate)
        {
            Log(LogLevel.Warn, messageTemplate);
        }

        private static void Log(LogLevel logLevelRequest, Exception exception, string messageTemplate)
        {
            GetContextLogger().Log(logLevelRequest, exception, messageTemplate);
        }

        private static void Log(LogLevel logLevelRequest, string messageTemplate, params object[] propertyValues)
        {
            GetContextLogger().Log(logLevelRequest, messageTemplate, propertyValues);
        }

        private static void Log(LogLevel logLevelRequest, string message)
        {
            GetContextLogger().Log(logLevelRequest, message);
        }

        /// <summary>
        /// This method is to detect the current machine tab which user is staying on, then return a appropriate logger for the tab
        /// Incase user is not staying on any machine tab, then shared general log will be returned
        /// </summary>
        /// <returns>A logger</returns>
        private static LoggerWrapper GetContextLogger()
        {
            try
            {
                if (!IsCurrentUIThread())
                    return generalLogger;

                var focusElement = Keyboard.FocusedElement;
                if (focusElement == null)
                    return generalLogger;

                DependencyObject tab = FindCurrentMachineTab(focusElement as DependencyObject);
                if (tab == null)
                    return generalLogger;

                var tabViewModel = tab.GetPropertyValue("DataContext");
                if (tabViewModel == null || string.IsNullOrEmpty(tabViewModel.ToString()))
                    return generalLogger;
                LoggerWrapper logger = GetLogger(tabViewModel.ToString());
                logger.Level = LogLevel.Fatal;

                return logger;
            }
            catch (Exception ex)
            {
                generalLogger.Log(LogLevel.Fatal, ex, "Error occured while initialize log.");
                return generalLogger;
            }
        }

        private static DependencyObject FindCurrentMachineTab(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            if (parent == null)
            {
                return null;
            }
            else
            {
                string a = parent.GetType().FullName;
                if (parent.GetType().FullName.Equals("System.Windows.Controls.Grid"))
                    return parent;

                return FindCurrentMachineTab(parent);
            }
        }

        public static bool IsCurrentUIThread()
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA &&
                !Thread.CurrentThread.IsBackground && !Thread.CurrentThread.IsThreadPoolThread && Thread.CurrentThread.IsAlive)
            {
                MethodInfo correctEntryMethod = Assembly.GetEntryAssembly().EntryPoint;
                StackTrace trace = new StackTrace();
                StackFrame[] frames = trace.GetFrames();
                for (int i = frames.Length - 1; i >= 0; i--)
                {
                    MethodBase method = frames[i].GetMethod();
                    if (correctEntryMethod == method)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static LoggerWrapper GetLogger(string machineName)
        {
            if (!loggers.TryGetValue(machineName, out LoggerWrapper logger))
            {
                logger = new LoggerWrapper(machineName);
                loggers.Add(machineName, logger);
            }

            return logger;
        }

        #endregion Fatal/Debug/Info/Warn log methods
    }
}
