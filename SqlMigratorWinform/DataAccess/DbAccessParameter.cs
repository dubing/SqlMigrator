using SqlMigratorWinform.Utility;
using System;
using System.Data;
using System.Data.Common;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SqlMigratorWinform.DataAccess
{

    [Serializable]
    public sealed class DbAccessParameter : MarshalByRefObject, IXmlSerializable
    {
        private System.Data.DbType _DbType;
        private ParameterDirection _Direction;
        private bool _IsNullable;
        private string _ParameterName;
        private byte _Precision;
        private int _ProvideType;
        private byte _Scale;
        private int _Size;
        private string _SourceColumn;
        private bool _SourceColumnNullMapping;
        private DataRowVersion _SourceVersion;
        private object _Value;
        private const int COUNT = 12;
        private const int DBTYPE = 1;
        private const int DIRECTION = 3;
        private const int ISNULLABLE = 4;
        private const int PARAMETERNAME = 0;
        private const int PRECISION = 5;
        private bool[] Properties;
        private const int PROVIDETYPE = 11;
        private const int SCALE = 6;
        private const int SIZE = 2;
        private const int SOURCECOLUMN = 7;
        private const int SOURCECOLUMNNULLMAPPING = 10;
        private const int SOURCEVERSION = 8;
        private const int VALUE = 9;

        static DbAccessParameter()
        {
            Type type = typeof(DbAccessParameter);
            SerializeHelper.KnownTypes[type.Name] = type;
        }

        public DbAccessParameter()
        {
            this._DbType = System.Data.DbType.String;
            this._ProvideType = -1;
            this._Direction = ParameterDirection.Input;
            this._ParameterName = string.Empty;
            this._Size = 0;
            this._Value = null;
            this._IsNullable = false;
            this._SourceColumn = string.Empty;
            this._SourceColumnNullMapping = false;
            this._SourceVersion = DataRowVersion.Current;
            this._Precision = 0;
            this._Scale = 0;
            this.Properties = new bool[12];
        }

        public DbAccessParameter(DbParameter parameter) : this()
        {
            ParameterChecker.CheckNull("DbAccessParameter", "parameter", parameter);
            ParameterChecker.CheckNullOrEmpty("DbAccessParameter", "parameter.ParameterName", parameter.ParameterName);
            this.ParameterName = parameter.ParameterName;
            this.DbType = parameter.DbType;
            this.Size = parameter.Size;
            this.Direction = parameter.Direction;
            this.IsNullable = parameter.IsNullable;
            this.Precision = ((IDbDataParameter) parameter).Precision;
            this.Scale = ((IDbDataParameter) parameter).Scale;
            this.SourceColumn = parameter.SourceColumn;
            this.SourceVersion = parameter.SourceVersion;
            this.Value = parameter.Value;
            this.SourceColumnNullMapping = parameter.SourceColumnNullMapping;
        }

        public DbAccessParameter(string parameterName) : this()
        {
            ParameterChecker.CheckNullOrEmpty("DbAccessParameter", "parameterName", parameterName);
            this.ParameterName = parameterName;
        }

        public DbAccessParameter(string parameterName, System.Data.DbType dbType) : this(parameterName)
        {
            this.DbType = dbType;
        }

        public DbAccessParameter(string parameterName, object parameterValue) : this(parameterName)
        {
            this.Value = parameterValue;
        }

        public DbAccessParameter(string parameterName, System.Data.DbType dbType, int size) : this(parameterName, dbType)
        {
            if (size > 0)
            {
                this.Size = size;
            }
        }

        public DbAccessParameter(string parameterName, object parameterValue, System.Data.DbType dbType, ParameterDirection parameterDirection) : this(parameterName, dbType)
        {
            this.Value = parameterValue;
            this.Direction = parameterDirection;
        }

        public DbAccessParameter(string parameterName, object parameterValue, System.Data.DbType dbType, int size, ParameterDirection parameterDirection) : this(parameterName, dbType, size)
        {
            this.Value = parameterValue;
            this.Direction = parameterDirection;
        }

        public DbAccessParameter(string parameterName, int providerType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value) : this(parameterName)
        {
            this.ProvideType = providerType;
            this.Size = size;
            this.Direction = direction;
            this.IsNullable = isNullable;
            this.Precision = precision;
            this.Scale = scale;
            this.SourceColumn = srcColumn;
            this.SourceVersion = srcVersion;
            this.Value = value;
        }

        public DbParameter ConvertParameter(DbParameter parameter)
        {
            if (parameter != null)
            {
                if (this.Properties[0])
                {
                    parameter.ParameterName = this.ParameterName;
                }
                if (this.Properties[1])
                {
                    parameter.DbType = this.DbType;
                }
                if (this.Properties[2])
                {
                    parameter.Size = this.Size;
                }
                if (this.Properties[3])
                {
                    parameter.Direction = this.Direction;
                }
                if (this.Properties[4])
                {
                    parameter.IsNullable = this.IsNullable;
                }
                if (this.Properties[5])
                {
                    ((IDbDataParameter) parameter).Precision = this.Precision;
                }
                if (this.Properties[6])
                {
                    ((IDbDataParameter) parameter).Scale = this.Scale;
                }
                if (this.Properties[7])
                {
                    parameter.SourceColumn = this.SourceColumn;
                }
                if (this.Properties[8])
                {
                    parameter.SourceVersion = this.SourceVersion;
                }
                if (this.Properties[9])
                {
                    parameter.Value = this.Value;
                }
                if (this.Properties[10])
                {
                    parameter.SourceColumnNullMapping = this.SourceColumnNullMapping;
                }
            }
            return parameter;
        }

        public void ResetDbType()
        {
            if (this.Properties[1])
            {
                this.DbType = System.Data.DbType.String;
            }
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            SerializeHelper.XmlDeserializeChildElements(reader, delegate (XmlReader _reader, object[] _args) {
                switch (reader.Name)
                {
                    case "ParameterName":
                        this.ParameterName = reader.ReadElementString();
                        break;

                    case "DbType":
                        this.DbType = (System.Data.DbType) DataConvert.ParseEnum(reader.ReadElementString(), System.Data.DbType.String);
                        break;

                    case "Size":
                        this.Size = DataConvert.ParseInt(reader.ReadElementString(), 0).Value;
                        break;

                    case "Direction":
                        this.Direction = (ParameterDirection) DataConvert.ParseEnum(reader.ReadElementString(), ParameterDirection.Input);
                        break;

                    case "IsNullable":
                        this.IsNullable = DataConvert.ParseBool(reader.ReadElementString(), false).Value;
                        break;

                    case "Precision":
                        this.Precision = DataConvert.ParseByte(reader.ReadElementString(), 0).Value;
                        break;

                    case "Scale":
                        this.Scale = DataConvert.ParseByte(reader.ReadElementString(), 0).Value;
                        break;

                    case "SourceColumn":
                        this.SourceColumn = reader.ReadElementString();
                        break;

                    case "SourceVersion":
                        this.SourceVersion = (DataRowVersion) DataConvert.ParseEnum(reader.ReadElementString(), DataRowVersion.Current);
                        break;

                    case "Value":
                        if (reader.Read())
                        {
                            this.Value = SerializeHelper.XmlReaderDeserialize(reader);
                            if (!reader.EOF)
                            {
                                reader.Read();
                            }
                        }
                        break;

                    case "SourceColumnNullMapping":
                        this.SourceColumnNullMapping = DataConvert.ParseBool(reader.ReadElementString(), false).Value;
                        break;

                    case "ProvideType":
                        this.ProvideType = DataConvert.ParseInt(reader.ReadElementString(), -1).Value;
                        break;
                }
            }, new object[0]);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (this.Properties[0])
            {
                writer.WriteElementString("ParameterName", this.ParameterName);
            }
            if (this.Properties[1])
            {
                writer.WriteElementString("DbType", this.DbType.ToString());
            }
            if (this.Properties[2])
            {
                writer.WriteElementString("Size", this.Size.ToString());
            }
            if (this.Properties[3])
            {
                writer.WriteElementString("Direction", this.Direction.ToString());
            }
            if (this.Properties[4])
            {
                writer.WriteElementString("IsNullable", this.IsNullable.ToString());
            }
            if (this.Properties[5])
            {
                writer.WriteElementString("Precision", this.Precision.ToString());
            }
            if (this.Properties[6])
            {
                writer.WriteElementString("Scale", this.Scale.ToString());
            }
            if (this.Properties[7])
            {
                writer.WriteElementString("SourceColumn", this.SourceColumn);
            }
            if (this.Properties[8])
            {
                writer.WriteElementString("SourceVersion", this.SourceVersion.ToString());
            }
            if (this.Properties[9])
            {
                writer.WriteStartElement("Value");
                SerializeHelper.XmlWriterSerialize(writer, this.Value);
                writer.WriteEndElement();
            }
            if (this.Properties[10])
            {
                writer.WriteElementString("SourceColumnNullMapping", this.SourceColumnNullMapping.ToString());
            }
            if (this.Properties[11])
            {
                writer.WriteElementString("ProvideType", this.ProvideType.ToString());
            }
        }

        public System.Data.DbType DbType
        {
            get
            {
                return this._DbType;
            }
            set
            {
                this.Properties[1] = true;
                this._DbType = value;
            }
        }

        public ParameterDirection Direction
        {
            get
            {
                return this._Direction;
            }
            set
            {
                this.Properties[3] = true;
                this._Direction = value;
            }
        }

        public bool IsNullable
        {
            get
            {
                return this._IsNullable;
            }
            set
            {
                this.Properties[4] = true;
                this._IsNullable = value;
            }
        }

        public string ParameterName
        {
            get
            {
                return this._ParameterName;
            }
            set
            {
                this.Properties[0] = true;
                this._ParameterName = value;
            }
        }

        public byte Precision
        {
            get
            {
                return this._Precision;
            }
            set
            {
                this.Properties[5] = true;
                this._Precision = value;
            }
        }

        public int ProvideType
        {
            get
            {
                return this._ProvideType;
            }
            set
            {
                this.Properties[11] = true;
                this._ProvideType = value;
            }
        }

        public byte Scale
        {
            get
            {
                return this._Scale;
            }
            set
            {
                this.Properties[6] = true;
                this._Scale = value;
            }
        }

        public int Size
        {
            get
            {
                return this._Size;
            }
            set
            {
                this.Properties[2] = true;
                this._Size = value;
            }
        }

        public string SourceColumn
        {
            get
            {
                return this._SourceColumn;
            }
            set
            {
                this.Properties[7] = true;
                this._SourceColumn = value;
            }
        }

        public bool SourceColumnNullMapping
        {
            get
            {
                return this._SourceColumnNullMapping;
            }
            set
            {
                this.Properties[10] = true;
                this._SourceColumnNullMapping = value;
            }
        }

        public DataRowVersion SourceVersion
        {
            get
            {
                return this._SourceVersion;
            }
            set
            {
                this.Properties[8] = true;
                this._SourceVersion = value;
            }
        }

        public object Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this.Properties[9] = true;
                this._Value = (value == null) ? DBNull.Value : value;
            }
        }
    }
}

