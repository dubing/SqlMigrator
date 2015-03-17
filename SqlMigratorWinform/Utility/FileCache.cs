using System;
using System.IO;
using System.Linq;
using System.Web;

namespace SqlMigratorWinform.Utility
{
    public sealed class FileCache
    {
        private readonly string CacheKeyPrepend;
        private static readonly bool IsTraced = Tracer.Instance.IsTraced(typeof(FileCache));
        private static readonly object LockKey = new object();
        private readonly ReadFileHandler OnReadFile;
        public int SlidingExpiration = 30;
        private static readonly string TraceClass = typeof(FileCache).FullName;

        public FileCache(string cacheKeyPrepend, ReadFileHandler onReadFile)
        {
            ParameterChecker.CheckNull("FileCache", "onReadFile", onReadFile);
            CacheKeyPrepend = cacheKeyPrepend ?? string.Empty;
            OnReadFile = onReadFile;
        }

        private string MakeCacheKey(params string[] filenames)
        {
            return (CacheKeyPrepend + "::" + string.Join(":", filenames));
        }

        public object ReadFile(params string[] anyFilenames)
        {
            ParameterChecker.CheckNullOrEmpty("FileCache.ReadFile", "anyFilenames", anyFilenames, true);
            string[] filenames = anyFilenames.Select(file => file.IndexOf("SqlMigratorWinform") != -1 ? file : new FileInfo(file).FullName).ToArray();
            string cacheKey = MakeCacheKey(filenames);
            object obj = HttpRuntime.Cache[cacheKey];
            if (obj == null)
            {
                lock (LockKey)
                {
                    obj = HttpRuntime.Cache[cacheKey];
                    if (obj != null)
                    {
                        return obj;
                    }
                    try
                    {
                        obj = OnReadFile(filenames);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.WriteEntry(ex);
                    }
                    if (obj != null)
                    {
                    }
                    if (IsTraced)
                    {
                        Tracer.Instance.Write(TraceClass, "OnReadFile", new[] { "filenames", "result" }, new[] { filenames, obj });
                    }
                }
            }
            return obj;
        }
    }
}

