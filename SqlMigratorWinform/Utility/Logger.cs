using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SqlMigratorWinform.Utility
{
    public sealed class Logger
    {
        private bool _HasInitialized = false;
        internal EventLog BaseLogger = null;
        private static bool IsTraced = Tracer.Instance.IsTraced(typeof(Logger));
        private Dictionary<string, LogCounter> LogCounterList;
        private DataProcessProxy<LogObject> LoggerProxy;
        public const int MaxEventLogStringSize = 0x3fff;
        private static string TraceClass = typeof(Logger).FullName;

        public Logger()
        {
            this.LoggerProxy = new DataProcessProxy<LogObject>(new DataProcessHandler<LogObject>(this.OnProxyLog));
            this.LogCounterList = new Dictionary<string, LogCounter>();
        }

        private void BaseWriteEntry(string message, EventLogEntryType logType, int eventId)
        {
            if (this.HasInitialized)
            {
                this.LoggerProxy.Process(new LogObject(message, logType, eventId));
            }
        }

        public void Dispose()
        {
            this.BaseLogger.Dispose();
        }

        private LogCounter GetLogCounter(string key)
        {
            LogCounter counter;
            if (this.LogCounterList.TryGetValue(key, out counter))
            {
                return counter;
            }
            return null;
        }

        public void Initialize(string machineName, string logName, string source)
        {
            this.BaseLogger = new EventLog(logName, machineName, source);
            this._HasInitialized = true;
        }

        private string MakeLogKey(Exception ex, int errorCode)
        {
            StringBuilder sb = new StringBuilder();
            while (ex != null)
            {
                sb.Append(ex.GetType().FullName + ":");
                ex = ex.InnerException;
            }
            sb.Append(errorCode);
            return sb.ToString();
        }

        public static Logger NewInstance(string machineName, string logName, string source)
        {
            Logger logger = new Logger();
            logger.Initialize(machineName, logName, source);
            return logger;
        }

        private void OnProxyLog(LogObject entry)
        {
            try
            {
                if (entry.EventId == -2147483648)
                {
                    this.BaseLogger.WriteEntry(entry.Message, entry.LogType);
                }
                else
                {
                    this.BaseLogger.WriteEntry(entry.Message, entry.LogType, entry.EventId);
                }
            }
            catch
            {
            }
        }

        private string TruncateForEventLog(string eventLogMessage)
        {
            if ((eventLogMessage != null) && (eventLogMessage.Length > 0x3fff))
            {
                return eventLogMessage.Substring(0, 0x3fff);
            }
            return eventLogMessage;
        }

        public void WriteEntry(Exception ex)
        {
            this.WriteEntry(ex, -2147483648);
        }

        public void WriteEntry(string message)
        {
            this.WriteEntry(message, EventLogEntryType.Information, -2147483648);
        }

        public void WriteEntry(Exception ex, int errorCode)
        {
            string message;
            string logKey = this.MakeLogKey(ex, errorCode);
            LogCounter counter = this.GetLogCounter(logKey);
            bool needRefresh = false;
            bool needWriteLog = true;
            lock (this.LogCounterList)
            {
                counter = this.GetLogCounter(logKey);
                if (counter == null)
                {
                    counter = new LogCounter();
                    this.LogCounterList.Add(logKey, counter);
                }
                else
                {
                    needRefresh = counter.NeedRefresh();
                    needWriteLog = counter.NeedWriteLog();
                    if (needRefresh)
                    {
                        counter.Refresh();
                    }
                    else
                    {
                        counter.Increase();
                    }
                }
            }
            if (needWriteLog)
            {
                message = this.TruncateForEventLog(ex.ToString());
                this.BaseWriteEntry(message, EventLogEntryType.Error, errorCode);
            }
            else if (needRefresh)
            {
                message = string.Format("{0} of {1} skipped.", counter.SkippedCounter, logKey);
                this.BaseWriteEntry(message, EventLogEntryType.Information, 0);
            }
            if (IsTraced)
            {
                Tracer.Instance.Write(TraceClass, "WriteEntry", ex, errorCode);
            }
        }

        public void WriteEntry(string message, EventLogEntryType logType)
        {
            this.WriteEntry(message, logType, -2147483648);
        }

        public void WriteEntry(string message, EventLogEntryType logType, int eventId)
        {
            message = this.TruncateForEventLog(message);
            this.BaseWriteEntry(message, logType, eventId);
            if (IsTraced)
            {
                Tracer.Instance.Write(TraceClass, "WriteEntry", new string[] { "message", "logType", "eventId" }, new object[] { message, logType, eventId });
            }
        }

        public bool HasInitialized
        {
            get
            {
                return this._HasInitialized;
            }
        }

        public static Logger Instance
        {
            get
            {
                return Singleton<Logger>.GetInstance();
            }
        }
    }
}

