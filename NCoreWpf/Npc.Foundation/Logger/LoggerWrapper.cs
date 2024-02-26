using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Logger
{
    public enum LogLevel
    {
        None,
        Debug,
        Info,
        Warn,
        Fatal
    }
    internal class LoggerWrapper : IDisposable
    {
        private readonly string logFolder = string.IsNullOrEmpty(ConfigurationManager.AppSettings["logFolder"]) ? ".\\LogProfile" : ConfigurationManager.AppSettings["logFolder"];

        private readonly string _name;
        private string _day;
        private Serilog.Core.Logger _logger;

        internal Serilog.Core.Logger Logger
        {
            get
            {
                var today = DateTime.Now.ToString("yyyy-MM-dd");
                if (_logger == null || !string.Equals(_day, today))
                {
                    _day = today;

                    if (_logger != null)
                        _logger.Dispose();

                    _logger = CreateLogger(today);
                }

                return _logger;
            }
        }

        internal LogLevel Level { get; set; }

        #region Constructors
        public LoggerWrapper(string name, LogLevel level = LogLevel.Debug)
        {
            _name = name;
            Level = level;
        }
        #endregion

        public void Log(LogLevel logLevelRequest, Exception exception, string messageTemplate)
        {
            if (!CanSuportLog(logLevelRequest))
                return;

            switch (logLevelRequest)
            {
                case LogLevel.Fatal:
                    Logger.Fatal(exception, messageTemplate);
                    break;

                case LogLevel.Debug:
                    Logger.Debug(exception, messageTemplate);
                    break;

                case LogLevel.Info:
                    Logger.Information(exception, messageTemplate);
                    break;

                case LogLevel.Warn:
                    Logger.Warning(exception, messageTemplate);
                    break;
            }
        }

        public void Log(LogLevel logLevelRequest, string messageTemplate, params object[] propertyValues)
        {
            if (!CanSuportLog(logLevelRequest))
                return;

            switch (logLevelRequest)
            {
                case LogLevel.Fatal:
                    Logger.Fatal(messageTemplate, propertyValues);
                    break;

                case LogLevel.Debug:
                    Logger.Debug(messageTemplate, propertyValues);
                    break;

                case LogLevel.Info:
                    Logger.Information(messageTemplate, propertyValues);
                    break;

                case LogLevel.Warn:
                    Logger.Warning(messageTemplate, propertyValues);
                    break;
            }
        }

        public void Log(LogLevel logLevelRequest, string messageTemplate)
        {
            if (!CanSuportLog(logLevelRequest))
                return;

            switch (logLevelRequest)
            {
                case LogLevel.Fatal:
                    Logger.Fatal(messageTemplate);
                    break;

                case LogLevel.Debug:
                    Logger.Debug(messageTemplate);
                    break;

                case LogLevel.Info:
                    Logger.Information(messageTemplate);
                    break;

                case LogLevel.Warn:
                    Logger.Warning(messageTemplate);
                    break;
            }
        }

        private bool CanSuportLog(LogLevel logLevelRequest)
        {
            if (Level == LogLevel.None)
                return false;

            return Level <= logLevelRequest;
        }

        /// <summary>
        /// Create a logger.
        /// </summary>
        /// <returns>The logger.</returns>
        private Serilog.Core.Logger CreateLogger(string today)
        {
            var logger = new Serilog.LoggerConfiguration()
               .MinimumLevel.Verbose()
               .WriteTo.Console()
               .WriteTo.File(
                   path: string.IsNullOrEmpty(_name) ? string.Format("{0}\\{1}-Review.log", logFolder, today) :
                                                      string.Format("{0}\\{1}-{2}-Review.log", logFolder, today, _name),
                   fileSizeLimitBytes: 5120000,
                   rollOnFileSizeLimit: true,
                   flushToDiskInterval: TimeSpan.FromSeconds(1))
               .CreateLogger();

            return logger;
        }

        #region IDisposable

        private bool disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing && _logger != null)
            {
                _logger.Dispose();
            }

            disposed = true;
        }
        #endregion
    }
}
