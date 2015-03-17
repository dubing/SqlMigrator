using System;

namespace SqlMigratorWinform.Utility
{
    internal class LogCounter
    {
        public int Counter = 1;
        public static int CountLimit = 100;
        public static int MinuteLimit = 10;
        public DateTime StartTime = DateTime.Now;

        public void Increase()
        {
            this.Counter++;
        }

        public bool NeedRefresh()
        {
            TimeSpan Param0004 = (TimeSpan) (DateTime.Now - this.StartTime);
            return (Param0004.TotalSeconds > (MinuteLimit * 60));
        }

        public bool NeedWriteLog()
        {
            return (this.Counter < CountLimit);
        }

        public void Refresh()
        {
            this.StartTime = DateTime.Now;
            this.Counter = 1;
        }

        public int SkippedCounter
        {
            get
            {
                return (this.Counter - CountLimit);
            }
        }
    }
}

