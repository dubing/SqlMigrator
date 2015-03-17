using SqlMigratorWinform.Utility;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml;

namespace SqlMigratorWinform.DataAccess
{
    public sealed class SqlAccessCommand : DbAccessCommand
    {
        private static bool IsTraced = Tracer.Instance.IsTraced(typeof(SqlAccessCommand));
        private static string TraceClass = typeof(SqlAccessCommand).FullName;

        public SqlAccessCommand()
        {
        }

        public SqlAccessCommand(string connectionString) : base(connectionString)
        {
        }

        public override IAsyncResult BeginExecuteNonQuery(AsyncCallback callback, object state)
        {
            if (IsTraced)
            {
                Tracer.Instance.EnterFunction(TraceClass, "AsyncExecuteNonQuery", new string[] { "CommandText", "callback", "state", "Parameters" }, new object[] { this.CommandText, callback, state, base.GetTraceParameters(true) });
            }
            return ((SqlCommand) base.DbCommand).BeginExecuteNonQuery(callback, state);
        }

        public override IAsyncResult BeginExecuteReader(AsyncCallback callback, object state, CommandBehavior behavior)
        {
            if (IsTraced)
            {
                Tracer.Instance.EnterFunction(TraceClass, "AsyncExecuteReader", new string[] { "CommandText", "callback", "state", "behavior", "Parameters" }, new object[] { this.CommandText, callback, state, behavior, base.GetTraceParameters(true) });
            }
            return ((SqlCommand) base.DbCommand).BeginExecuteReader(callback, state, behavior);
        }

        public override IAsyncResult BeginExecuteXmlReader(AsyncCallback callback, object state)
        {
            if (IsTraced)
            {
                Tracer.Instance.EnterFunction(TraceClass, "AsyncExecuteXmlReader", new string[] { "CommandText", "callback", "state", "Parameters" }, new object[] { this.CommandText, callback, state, base.GetTraceParameters(true) });
            }
            return ((SqlCommand) base.DbCommand).BeginExecuteXmlReader(callback, state);
        }

        public override DbParameter CreateParameter(string parameterName, object value)
        {
            return new SqlParameter(parameterName, value);
        }

        public override DbParameter CreateParameter(string parameterName, int providerType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value)
        {
            return new SqlParameter(parameterName, (SqlDbType) providerType, size, direction, isNullable, precision, scale, srcColumn, srcVersion, value);
        }

        protected override void DeriveParameters(DbCommand dbCommand)
        {
            SqlCommandBuilder.DeriveParameters((SqlCommand) dbCommand);
        }

        public override int EndExecuteNonQuery(IAsyncResult asyncResult)
        {
            ParameterChecker.CheckNull("SqlAccessCommand.EndExecuteNonQuery", "asyncResult", asyncResult);
            int result = ((SqlCommand) base.DbCommand).EndExecuteNonQuery(asyncResult);
            if (IsTraced)
            {
                Tracer.Instance.LeaveFunction(TraceClass, "AsyncExecuteNonQuery", new string[] { "asyncResult", "result", "Parameters" }, new object[] { "Null:" + (asyncResult == null), result, base.GetTraceParameters(false) });
            }
            return result;
        }

        public override DbDataReader EndExecuteReader(IAsyncResult asyncResult)
        {
            ParameterChecker.CheckNull("SqlAccessCommand.EndExecuteReader", "asyncResult", asyncResult);
            DbDataReader result = ((SqlCommand) base.DbCommand).EndExecuteReader(asyncResult);
            if (IsTraced)
            {
                Tracer.Instance.LeaveFunction(TraceClass, "AsyncExecuteReader", new string[] { "asyncResult", "result", "Parameters" }, new object[] { "Null:" + (asyncResult == null), "Null:" + (result == null), base.GetTraceParameters(false) });
            }
            return result;
        }

        public override XmlReader EndExecuteXmlReader(IAsyncResult asyncResult)
        {
            ParameterChecker.CheckNull("SqlAccessCommand.EndExecuteXmlReader", "asyncResult", asyncResult);
            XmlReader result = ((SqlCommand) base.DbCommand).EndExecuteXmlReader(asyncResult);
            if (IsTraced)
            {
                Tracer.Instance.LeaveFunction(TraceClass, "AsyncExecuteXmlReader", new string[] { "asyncResult", "result", "Parameters" }, new object[] { "Null:" + (asyncResult == null), "Null:" + (result == null), base.GetTraceParameters(false) });
            }
            return result;
        }

        public override XmlReader ExecuteXmlReader()
        {
            XmlReader param001;
            DbAccessCommand.ExecuteContext context = new DbAccessCommand.ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "ExecuteXmlReader", new string[] { "CommandText", "Parameters" }, new object[] { this.CommandText, base.GetTraceParameters(true) });
                }
                XmlReader reader = ((SqlCommand) base.DbCommand).ExecuteXmlReader();
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "ExecuteXmlReader", new string[] { "result", "Parameters" }, new object[] { "Null:" + (reader == null), base.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = reader;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        public static SqlAccessCommand GetInstance(string connectionString)
        {
            return DbAccessCommand.GetInstance<SqlAccessCommand>(connectionString);
        }

        protected override DbDataAdapter InitDbAdapter()
        {
            return new SqlDataAdapter((SqlCommand) base.DbCommand);
        }

        protected override DbCommandBuilder InitDbBuilder()
        {
            return new SqlCommandBuilder((SqlDataAdapter) base.DbAdapter);
        }

        protected override DbConnection InitDbConnection()
        {
            return new SqlConnection();
        }

        public override string ParameterPrefix
        {
            get
            {
                return "@";
            }
        }
    }
}

