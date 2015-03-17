using System;
using System.Collections.Generic;
using System.Threading;

namespace SqlMigratorWinform.Utility
{
    public static class Singleton<T>
    {
        private static T _Instance;
        private static object LockKey;

        static Singleton()
        {
            Singleton<T>.LockKey = new object();
        }

        public static T GetInstance()
        {
            return Singleton<T>.GetInstance(null);
        }

        public static T GetInstance(CreateInstanceHandler<T> onCreateInstance)
        {
            if (Singleton<T>._Instance == null)
            {
                lock (Singleton<T>.LockKey)
                {
                    if (Singleton<T>._Instance == null)
                    {
                        try
                        {
                            if (onCreateInstance == null)
                            {
                                Singleton<T>._Instance = Activator.CreateInstance<T>();
                            }
                            else
                            {
                                Singleton<T>._Instance = onCreateInstance();
                            }
                        }
                        catch
                        {
                            Singleton<T>._Instance = default(T);
                        }
                    }
                }
            }
            return Singleton<T>._Instance;
        }

        public static T GetThreadInstance()
        {
            return Singleton<T>.GetThreadInstance(null, null);
        }

        public static T GetThreadInstance(string appendedkey)
        {
            return Singleton<T>.GetThreadInstance(appendedkey, null);
        }

        public static T GetThreadInstance(string appendedkey, CreateInstanceHandler<T> onCreateInstance)
        {
            object obj;
            object Param0001;
            LocalDataStoreSlot slot = Thread.GetNamedDataSlot("__ThreadInstanceTable");
            Dictionary<string, object> instances = Thread.GetData(slot) as Dictionary<string, object>;
            if (instances == null)
            {
                lock ((Param0001 = Singleton<T>.LockKey))
                {
                    instances = Thread.GetData(slot) as Dictionary<string, object>;
                    if (instances == null)
                    {
                        instances = new Dictionary<string, object>();
                        Thread.SetData(slot, instances);
                    }
                }
            }
            string key = string.Format("{0}:{1}", typeof(T).AssemblyQualifiedName, appendedkey);
            if (instances.TryGetValue(key, out obj))
            {
                return (T) obj;
            }
            lock ((Param0001 = Singleton<T>.LockKey))
            {
                T t;
                if (instances.TryGetValue(key, out obj))
                {
                    return (T) obj;
                }
                try
                {
                    t = (onCreateInstance == null) ? Activator.CreateInstance<T>() : onCreateInstance();
                }
                catch
                {
                    t = default(T);
                }
                instances[key] = t;
                return t;
            }
        }

        public static void ReleaseInstance()
        {
            lock (Singleton<T>.LockKey)
            {
                IDisposable id = Singleton<T>._Instance as IDisposable;
                if (id != null)
                {
                    id.Dispose();
                }
                Singleton<T>._Instance = default(T);
            }
        }

        public static void ReleaseThreadInstance()
        {
            LocalDataStoreSlot slot = Thread.GetNamedDataSlot("__ThreadInstanceTable");
            lock (Singleton<T>.LockKey)
            {
                Dictionary<int, object> instances = Thread.GetData(slot) as Dictionary<int, object>;
                if (instances != null)
                {
                    int threadId = Thread.CurrentThread.ManagedThreadId;
                    instances.Remove(threadId);
                }
            }
        }
    }
}

