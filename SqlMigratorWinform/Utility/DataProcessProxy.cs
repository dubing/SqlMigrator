using System;
using System.Collections.Generic;
using System.Threading;

namespace SqlMigratorWinform.Utility
{
    public sealed class DataProcessProxy<T> : IDisposable
    {
        private readonly DataProcessHandler<T> DataProcess;
        private readonly Queue<T> DataQueue;
        private bool IsRunning;
        public int QueueMaxLength;
        public int ThreadSleepTime;
        private Thread WorkThread;

        public DataProcessProxy(DataProcessHandler<T> dataProcess)
        {
            QueueMaxLength = 0x3e8;
            ThreadSleepTime = 100;
            IsRunning = false;
            DataQueue = new Queue<T>();
            DataProcess = dataProcess;
        }

        public void Dispose()
        {
            IsRunning = false;
            Thread.Sleep(100);
            int count = DataQueue.Count;
            Logger.Instance.BaseLogger.WriteEntry(string.Format("{0} objects skipped by dispose DataProcessProxy.", count));
            lock (DataQueue)
            {
                DataQueue.Clear();
            }
        }

        public void Process(T data)
        {
            if (DataQueue.Count > QueueMaxLength)
            {
                T temp;
                lock ((DataQueue))
                {
                    DataQueue.Enqueue(data);
                    temp = DataQueue.Dequeue();
                }
                DataProcess(temp);
            }
            else
            {
                lock ((DataQueue))
                {
                    DataQueue.Enqueue(data);
                }
                Start();
            }
        }

        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                WorkThread = new Thread(WorkThreadStart) {Priority = ThreadPriority.BelowNormal};
                WorkThread.Start();
            }
        }

        public void Stop()
        {
            Dispose();
        }

        private void WorkThreadStart()
        {
            for (int count = DataQueue.Count; count > 0; count = DataQueue.Count)
            {
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        T data = DataQueue.Dequeue();
                        DataProcess(data);
                    }
                    catch
                    {
                    }
                    Thread.Sleep(10);
                }
            }
            IsRunning = false;
        }
    }
}

