using SqlMigratorWinform.Utility;
using System;
using System.Data;
using System.Data.Common;

namespace SqlMigratorWinform.DataAccess
{
    [Serializable]
    public sealed class DbAccessInformation : MarshalByRefObject
    {
        private string _CommandText;
        private int _CommandTimeout;
        private CommandType _CommandType;
        private DbAccessParameterCollection _Parameters;

        static DbAccessInformation()
        {
            Type type = typeof (DbAccessInformation);
            SerializeHelper.KnownTypes[type.Name] = type;
        }

        public DbAccessInformation()
        {
            this._CommandText = null;
            this._CommandType = CommandType.Text;
            this._CommandTimeout = 30;
            this._Parameters = new DbAccessParameterCollection();
        }

        public DbAccessInformation(string commandText) : this(commandText, CommandType.Text)
        {
        }

        public DbAccessInformation(string commandText, System.Data.CommandType commandType)
            : this(commandText, commandType, 30)
        {
        }

        public DbAccessInformation(string commandText, System.Data.CommandType commandType, int commandTimeout) : this()
        {
            ParameterChecker.CheckNullOrEmpty("DbAccessInformation", "commandText", commandText);
            this._CommandText = commandText;
            this._CommandType = commandType;
            if (commandTimeout > 0)
            {
                this._CommandTimeout = commandTimeout;
            }
        }

        public void AddParameter(DbAccessParameter parameter)
        {
            ParameterChecker.CheckNull("DbAccessInformation.AddParameter", "parameter", parameter);
            this._Parameters.Add(parameter);
        }

        public void AddParameter(DbParameter parameter)
        {
            this._Parameters.Add(new DbAccessParameter(parameter));
        }

        public void AddParameter(string parameterName, DbType dbType)
        {
            this._Parameters.Add(new DbAccessParameter(parameterName, dbType));
        }

        public void AddParameter(string parameterName, object parameterValue)
        {
            this._Parameters.Add(new DbAccessParameter(parameterName, parameterValue));
        }

        public void AddParameter(string parameterName, DbType dbType, int size)
        {
            this._Parameters.Add(new DbAccessParameter(parameterName, dbType, size));
        }

        public void AddParameter(string parameterName, object parameterValue, DbType dbType,
                                 ParameterDirection parameterDirection)
        {
            this._Parameters.Add(new DbAccessParameter(parameterName, parameterValue, dbType, parameterDirection));
        }

        public void AddParameter(string parameterName, object parameterValue, DbType dbType, int size,
                                 ParameterDirection parameterDirection)
        {
            this._Parameters.Add(new DbAccessParameter(parameterName, parameterValue, dbType, size, parameterDirection));
        }

        public void AddParameter(string parameterName, int providerType, int size, ParameterDirection direction,
                                 bool isNullable, byte precision, byte scale, string srcColumn,
                                 DataRowVersion srcVersion, object value)
        {
            this._Parameters.Add(new DbAccessParameter(parameterName, providerType, size, direction, isNullable,
                                                       precision, scale, srcColumn, srcVersion, value));
        }

        public object GetParameterValue(int parameterIndex)
        {
            DbAccessParameter parameter = this._Parameters[parameterIndex];
            return ((parameter == null) ? null : parameter.Value);
        }

        public object GetParameterValue(string parameterName)
        {
            DbAccessParameter parameter = this._Parameters[parameterName];
            return ((parameter == null) ? null : parameter.Value);
        }

        public object GetReturnValue()
        {
            foreach (DbAccessParameter parameter in this.Parameters)
            {
                if (parameter.Direction == ParameterDirection.ReturnValue)
                {
                    return parameter.Value;
                }
            }
            return null;
        }

        public string CommandText
        {
            get { return this._CommandText; }
            set { this._CommandText = value; }
        }

        public int CommandTimeout
        {
            get { return this._CommandTimeout; }
            set { this._CommandTimeout = value; }
        }

        public System.Data.CommandType CommandType
        {
            get { return this._CommandType; }
            set { this._CommandType = value; }
        }

        public DbAccessParameterCollection Parameters
        {
            get { return this._Parameters; }
            set { this._Parameters = value; }
        }
    }
}