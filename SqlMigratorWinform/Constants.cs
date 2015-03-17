using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlMigratorWinform
{
    public static class Constants
    {
        public static readonly string ConnectionString = @"ConnectionString";
    }

    public static class ParameterCheckerMessage
    {
        public static readonly string CheckNull = "参数 {0} 为null.";
        public static readonly string Empty = "参数 {0} 为Empty.";
        public static readonly string CheckNullOrEmpty = "参数 {0} 为空.";
        public static readonly string CheckRange = "参数 {0} 超出范围.";
        public static readonly string Items = ".Items";
    }

}
