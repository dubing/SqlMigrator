using System;
using System.Diagnostics;
using System.Text;
using System.Web;

namespace SqlMigratorWinform.Utility
{
    [Serializable]
    public class TraceObject
    {
        private string _Description;
        private string _Detail;
        private TraceLevel _Level;
        private string _Message;
        private string _TraceClass;
        private string _TraceMethod;
        private DateTime _TraceTime;

        public TraceObject()
        {
            this._Level = TraceLevel.Verbose;
            this._TraceTime = DateTime.Now;
        }

        public TraceObject(string traceClass, string traceMethod, string message)
        {
            this._Level = TraceLevel.Verbose;
            this._TraceTime = DateTime.Now;
            this.TraceClass = traceClass;
            this.TraceMethod = traceMethod;
            this.Message = message;
        }

        public TraceObject(string traceClass, string traceMethod, string description, string detail)
        {
            this._Level = TraceLevel.Verbose;
            this._TraceTime = DateTime.Now;
            this.TraceClass = traceClass;
            this.TraceMethod = traceMethod;
            this.Description = description;
            this.Detail = detail;
        }

        private void AppendString(StringBuilder sb, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (sb.Length > 0)
                {
                    sb.Append('&');
                }
                sb.Append(name);
                sb.Append('=');
                sb.Append(HttpUtility.UrlEncode(value));
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            this.AppendString(sb, "TraceClass", this.TraceClass);
            this.AppendString(sb, "TraceMethod", this.TraceMethod);
            this.AppendString(sb, "Message", this.Message);
            this.AppendString(sb, "Description", this.Description);
            this.AppendString(sb, "Detail", this.Detail);
            this.AppendString(sb, "Level", this.Level.ToString());
            this.AppendString(sb, "TraceTime", this.TraceTime.ToString());
            return sb.ToString();
        }

        public string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                this._Description = value;
            }
        }

        public string Detail
        {
            get
            {
                return this._Detail;
            }
            set
            {
                this._Detail = value;
            }
        }

        public TraceLevel Level
        {
            get
            {
                return this._Level;
            }
            set
            {
                this._Level = value;
            }
        }

        public string Message
        {
            get
            {
                return this._Message;
            }
            set
            {
                this._Message = value;
            }
        }

        public string TraceClass
        {
            get
            {
                return this._TraceClass;
            }
            set
            {
                this._TraceClass = value;
            }
        }

        public string TraceMethod
        {
            get
            {
                return this._TraceMethod;
            }
            set
            {
                this._TraceMethod = value;
            }
        }

        public DateTime TraceTime
        {
            get
            {
                return this._TraceTime;
            }
            set
            {
                this._TraceTime = value;
            }
        }
    }
}

