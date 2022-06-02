using System;
using NLog;
using NLog.Targets;

namespace TodoApi
{
    public class ExternalEventArgs : EventArgs
    {
        public string Information { get; protected set; }
        public string Level { get; protected set; }

        public object[] Arguments { get; protected set; }

        public ExternalEventArgs(string information, string level, params object[] args)
        {
            Information = information;
            Level = level;
            Arguments = args;
        }
    }
    internal class Log
    {
        private static readonly object Sync = new object();

        private static volatile Log _instance;

        public static Log Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Sync)
                    {
                        if (_instance == null)
                        {
                            var temp = new Log();
                            temp.Init();

                            _instance = temp;
                        }
                    }
                }

                return _instance;
            }
        }

        private LogFactory _factory;

        private void Init()
        {
            if (_factory == null)
            {
                _factory = new LogFactory();

            }
        }

        private Logger Logger(object obj)
        {
            return _factory.GetLogger(obj.GetType().Name);
        }

        public EventHandler<ExternalEventArgs> ExternalEvent;

        private void External(string message, string level, params object[] args)
        {
            try
            {
                if (ExternalEvent != null && !string.IsNullOrWhiteSpace(message))
                {
                    ExternalEvent(this, new ExternalEventArgs(message, level, args));
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void Info(object obj, string message, params object[] args)
        {
            Logger(obj).Info(message, args);

            External(message, "INFO", args);
        }

        public void Info(string message, params object[] args)
        {
            Logger(new object()).Info(message, args);

            External(message, "INFO", args);
        }

        public void Error(object obj, string message, params object[] args)
        {
            Logger(obj).Error(message, args);

            External(message, "ERROR", args);
        }

        public void Error(object obj, Exception ex)
        {
            Logger(obj).Error(ex.Message);

            External(ex.Message, "ERROR", ex);
        }

        public string LogFileName
        {
            get
            {
                var fileTarget = (FileTarget)_factory.Configuration.FindTargetByName("fileLogTrace");
                return fileTarget.FileName.Render(new LogEventInfo { TimeStamp = DateTime.Now });
            }
        }

        public void SetLogFileName(string dir, string name)
        {
            var target = (FileTarget)_factory.Configuration.FindTargetByName("fileLogTrace");
            target.FileName = name;

            var config = _factory.Configuration;
            config.Variables["logFolder"] = dir;
            config.LoggingRules[0].Targets.Clear();
            config.LoggingRules[0].Targets.Add(target);

            LogManager.ReconfigExistingLoggers();
            LogManager.Configuration = config;
            LogManager.ReconfigExistingLoggers();
        }
    }
}