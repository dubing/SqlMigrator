using SqlMigratorWinform.Utility;
using System;
using System.Data;
using System.Data.Common;
using System.Xml;

namespace SqlMigratorWinform.DataAccess
{


    public sealed class DbAccessor : IDisposable
    {
        private DbAccessType _AccessType;
        private string _ConnectionString;
        private DbAccessCommand DbCommand;

        public DbAccessor()
        {
            this._AccessType = DbAccessType.Sql;
        }

        public DbAccessor(DbAccessType accessType, string connectionString) : this()
        {
            this._AccessType = accessType;
            this._ConnectionString = connectionString;
            this.CreateAccessCommand();
        }

        public void BeginTransaction()
        {
            this.DbCommand.BeginTransaction();
        }

        public void Close()
        {
            this.DbCommand.Close();
        }

        public void CommitTransaction()
        {
            this.DbCommand.CommitTransaction();
        }

        private DbAccessCommand CreateAccessCommand()
        {
            switch (this.AccessType)
            {
                case DbAccessType.Odbc:
                    this.DbCommand = OdbcAccessCommand.GetInstance(this.ConnectionString);
                    break;

                case DbAccessType.OleDb:
                    this.DbCommand = OleDbAccessCommand.GetInstance(this.ConnectionString);
                    break;

                case DbAccessType.Oracle:
                    this.DbCommand = OracleAccessCommand.GetInstance(this.ConnectionString);
                    break;

                case DbAccessType.Sql:
                    this.DbCommand = SqlAccessCommand.GetInstance(this.ConnectionString);
                    break;

                default:
                    throw new NotSupportedException(this.AccessType.ToString());
            }
            return this.DbCommand;
        }

        public void Dispose()
        {
            if (this.DbCommand != null)
            {
                this.DbCommand.Dispose();
            }
        }

        public object ExecuteCommand(Func<DbAccessCommand, object> commandHandler)
        {
            if (commandHandler == null)
            {
                return null;
            }
            object result = null;
            if (commandHandler != null)
            {
                result = commandHandler(this.DbCommand);
            }
            return result;
        }

        public DataRow ExecuteDataRow(DbAccessInformation accessInfo)
        {
            if (accessInfo == null)
            {
                return null;
            }
            DataRow result = null;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteDataRow();
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public DataRow ExecuteDataRow(DbAccessInformation accessInfo, DataTable dataTable)
        {
            if (accessInfo == null)
            {
                return null;
            }
            DataRow result = null;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteDataRow(dataTable);
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public DataSet ExecuteDataSet(DbAccessInformation accessInfo)
        {
            if (accessInfo == null)
            {
                return null;
            }
            DataSet result = null;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteDataSet();
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public int ExecuteDataSet(DbAccessInformation accessInfo, DataSet dataSet)
        {
            if (accessInfo == null)
            {
                return -1;
            }
            int result = -1;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteDataSet(dataSet);
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public int ExecuteDataSet(DbAccessInformation accessInfo, DataSet dataSet, string srcTable)
        {
            if (accessInfo == null)
            {
                return -1;
            }
            int result = -1;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteDataSet(dataSet, srcTable);
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public int ExecuteDataSet(DbAccessInformation accessInfo, DataSet dataSet, int startRecord, int maxRecords, string srcTable)
        {
            if (accessInfo == null)
            {
                return 0;
            }
            int result = 0;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteDataSet(dataSet, startRecord, maxRecords, srcTable);
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public DataTable ExecuteDataTable(DbAccessInformation accessInfo)
        {
            if (accessInfo == null)
            {
                return null;
            }
            DataTable result = null;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteDataTable();
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public int ExecuteDataTable(DbAccessInformation accessInfo, DataTable dataTable)
        {
            if (accessInfo == null)
            {
                return -1;
            }
            int result = -1;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteDataTable(dataTable);
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public DataTable ExecuteDataTable(DbAccessInformation accessInfo, int startRecord, int maxRecords)
        {
            if (accessInfo == null)
            {
                return null;
            }
            DataTable result = null;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteDataTable(startRecord, maxRecords);
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public int ExecuteDataTable(DbAccessInformation accessInfo, DataTable dataTable, int startRecord, int maxRecords)
        {
            if (accessInfo == null)
            {
                return -1;
            }
            int result = -1;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteDataTable(dataTable, startRecord, maxRecords);
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public int ExecuteNonQuery(DbAccessInformation accessInfo)
        {
            if (accessInfo == null)
            {
                return -1;
            }
            int result = 0;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteNonQuery();
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public object ExecuteReader(DbAccessInformation accessInfo, Func<DbDataReader, object> readerHandler)
        {
            if ((accessInfo == null) || (readerHandler == null))
            {
                return null;
            }
            object result = null;
            this.DbCommand.AttachAccessInfo(accessInfo);
            DbDataReader reader = this.DbCommand.ExecuteReader();
            if (readerHandler != null)
            {
                result = readerHandler(reader);
            }
            if (!reader.IsClosed)
            {
                reader.Close();
            }
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public object ExecuteReader(DbAccessInformation accessInfo, Func<DbDataReader, object> readerHandler, int startRecord)
        {
            if ((accessInfo == null) || (readerHandler == null))
            {
                return null;
            }
            object result = null;
            this.DbCommand.AttachAccessInfo(accessInfo);
            DbDataReader reader = this.DbCommand.ExecuteReader(startRecord);
            if (readerHandler != null)
            {
                result = readerHandler(reader);
            }
            if (!reader.IsClosed)
            {
                reader.Close();
            }
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public object ExecuteScalar(DbAccessInformation accessInfo)
        {
            if (accessInfo == null)
            {
                return null;
            }
            object result = null;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteScalar();
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public DataTable ExecuteSchemaTable(DbAccessInformation accessInfo)
        {
            if (accessInfo == null)
            {
                return null;
            }
            DataTable result = null;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteSchemaTable();
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public DataTable[] ExecuteSetSchema(DbAccessInformation accessInfo)
        {
            if (accessInfo == null)
            {
                return null;
            }
            DataTable[] result = null;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteSetSchema();
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public DataTable ExecuteTableSchema(DbAccessInformation accessInfo)
        {
            if (accessInfo == null)
            {
                return null;
            }
            DataTable result = null;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteTableSchema();
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public XmlDocument ExecuteXml(XmlNode input)
        {
            XmlNodeList accessNodes = input.SelectNodes("DbAccessInformation");
            DbExcuteType[] exeTypes = new DbExcuteType[accessNodes.Count];
            DbAccessInformation[] accessInfos = new DbAccessInformation[accessNodes.Count];
            int i = 0;
            int j = accessNodes.Count;
            while (i < j)
            {
                XmlNode accessNode = accessNodes[i].CloneNode(true);
                DbExcuteType exeType = (DbExcuteType) DataConvert.ParseEnum(XmlHelper.GetAttributeValue(accessNode, "excuteType"), DbExcuteType.ExecuteNonQuery);
                DbAccessInformation accessInfo = SerializeHelper.XmlDeserialize(accessNode, typeof(DbAccessInformation)) as DbAccessInformation;
                exeTypes[i] = exeType;
                accessInfos[i] = accessInfo;
                i++;
            }
            XmlDocument doc = XmlHelper.CreateXmlDocument("ActionItems", null);
            bool needOpen = this.DbCommand.Connection.State != ConnectionState.Open;
            if (needOpen)
            {
                this.DbCommand.Open();
            }
            bool isFailed = true;
            try
            {
                this.InnerExecuteXml(doc, this.DbCommand, exeTypes, accessInfos);
                isFailed = false;
            }
            finally
            {
                if (isFailed && (this.DbCommand.Transaction != null))
                {
                    this.DbCommand.RollbackTransaction();
                }
                if (needOpen || isFailed)
                {
                    this.DbCommand.Close();
                }
            }
            return doc;
        }

        public XmlReader ExecuteXmlReader(DbAccessInformation accessInfo, bool isTransaction)
        {
            if (accessInfo == null)
            {
                return null;
            }
            XmlReader result = null;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.ExecuteXmlReader();
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        private void FillResultNode(XmlNode resultNode, DbAccessCommand dbCommand, DbExcuteType exeType)
        {
            object result = null;
            switch (exeType)
            {
                case DbExcuteType.ExecuteNonQuery:
                    result = this.DbCommand.ExecuteNonQuery();
                    break;

                case DbExcuteType.ExecuteReader:
                    result = this.DbCommand.ExecuteDataSet();
                    break;

                case DbExcuteType.ExecuteScalar:
                    result = this.DbCommand.ExecuteScalar();
                    break;

                case DbExcuteType.ExecuteXmlReader:
                {
                    XmlDocument tempDoc = new XmlDocument();
                    tempDoc.Load(this.DbCommand.ExecuteXmlReader());
                    result = tempDoc.DocumentElement;
                    break;
                }
                case DbExcuteType.ExecuteSchemaTable:
                    result = this.DbCommand.ExecuteSchemaTable();
                    break;

                case DbExcuteType.ExecuteDataTable:
                    result = this.DbCommand.ExecuteDataTable();
                    break;

                case DbExcuteType.ExecuteDataRow:
                    result = this.DbCommand.ExecuteDataRow().Table;
                    break;

                case DbExcuteType.ExecuteTableSchema:
                    result = this.DbCommand.ExecuteTableSchema();
                    break;

                case DbExcuteType.ExecuteDataSet:
                    result = this.DbCommand.ExecuteDataSet();
                    break;

                case DbExcuteType.ExecuteSetSchema:
                    result = this.DbCommand.ExecuteSetSchema();
                    break;
            }
            if (result is XmlNode)
            {
                XmlHelper.AddNode(resultNode, result as XmlNode);
            }
            else if (result != null)
            {
                XmlHelper.AddNode(resultNode, SerializeHelper.XmlSerialize(result));
            }
        }

        public static T GetAccessCommand<T>(string connectionString) where T: DbAccessCommand, new()
        {
            return Singleton<T>.GetThreadInstance(connectionString, delegate {
                T t = Activator.CreateInstance<T>();
                t.ConnectionString = connectionString;
                return t;
            });
        }

        private void InnerExecuteXml(XmlDocument doc, DbAccessCommand dbCommand, DbExcuteType[] exeTypes, DbAccessInformation[] accessInfos)
        {
            int i = 0;
            int j = exeTypes.Length;
            while (i < j)
            {
                DbAccessInformation accessInfo = accessInfos[i];
                XmlNode itemNode = XmlHelper.SetInnerText(doc.DocumentElement, "ActionItem", string.Empty);
                XmlHelper.SetAttribute(itemNode, "sequence", (i + 1).ToString());
                dbCommand.AttachAccessInfo(accessInfo);
                XmlNode resultNode = XmlHelper.SetInnerText(itemNode, "Result", string.Empty);
                this.FillResultNode(resultNode, dbCommand, exeTypes[i]);
                dbCommand.PickupParameteValues(accessInfo);
                XmlHelper.AddNode(itemNode, SerializeHelper.XmlSerialize(accessInfo));
                i++;
            }
        }

        public void Open()
        {
            this.DbCommand.Open();
        }

        public int UpdateDataRow(DbAccessInformation accessInfo, params DataRow[] dataRows)
        {
            if (accessInfo == null)
            {
                return -1;
            }
            int result = -1;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.UpdateDataRow(dataRows);
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public int UpdateDataTable(DbAccessInformation accessInfo, DataTable dataTable)
        {
            if (accessInfo == null)
            {
                return -1;
            }
            int result = -1;
            this.DbCommand.AttachAccessInfo(accessInfo);
            result = this.DbCommand.UpdateDataTable(dataTable);
            this.DbCommand.PickupParameteValues(accessInfo);
            return result;
        }

        public DbAccessType AccessType
        {
            get
            {
                return this._AccessType;
            }
            set
            {
                this._AccessType = value;
                this.CreateAccessCommand();
            }
        }

        public string ConnectionString
        {
            get
            {
                return this._ConnectionString;
            }
            set
            {
                this._ConnectionString = value;
                this.CreateAccessCommand();
            }
        }

        public static DbAccessor Instance
        {
            get
            {
                return Singleton<DbAccessor>.GetThreadInstance();
            }
        }

        public enum DbExcuteType
        {
            ExecuteNonQuery,
            ExecuteReader,
            ExecuteScalar,
            ExecuteXmlReader,
            ExecuteSchemaTable,
            ExecuteDataTable,
            ExecuteDataRow,
            ExecuteTableSchema,
            ExecuteDataSet,
            ExecuteSetSchema
        }
    }
}

