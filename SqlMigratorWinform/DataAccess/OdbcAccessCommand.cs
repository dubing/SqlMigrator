using SqlMigratorWinform.Utility;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;

namespace SqlMigratorWinform.DataAccess
{
    public sealed class OdbcAccessCommand : DbAccessCommand
    {
        private static bool IsTraced = Tracer.Instance.IsTraced(typeof(OdbcAccessCommand));
        private static string TraceClass = typeof(OdbcAccessCommand).FullName;

        public OdbcAccessCommand()
        {
        }

        public OdbcAccessCommand(string connectionString) : base(connectionString)
        {
        }

        public override DbParameter CreateParameter(string parameterName, object value)
        {
            return new OdbcParameter(parameterName, value);
        }

        public override DbParameter CreateParameter(string parameterName, int providerType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value)
        {
            return new OdbcParameter(parameterName, (OdbcType) providerType, size, direction, isNullable, precision, scale, srcColumn, srcVersion, value);
        }

        protected override void DeriveParameters(DbCommand dbCommand)
        {
            OdbcCommandBuilder.DeriveParameters((OdbcCommand) dbCommand);
        }

        public static OdbcAccessCommand GetInstance(string connectionString)
        {
            return DbAccessCommand.GetInstance<OdbcAccessCommand>(connectionString);
        }

        protected override DbDataAdapter InitDbAdapter()
        {
            return new OdbcDataAdapter((OdbcCommand) base.DbCommand);
        }

        protected override DbCommandBuilder InitDbBuilder()
        {
            return new OdbcCommandBuilder((OdbcDataAdapter) base.DbAdapter);
        }

        protected override DbConnection InitDbConnection()
        {
            return new OdbcConnection();
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

