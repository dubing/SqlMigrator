using SqlMigratorWinform.Utility;
using System;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;

namespace SqlMigratorWinform.DataAccess
{


    public sealed class OracleAccessCommand : DbAccessCommand
    {
        private static bool IsTraced = Tracer.Instance.IsTraced(typeof(OracleAccessCommand));
        private static string TraceClass = typeof(OracleAccessCommand).FullName;

        public OracleAccessCommand()
        {
        }

        public OracleAccessCommand(string connectionString) : base(connectionString)
        {
        }

        public override DbParameter CreateParameter(string parameterName, object value)
        {
            return new OracleParameter(parameterName, value);
        }

        public override DbParameter CreateParameter(string parameterName, int providerType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value)
        {
            return new OracleParameter(parameterName, (OracleType) providerType, size, direction, isNullable, precision, scale, srcColumn, srcVersion, value);
        }

        protected override void DeriveParameters(DbCommand dbCommand)
        {
            OracleCommandBuilder.DeriveParameters((OracleCommand) dbCommand);
        }

        public static OracleAccessCommand GetInstance(string connectionString)
        {
            return DbAccessCommand.GetInstance<OracleAccessCommand>(connectionString);
        }

        protected override DbDataAdapter InitDbAdapter()
        {
            return new OracleDataAdapter((OracleCommand) base.DbCommand);
        }

        protected override DbCommandBuilder InitDbBuilder()
        {
            return new OracleCommandBuilder((OracleDataAdapter) base.DbAdapter);
        }

        protected override DbConnection InitDbConnection()
        {
            return new OracleConnection();
        }

        public override string ParameterPrefix
        {
            get
            {
                return ":";
            }
        }
    }
}

