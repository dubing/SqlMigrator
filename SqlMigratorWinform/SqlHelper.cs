using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using SqlMigratorWinform.DataAccess;
using SqlMigratorWinform.Utility;


namespace SqlMigratorWinform
{
    public class SqlHelper
    {
        public NameValueCollection CurrentSettings { get; set; }

        public static SqlHelper Instance
        {
            get { return Singleton<SqlHelper>.GetInstance(); }
        }
        public List<string> GetTableNames()
        {
            return GetDbObjectNames("U", new[] { "sysdiagrams", "dtproperties" });
        }

        public List<VersionInfo> GetAllVersion()
        {
            const string commandText = @"SELECT * FROM [VersionInfo]";
            DbAccessCommand dbcmd = GetDbAccessCommand();
            dbcmd.AttachAccessInfo(commandText, new DbParameter[0]);
            DataTable table = dbcmd.ExecuteDataTable();
            return (from DataRow row in table.Rows
                select new VersionInfo
                {
                    Version = (long) row[0], AppliedOn = (DateTime?) row[1], Description = (string) row[2]
                }).ToList();
        }

        private List<string> GetDbObjectNames(string type, params string[] exceptNames)
        {
            var names = new List<string>();
            var sb = new StringBuilder();
            if (exceptNames != null)
            {
                foreach (string name in exceptNames)
                {
                    sb.AppendFormat(" AND [Name]!='{0}'", name);
                }
            }
            string commandText = string.Format("SELECT [Name] FROM [sysobjects] WHERE [type]='{0}'{1} ORDER BY [Name]",
                                               type, sb);
            try
            {
                DbAccessCommand dbcmd = GetDbAccessCommand();
                dbcmd.AttachAccessInfo(commandText, new DbParameter[0]);
                DataTable table = dbcmd.ExecuteDataTable();
                names.AddRange(from DataRow row in table.Rows select (string)row[0]);
            }
            catch(Exception ex)
            {
            }
            return names;
        }

        public DbAccessCommand GetDbAccessCommand()
        {
            return new SqlAccessCommand(CurrentSettings["ConnectionString"]);
        }
    }
}
