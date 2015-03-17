using SqlMigratorWinform.Utility;
using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace SqlMigratorWinform.DataAccess
{
    public sealed class OleDbAccessCommand : DbAccessCommand
    {
        private static bool IsTraced = Tracer.Instance.IsTraced(typeof(OleDbAccessCommand));
        private static string TraceClass = typeof(OleDbAccessCommand).FullName;

        public OleDbAccessCommand()
        {
        }

        public OleDbAccessCommand(string connectionString) : base(connectionString)
        {
        }

        public override DbParameter CreateParameter(string parameterName, object value)
        {
            return new OleDbParameter(parameterName, value);
        }

        public override DbParameter CreateParameter(string parameterName, int providerType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value)
        {
            return new OleDbParameter(parameterName, (OleDbType) providerType, size, direction, isNullable, precision, scale, srcColumn, srcVersion, value);
        }

        protected override void DeriveParameters(DbCommand dbCommand)
        {
            OleDbCommandBuilder.DeriveParameters((OleDbCommand) dbCommand);
        }

        public static OleDbAccessCommand GetInstance(string connectionString)
        {
            return DbAccessCommand.GetInstance<OleDbAccessCommand>(connectionString);
        }

        protected override DbDataAdapter InitDbAdapter()
        {
            return new OleDbDataAdapter((OleDbCommand) base.DbCommand);
        }

        protected override DbCommandBuilder InitDbBuilder()
        {
            return new OleDbCommandBuilder((OleDbDataAdapter) base.DbAdapter);
        }

        protected override DbConnection InitDbConnection()
        {
            return new OleDbConnection();
        }

        public override string ParameterPrefix
        {
            get
            {
                return string.Empty;
            }
        }
    }
}

