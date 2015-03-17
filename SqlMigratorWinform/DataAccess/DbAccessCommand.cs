using SqlMigratorWinform.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace SqlMigratorWinform.DataAccess
{
    public abstract class DbAccessCommand : DbCommand, IDisposable
    {
        private bool _AutoOpenClose;
        private DbDataAdapter _DbAdapter;
        private DbCommandBuilder _DbBuilder;
        private System.Data.Common.DbCommand _DbCommand;
        private System.Data.Common.DbConnection _DbConnection;
        protected AsyncExecuteNonQueryHandler AsyncExecuteNonQuery;
        protected AsyncExecuteReaderHandler AsyncExecuteReader;
        protected AsyncExecuteXmlReaderHandler AsyncExecuteXmlReader;
        private static bool IsTraced = Tracer.Instance.IsTraced(typeof(DbAccessCommand));
        private static object LockKey = new object();
        private Dictionary<string, DbParameter[]> ParametersCacheTable;
        private static string TraceClass = typeof(DbAccessCommand).FullName;

        protected DbAccessCommand()
        {
            this._DbConnection = null;
            this._DbCommand = null;
            this._DbAdapter = null;
            this._DbBuilder = null;
            this._AutoOpenClose = true;
        }

        protected DbAccessCommand(string connectionString)
        {
            this._DbConnection = null;
            this._DbCommand = null;
            this._DbAdapter = null;
            this._DbBuilder = null;
            this._AutoOpenClose = true;
            ParameterChecker.CheckNullOrEmpty("DbAccessCommand", "connectionString", connectionString);
            this.ConnectionString = connectionString;
        }

        public DbParameter AddParameter(string parameterName, object value)
        {
            DbParameter parameter = this.CreateParameter(parameterName, value);
            base.Parameters.Add(parameter);
            return parameter;
        }

        public DbParameter AddParameter(string parameterName, int providerType, int size, ParameterDirection direction, byte precision, byte scale, object value)
        {
            DbParameter parameter = this.CreateParameter(parameterName, providerType, size, direction, precision, scale, value);
            base.Parameters.Add(parameter);
            return parameter;
        }

        public void AssignParameterValues(object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length != 0))
            {
                int count = parameterValues.Length;
                if (this.DbCommand.Parameters.Count != count)
                {
                    throw new ApplicationException("The number of parameters doesn't match the number of parameterValues.");
                }
                for (int i = 0; i < count; i++)
                {
                    object value = parameterValues[i];
                    if (value == null)
                    {
                        value = DBNull.Value;
                    }
                    this.DbCommand.Parameters[i].Value = value;
                }
            }
        }

        public void AssignParameterValues(DbAccessParameterCollection accessParameters)
        {
            if (accessParameters != null)
            {
                foreach (DbAccessParameter parameter in accessParameters)
                {
                    DbParameter dbParameter = this.DbCommand.Parameters[parameter.ParameterName];
                    if (dbParameter != null)
                    {
                        object value = parameter.Value;
                        if (value == null)
                        {
                            value = DBNull.Value;
                        }
                        dbParameter.Value = value;
                    }
                }
            }
        }

        public void AttachAccessInfo(DbAccessInformation accessInfo)
        {
            this.AttachAccessInfo(accessInfo, null);
        }

        public void AttachAccessInfo(DbAccessInformation accessInfo, DbParameter[] parameters)
        {
            if (accessInfo != null)
            {
                this.DbCommand.CommandText = accessInfo.CommandText;
                this.DbCommand.CommandType = accessInfo.CommandType;
                this.DbCommand.CommandTimeout = accessInfo.CommandTimeout;
                this.DbCommand.Parameters.Clear();
                if ((parameters == null) && (accessInfo.CommandType == System.Data.CommandType.StoredProcedure))
                {
                    parameters = this.GetCachedParameters(accessInfo.CommandText);
                }
                if (parameters == null)
                {
                    if (accessInfo.Parameters != null)
                    {
                        foreach (DbAccessParameter accParameter in accessInfo.Parameters)
                        {
                            if (accParameter.ProvideType >= 0)
                            {
                                this.DbCommand.Parameters.Add(this.CreateParameter(accParameter.ParameterName, accParameter.ProvideType, accParameter.Size, accParameter.Direction, accParameter.IsNullable, accParameter.Precision, accParameter.Scale, accParameter.SourceColumn, accParameter.SourceVersion, accParameter.Value));
                            }
                            else
                            {
                                this.DbCommand.Parameters.Add(accParameter.ConvertParameter(base.CreateParameter()));
                            }
                        }
                    }
                }
                else
                {
                    this.DbCommand.Parameters.AddRange(parameters);
                    if (accessInfo.Parameters != null)
                    {
                        this.AssignParameterValues(accessInfo.Parameters);
                    }
                }
                foreach (DbParameter parameter in this.DbCommand.Parameters)
                {
                    if (parameter.Value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                }
            }
        }

        public void AttachAccessInfo(string commandText, params DbParameter[] parameters)
        {
            this.AttachAccessInfo(commandText, System.Data.CommandType.Text, null, parameters);
        }

        public void AttachAccessInfo(string commandText, System.Data.CommandType commandType, DataRow row, params DbParameter[] parameters)
        {
            if (!string.IsNullOrEmpty(commandText))
            {
                this.DbCommand.CommandText = commandText;
                this.DbCommand.CommandType = commandType;
                if ((parameters == null) && (commandType == System.Data.CommandType.StoredProcedure))
                {
                    parameters = this.GetCachedParameters(commandText);
                }
                this.DbCommand.Parameters.Clear();
                if (parameters != null)
                {
                    if (row != null)
                    {
                        DataRowVersion defaultVersion = this.GetReferenceRowVersion(row);
                        foreach (DbParameter parameter in parameters)
                        {
                            DataColumn col = row.Table.Columns[parameter.SourceColumn];
                            if (col != null)
                            {
                                if (row.HasVersion(parameter.SourceVersion))
                                {
                                    parameter.Value = row[col, parameter.SourceVersion];
                                }
                                else
                                {
                                    parameter.Value = row[col, defaultVersion];
                                }
                            }
                        }
                    }
                    this.DbCommand.Parameters.AddRange(parameters);
                }
                foreach (DbParameter parameter in this.DbCommand.Parameters)
                {
                    if (parameter.Value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                }
            }
        }

        public IAsyncResult BeginExecuteNonQuery()
        {
            return this.BeginExecuteNonQuery(null, null);
        }

        public virtual IAsyncResult BeginExecuteNonQuery(AsyncCallback callback, object state)
        {
            if (IsTraced)
            {
                Tracer.Instance.EnterFunction(TraceClass, "AsyncExecuteNonQuery", new string[] { "CommandText", "callback", "state", "Parameters" }, new object[] { this.CommandText, callback, state, this.GetTraceParameters(true) });
            }
            this.AsyncExecuteNonQuery = new AsyncExecuteNonQueryHandler(this.ExecuteNonQuery);
            return this.AsyncExecuteNonQuery.BeginInvoke(callback, state);
        }

        public IAsyncResult BeginExecuteReader()
        {
            return this.BeginExecuteReader(CommandBehavior.Default);
        }

        public IAsyncResult BeginExecuteReader(CommandBehavior behavior)
        {
            return this.BeginExecuteReader(null, null, behavior);
        }

        public IAsyncResult BeginExecuteReader(AsyncCallback callback, object state)
        {
            return this.BeginExecuteReader(callback, state, CommandBehavior.Default);
        }

        public virtual IAsyncResult BeginExecuteReader(AsyncCallback callback, object state, CommandBehavior behavior)
        {
            if (IsTraced)
            {
                Tracer.Instance.EnterFunction(TraceClass, "AsyncExecuteReader", new string[] { "CommandText", "callback", "state", "behavior", "Parameters" }, new object[] { this.CommandText, callback, state, behavior, this.GetTraceParameters(true) });
            }
            this.AsyncExecuteReader = new AsyncExecuteReaderHandler(this.ExecuteReader);
            return this.AsyncExecuteReader.BeginInvoke(behavior, callback, state);
        }

        public IAsyncResult BeginExecuteXmlReader()
        {
            return this.BeginExecuteXmlReader(null, null);
        }

        public virtual IAsyncResult BeginExecuteXmlReader(AsyncCallback callback, object state)
        {
            if (IsTraced)
            {
                Tracer.Instance.EnterFunction(TraceClass, "AsyncExecuteXmlReader", new string[] { "CommandText", "callback", "state", "Parameters" }, new object[] { this.CommandText, callback, state, this.GetTraceParameters(true) });
            }
            this.AsyncExecuteXmlReader = new AsyncExecuteXmlReaderHandler(this.ExecuteXmlReader);
            return this.AsyncExecuteXmlReader.BeginInvoke(callback, state);
        }

        public void BeginTransaction()
        {
            if (this.AutoOpenClose)
            {
                this.Open();
            }
            this.DbCommand.Transaction = this.DbConnection.BeginTransaction();
            if (IsTraced)
            {
                Tracer.Instance.EnterFunction(TraceClass, "TransactionBegin_End", string.Empty);
            }
        }

        public override void Cancel()
        {
            this.DbCommand.Cancel();
        }

        public DbParameter[] CloneParameters(params DbParameter[] parameters)
        {
            if (parameters == null)
            {
                return null;
            }
            DbParameter[] clonedParameters = new DbParameter[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                object oldValue = parameters[i].Value;
                parameters[i].Value = DBNull.Value;
                clonedParameters[i] = ((ICloneable) parameters[i]).Clone() as DbParameter;
                parameters[i].Value = oldValue;
            }
            return clonedParameters;
        }

        public void Close()
        {
            if (this.DbConnection.State != ConnectionState.Closed)
            {
                this.DbConnection.Close();
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "Open_Close", string.Empty);
                }
            }
        }

        public void CommitTransaction()
        {
            if (this.DbCommand.Transaction != null)
            {
                this.DbCommand.Transaction.Commit();
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "TransactionBegin_End", "CommitTransaction");
                }
                if (this.AutoOpenClose)
                {
                    this.Close();
                }
            }
        }

        protected override DbParameter CreateDbParameter()
        {
            return this.DbCommand.CreateParameter();
        }

        public abstract DbParameter CreateParameter(string parameterName, object value);
        public DbParameter CreateParameter(string parameterName, int providerType, int size, ParameterDirection direction, byte precision, byte scale, object value)
        {
            return this.CreateParameter(parameterName, providerType, size, direction, true, precision, scale, string.Empty, DataRowVersion.Default, value);
        }

        public abstract DbParameter CreateParameter(string parameterName, int providerType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value);
        protected abstract void DeriveParameters(System.Data.Common.DbCommand dbCommand);
        public DbParameter[] DeriveParameters(string procedureName, bool includeReturnValueParameter)
        {
            ParameterChecker.CheckNullOrEmpty("DbAccessCommand.DeriveParameters", "procedureName", procedureName);
            System.Data.Common.DbCommand cmd = this.DbConnection.CreateCommand();
            cmd.CommandText = procedureName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                this.DeriveParameters(cmd);
                context.SuccessfullyCall();
            }
            finally
            {
                context.FinallyCall();
            }
            if (!includeReturnValueParameter)
            {
                DbParameter returnParameter = null;
                foreach (DbParameter parameter in cmd.Parameters)
                {
                    if (parameter.Direction == ParameterDirection.ReturnValue)
                    {
                        returnParameter = parameter;
                        break;
                    }
                }
                if (returnParameter != null)
                {
                    cmd.Parameters.Remove(returnParameter);
                }
            }
            DbParameter[] parameters = new DbParameter[cmd.Parameters.Count];
            cmd.Parameters.CopyTo(parameters, 0);
            return parameters;
        }

        public void Dispose()
        {
            this.Close();
            if (this._DbConnection != null)
            {
                this._DbConnection.Dispose();
                this._DbConnection = null;
            }
            if (this._DbCommand != null)
            {
                this._DbCommand.Dispose();
                this._DbCommand = null;
            }
            if (this._DbAdapter != null)
            {
                this._DbAdapter.Dispose();
                this._DbAdapter = null;
            }
            if (this._DbBuilder != null)
            {
                this._DbBuilder.Dispose();
                this._DbBuilder = null;
            }
            base.Dispose();
        }

        public virtual int EndExecuteNonQuery(IAsyncResult asyncResult)
        {
            ParameterChecker.CheckNull("DbAccessCommand.EndExecuteNonQuery", "AsyncExecuteNonQuery", this.AsyncExecuteNonQuery);
            ParameterChecker.CheckNull("DbAccessCommand.EndExecuteNonQuery", "asyncResult", asyncResult);
            int result = this.AsyncExecuteNonQuery.EndInvoke(asyncResult);
            this.AsyncExecuteNonQuery = null;
            if (IsTraced)
            {
                Tracer.Instance.LeaveFunction(TraceClass, "AsyncExecuteNonQuery", new string[] { "asyncResult", "result", "Parameters" }, new object[] { "Null:" + (asyncResult == null), result, this.GetTraceParameters(false) });
            }
            return result;
        }

        public virtual DbDataReader EndExecuteReader(IAsyncResult asyncResult)
        {
            ParameterChecker.CheckNull("DbAccessCommand.EndExecuteReader", "AsyncExecuteReader", this.AsyncExecuteReader);
            ParameterChecker.CheckNull("DbAccessCommand.EndExecuteReader", "asyncResult", asyncResult);
            DbDataReader result = this.AsyncExecuteReader.EndInvoke(asyncResult);
            this.AsyncExecuteReader = null;
            if (IsTraced)
            {
                Tracer.Instance.LeaveFunction(TraceClass, "AsyncExecuteReader", new string[] { "asyncResult", "result", "Parameters" }, new object[] { "Null:" + (asyncResult == null), "Null:" + (result == null), this.GetTraceParameters(false) });
            }
            return result;
        }

        public virtual XmlReader EndExecuteXmlReader(IAsyncResult asyncResult)
        {
            ParameterChecker.CheckNull("DbAccessCommand.EndExecuteXmlReader", "AsyncExecuteXmlReader", this.AsyncExecuteXmlReader);
            ParameterChecker.CheckNull("DbAccessCommand.EndExecuteXmlReader", "asyncResult", asyncResult);
            XmlReader result = this.AsyncExecuteXmlReader.EndInvoke(asyncResult);
            this.AsyncExecuteXmlReader = null;
            if (IsTraced)
            {
                Tracer.Instance.LeaveFunction(TraceClass, "AsyncExecuteXmlReader", new string[] { "asyncResult", "result", "Parameters" }, new object[] { "Null:" + (asyncResult == null), "Null:" + (result == null), this.GetTraceParameters(false) });
            }
            return result;
        }

        public DataRow ExecuteDataRow()
        {
            return this.ExecuteDataRow(null);
        }

        public DataRow ExecuteDataRow(DataTable dataTable)
        {
            if (dataTable == null)
            {
                dataTable = new DataTable();
            }
            this.ExecuteDataTable(dataTable, 0, 1);
            if (((dataTable == null) || (dataTable.Rows == null)) || (dataTable.Rows.Count == 0))
            {
                return null;
            }
            return dataTable.Rows[0];
        }

        public DataSet ExecuteDataSet()
        {
            DataSet dataSet = new DataSet();
            this.ExecuteDataSet(dataSet);
            return dataSet;
        }

        public int ExecuteDataSet(DataSet dataSet)
        {
            int param001;
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "ExecuteDataSet", new string[] { "dataSet", "CommandText", "Parameters" }, new object[] { "Null:" + (dataSet == null), this.CommandText, this.GetTraceParameters(true) });
                }
                if (dataSet == null)
                {
                    dataSet = new DataSet();
                }
                int result = this.DbAdapter.Fill(dataSet);
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "ExecuteDataSet", new string[] { "result", "Parameters" }, new object[] { result, this.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = result;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        public int ExecuteDataSet(DataSet dataSet, string srcTable)
        {
            int param001;
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "ExecuteDataSet", new string[] { "dataSet", "srcTable", "CommandText", "Parameters" }, new object[] { "Null:" + (dataSet == null), srcTable, this.CommandText, this.GetTraceParameters(true) });
                }
                if (dataSet == null)
                {
                    dataSet = new DataSet();
                }
                if (string.IsNullOrEmpty(srcTable))
                {
                    srcTable = "Table";
                }
                int result = this.DbAdapter.Fill(dataSet, srcTable);
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "ExecuteDataSet", new string[] { "result", "Parameters" }, new object[] { result, this.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = result;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        public int ExecuteDataSet(DataSet dataSet, int startRecord, int maxRecords, string srcTable)
        {
            int param001;
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "ExecuteDataSet", new string[] { "dataSet", "startRecord", "maxRecords", "srcTable", "CommandText", "Parameters" }, new object[] { "Null:" + (dataSet == null), startRecord, maxRecords, srcTable, this.CommandText, this.GetTraceParameters(true) });
                }
                if (dataSet == null)
                {
                    dataSet = new DataSet();
                }
                if (string.IsNullOrEmpty(srcTable))
                {
                    srcTable = "Table";
                }
                int result = this.DbAdapter.Fill(dataSet, startRecord, maxRecords, srcTable);
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "ExecuteDataSet", new string[] { "result", "Parameters" }, new object[] { result, this.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = result;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        public DataTable ExecuteDataTable()
        {
            DataTable dataTable = new DataTable();
            this.ExecuteDataTable(dataTable);
            return dataTable;
        }

        public int ExecuteDataTable(DataTable dataTable)
        {
            int param001;
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "ExecuteDataTable", new string[] { "dataTable", "CommandText", "Parameters" }, new object[] { "Null:" + (dataTable == null), this.CommandText, this.GetTraceParameters(true) });
                }
                if (dataTable == null)
                {
                    dataTable = new DataTable();
                }
                int result = this.DbAdapter.Fill(dataTable);
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "ExecuteDataTable", new string[] { "result", "Parameters" }, new object[] { result, this.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = result;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        public DataTable ExecuteDataTable(int startRecord, int maxRecords)
        {
            DataTable dataTable = new DataTable();
            this.ExecuteDataTable(dataTable, startRecord, maxRecords);
            return dataTable;
        }

        public int ExecuteDataTable(DataTable dataTable, int startRecord, int maxRecords)
        {
            int param001;
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "ExecuteDataTable", new string[] { "dataTable", "startRecord", "maxRecords", "CommandText", "Parameters" }, new object[] { "Null:" + (dataTable == null), startRecord, maxRecords, this.CommandText, this.GetTraceParameters(true) });
                }
                if (dataTable == null)
                {
                    dataTable = new DataTable();
                }
                int result = this.DbAdapter.Fill(startRecord, maxRecords, new DataTable[] { dataTable });
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "ExecuteDataTable", new string[] { "result", "Parameters" }, new object[] { result, this.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = result;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            DbDataReader param001;
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "ExecuteDbDataReader", new string[] { "behavior", "CommandText", "Parameters" }, new object[] { behavior, this.CommandText, this.GetTraceParameters(true) });
                }
                DbDataReader result = this.DbCommand.ExecuteReader(behavior);
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "ExecuteDbDataReader", new string[] { "result", "Parameters" }, new object[] { "result is DbDataReader", this.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = result;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        public override int ExecuteNonQuery()
        {
            int param001;
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "ExecuteNonQuery", new string[] { "CommandText", "Parameters" }, new object[] { this.CommandText, this.GetTraceParameters(true) });
                }
                int result = this.DbCommand.ExecuteNonQuery();
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "ExecuteNonQuery", new string[] { "result", "Parameters" }, new object[] { result, this.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = result;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        public DbDataReader ExecuteReader(int startRecord)
        {
            DbDataReader param001;
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "ExecuteReader", new string[] { "startRecord", "CommandText", "Parameters" }, new object[] { startRecord, this.CommandText, this.GetTraceParameters(true) });
                }
                DbDataReader dataReader = this.DbCommand.ExecuteReader();
                int tempIndex = -1;
                while ((++tempIndex < startRecord) && dataReader.Read())
                {
                }
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "ExecuteReader", new string[] { "result", "Parameters" }, new object[] { "result is DbDataReader", this.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = dataReader;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        public override object ExecuteScalar()
        {
            object param001;
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "ExecuteScalar", new string[] { "CommandText", "Parameters" }, new object[] { this.CommandText, this.GetTraceParameters(true) });
                }
                object result = this.DbCommand.ExecuteScalar();
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "ExecuteScalar", new string[] { "result", "Parameters" }, new object[] { result, this.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = result;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        public DataTable ExecuteSchemaTable()
        {
            DataTable param001;
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "ExecuteSchemaTable", new string[] { "CommandText", "Parameters" }, new object[] { this.CommandText, this.GetTraceParameters(true) });
                }
                DataTable result = null;
                using (DbDataReader reader = this.DbCommand.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    result = reader.GetSchemaTable();
                    reader.Close();
                }
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "ExecuteSchemaTable", new string[] { "result", "Parameters" }, new object[] { "Null:" + (result == null), this.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = result;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        public DataTable[] ExecuteSetSchema()
        {
            return this.ExecuteSetSchema(null, SchemaType.Source);
        }

        public DataTable[] ExecuteSetSchema(DataSet dataSet, SchemaType schemaType)
        {
            DataTable[] param001;
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "ExecuteSetSchema", new string[] { "dataSet", "schemaType", "CommandText", "Parameters" }, new object[] { "Null:" + (dataSet == null), schemaType, this.CommandText, this.GetTraceParameters(true) });
                }
                if (dataSet == null)
                {
                    dataSet = new DataSet();
                }
                DataTable[] tables = this.DbAdapter.FillSchema(dataSet, schemaType);
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "ExecuteSetSchema", new string[] { "result", "Parameters" }, new object[] { "Null:" + (tables == null), this.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = tables;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        public DataTable[] ExecuteSetSchema(DataSet dataSet, SchemaType schemaType, string srcTable)
        {
            DataTable[] param001;
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "ExecuteSetSchema", new string[] { "dataSet", "schemaType", "srcTable", "CommandText", "Parameters" }, new object[] { "Null:" + (dataSet == null), schemaType, srcTable, this.CommandText, this.GetTraceParameters(true) });
                }
                if (dataSet == null)
                {
                    dataSet = new DataSet();
                }
                if (string.IsNullOrEmpty(srcTable))
                {
                    srcTable = "Table";
                }
                DataTable[] tables = this.DbAdapter.FillSchema(dataSet, schemaType, srcTable);
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "ExecuteSetSchema", new string[] { "result", "Parameters" }, new object[] { "Null:" + (tables == null), this.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = tables;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        public DataTable ExecuteTableSchema()
        {
            return this.ExecuteTableSchema(new DataTable(), SchemaType.Source);
        }

        public DataTable ExecuteTableSchema(DataTable dataTable, SchemaType schemaType)
        {
            DataTable param001;
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "ExecuteTableSchema", new string[] { "dataTable", "schemaType", "CommandText", "Parameters" }, new object[] { "Null:" + (dataTable == null), schemaType, this.CommandText, this.GetTraceParameters(true) });
                }
                if (dataTable == null)
                {
                    dataTable = new DataTable();
                }
                DataTable result = this.DbAdapter.FillSchema(dataTable, schemaType);
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "ExecuteTableSchema", new string[] { "result", "Parameters" }, new object[] { "result is DataTable", this.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = result;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        public virtual XmlReader ExecuteXmlReader()
        {
            DataSet dataSet = this.ExecuteDataSet();
            MemoryStream ms = new MemoryStream();
            dataSet.WriteXml(ms);
            ms.Position = 0L;
            return new XmlTextReader(ms);
        }

        public DbParameter[] GetCachedParameters(string procedureName)
        {
            DbParameter[] parameters;
            object param002;
            string cacheKey = string.Format("{0}:{1}", this.ConnectionString, procedureName);
            if (this.ParametersCacheTable == null)
            {
                lock ((param002 = LockKey))
                {
                    this.ParametersCacheTable = new Dictionary<string, DbParameter[]>();
                }
            }
            if (!this.ParametersCacheTable.TryGetValue(cacheKey, out parameters))
            {
                lock ((param002 = LockKey))
                {
                    if (!this.ParametersCacheTable.TryGetValue(cacheKey, out parameters))
                    {
                        parameters = this.DeriveParameters(procedureName, true);
                        this.ParametersCacheTable[cacheKey] = parameters;
                    }
                }
            }
            return this.CloneParameters(parameters);
        }

        protected static T GetInstance<T>(string connectionString) where T: DbAccessCommand, new()
        {
            return Singleton<T>.GetThreadInstance(connectionString, delegate {
                T t = Activator.CreateInstance<T>();
                t.ConnectionString = connectionString;
                return t;
            });
        }

        protected DataRowVersion GetReferenceRowVersion(DataRow row)
        {
            if (row != null)
            {
                DataRowVersion[] versions = new DataRowVersion[] { DataRowVersion.Default, DataRowVersion.Current, DataRowVersion.Original, DataRowVersion.Proposed };
                for (int i = 0; i < versions.Length; i++)
                {
                    if (row.HasVersion(versions[i]))
                    {
                        return versions[i];
                    }
                }
            }
            return DataRowVersion.Default;
        }

        public object GetReturnValue()
        {
            foreach (DbParameter parameter in this.DbCommand.Parameters)
            {
                if (parameter.Direction == ParameterDirection.ReturnValue)
                {
                    return parameter.Value;
                }
            }
            return null;
        }

        protected SortedList GetTraceParameters(bool isBegin)
        {
            SortedList parameters = new SortedList();
            foreach (DbParameter parameter in this.DbCommand.Parameters)
            {
                if (isBegin)
                {
                    parameters.Add(parameter.ParameterName, parameter.Value);
                }
                else if (parameter.Direction != ParameterDirection.Input)
                {
                    parameters.Add(parameter.ParameterName, parameter.Value);
                }
            }
            return parameters;
        }

        protected abstract DbDataAdapter InitDbAdapter();
        protected abstract DbCommandBuilder InitDbBuilder();
        protected abstract System.Data.Common.DbConnection InitDbConnection();
        public void Open()
        {
            if (this.DbConnection.State != ConnectionState.Open)
            {
                this.DbConnection.Open();
            }
            if (IsTraced)
            {
                Tracer.Instance.EnterFunction(TraceClass, "Open_Close", this.ConnectionString);
            }
        }

        public void PickupParameteValues(DbAccessInformation accessInfo)
        {
            foreach (DbParameter parameter in this.DbCommand.Parameters)
            {
                if (parameter.Direction != ParameterDirection.Input)
                {
                    if (accessInfo.Parameters.Contains(parameter.ParameterName))
                    {
                        accessInfo.Parameters[parameter.ParameterName].Value = parameter.Value;
                    }
                    else
                    {
                        accessInfo.AddParameter(parameter.ParameterName, parameter.Value);
                    }
                }
            }
        }

        public override void Prepare()
        {
            this.DbCommand.Prepare();
        }

        public void RollbackTransaction()
        {
            if (this.DbCommand.Transaction != null)
            {
                this.DbCommand.Transaction.Rollback();
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "TransactionBegin_End", "RollbackTransaction");
                }
                if (this.AutoOpenClose)
                {
                    this.Close();
                }
            }
        }

        public virtual int UpdateDataRow(params DataRow[] dataRows)
        {
            int param001;
            if (dataRows == null)
            {
                return -1;
            }
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "UpdateDataTable", new string[] { "CommandText", "Parameters" }, new object[] { this.CommandText, this.GetTraceParameters(true) });
                }
                this.DbBuilder.SetAllValues = false;
                int result = this.DbAdapter.Update(dataRows);
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "UpdateDataTable", new string[] { "result", "Parameters" }, new object[] { result, this.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = result;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        public virtual int UpdateDataTable(DataTable dataTable)
        {
            int param001;
            if (dataTable == null)
            {
                return -1;
            }
            ExecuteContext context = new ExecuteContext(this);
            try
            {
                if (IsTraced)
                {
                    Tracer.Instance.EnterFunction(TraceClass, "UpdateDataTable", new string[] { "CommandText", "Parameters" }, new object[] { this.CommandText, this.GetTraceParameters(true) });
                }
                this.DbBuilder.SetAllValues = false;
                int result = this.DbAdapter.Update(dataTable);
                if (IsTraced)
                {
                    Tracer.Instance.LeaveFunction(TraceClass, "UpdateDataTable", new string[] { "result", "Parameters" }, new object[] { result, this.GetTraceParameters(false) });
                }
                context.SuccessfullyCall();
                param001 = result;
            }
            finally
            {
                context.FinallyCall();
            }
            return param001;
        }

        public bool AutoOpenClose
        {
            get
            {
                return this._AutoOpenClose;
            }
            set
            {
                this._AutoOpenClose = value;
            }
        }

        public override string CommandText
        {
            get
            {
                return this.DbCommand.CommandText;
            }
            set
            {
                this.DbCommand.CommandText = value;
            }
        }

        public override int CommandTimeout
        {
            get
            {
                return this.DbCommand.CommandTimeout;
            }
            set
            {
                this.DbCommand.CommandTimeout = value;
            }
        }

        public override System.Data.CommandType CommandType
        {
            get
            {
                return this.DbCommand.CommandType;
            }
            set
            {
                this.DbCommand.CommandType = value;
            }
        }

        public string ConnectionString
        {
            get
            {
                return this.DbConnection.ConnectionString;
            }
            set
            {
                this.DbConnection.ConnectionString = value;
            }
        }

        protected DbDataAdapter DbAdapter
        {
            get
            {
                if (this._DbAdapter == null)
                {
                    this._DbAdapter = this.InitDbAdapter();
                }
                return this._DbAdapter;
            }
            set
            {
                this._DbAdapter = value;
            }
        }

        protected DbCommandBuilder DbBuilder
        {
            get
            {
                if (this._DbBuilder == null)
                {
                    this._DbBuilder = this.InitDbBuilder();
                }
                return this._DbBuilder;
            }
            set
            {
                this._DbBuilder = value;
            }
        }

        protected System.Data.Common.DbCommand DbCommand
        {
            get
            {
                if (this._DbCommand == null)
                {
                    this._DbCommand = this.DbConnection.CreateCommand();
                }
                return this._DbCommand;
            }
            set
            {
                this._DbCommand = value;
            }
        }

        protected override System.Data.Common.DbConnection DbConnection
        {
            get
            {
                if (this._DbConnection == null)
                {
                    this._DbConnection = this.InitDbConnection();
                }
                return this._DbConnection;
            }
            set
            {
                this._DbConnection = value;
            }
        }

        protected override System.Data.Common.DbParameterCollection DbParameterCollection
        {
            get
            {
                return this.DbCommand.Parameters;
            }
        }

        protected override System.Data.Common.DbTransaction DbTransaction
        {
            get
            {
                return this.DbCommand.Transaction;
            }
            set
            {
                this.DbCommand.Transaction = value;
            }
        }

        public override bool DesignTimeVisible
        {
            get
            {
                return this.DbCommand.DesignTimeVisible;
            }
            set
            {
                this.DbCommand.DesignTimeVisible = value;
            }
        }

        public abstract string ParameterPrefix { get; }

        public override UpdateRowSource UpdatedRowSource
        {
            get
            {
                return this.DbCommand.UpdatedRowSource;
            }
            set
            {
                this.DbCommand.UpdatedRowSource = value;
            }
        }

        protected delegate int AsyncExecuteNonQueryHandler();

        protected delegate DbDataReader AsyncExecuteReaderHandler(CommandBehavior behavior);

        protected delegate XmlReader AsyncExecuteXmlReaderHandler();

        [StructLayout(LayoutKind.Sequential)]
        protected struct ExecuteContext
        {
            private bool NeedClose;
            private bool IsSuccessful;
            private DbAccessCommand DbCommand;
            public ExecuteContext(DbAccessCommand dbCommand)
            {
                this.NeedClose = false;
                this.IsSuccessful = false;
                this.DbCommand = dbCommand;
                if (this.DbCommand.AutoOpenClose && (this.DbCommand.DbConnection.State != ConnectionState.Open))
                {
                    this.DbCommand.Open();
                    this.NeedClose = true;
                }
            }

            public void SuccessfullyCall()
            {
                this.IsSuccessful = true;
            }

            public void FinallyCall()
            {
                if (this.DbCommand.AutoOpenClose)
                {
                    if (!this.IsSuccessful)
                    {
                        this.DbCommand.RollbackTransaction();
                    }
                    if (!(!this.NeedClose && this.IsSuccessful))
                    {
                        this.DbCommand.Close();
                    }
                }
            }
        }
    }
}

