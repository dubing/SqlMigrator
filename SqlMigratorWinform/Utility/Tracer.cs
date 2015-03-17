using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web;
using System.Xml;

namespace SqlMigratorWinform.Utility
{
    public sealed class Tracer
    {
        private bool _HasInitialized = false;
        private string Category;
        private Converter<TraceObject, SortedList> OnWriteTrace;
        private Dictionary<string, bool> TraceItems = new Dictionary<string, bool>();
        private DataProcessProxy<SortedList> TracerProxy;

        public Tracer()
        {
            this.TracerProxy = new DataProcessProxy<SortedList>(new DataProcessHandler<SortedList>(this.OnProxyTrace));
        }

        private string CreateTraceDetail(string[] names, object[] values)
        {
            SortedList parameters = new SortedList();
            for (int i = 0; i < names.Length; i++)
            {
                parameters.Add(names[i], values[i]);
            }
            return SerializeHelper.XmlSerialize(parameters).OuterXml;
        }

        public void EnterFunction(string traceClass, string traceMethod, string message)
        {
            this.Write(new TraceObject(traceClass, traceMethod, "EnterFunction", message));
        }

        public void EnterFunction(string traceClass, string traceMethod, string[] names, object[] values)
        {
            this.Write(new TraceObject(traceClass, traceMethod, "EnterFunction", this.CreateTraceDetail(names, values)));
        }

        public void Initialize(string category, Dictionary<string, bool> traceItems, Converter<TraceObject, SortedList> onWriteTrace)
        {
            ParameterChecker.CheckNull("Tracer.Initialize", "traceItems", traceItems);
            this.Category = category;
            this.TraceItems = traceItems;
            if (onWriteTrace == null)
            {
                this.OnWriteTrace = new Converter<TraceObject, SortedList>(this.OnDefaultWriteTrace);
            }
            else
            {
                this.OnWriteTrace = onWriteTrace;
            }
            this._HasInitialized = true;
        }

        public bool IsTraced(Type type)
        {
            while (type != null)
            {
                bool isTraced;
                if (this.TraceItems.TryGetValue(type.FullName, out isTraced))
                {
                    return isTraced;
                }
                type = type.BaseType;
            }
            return false;
        }

        public void LeaveFunction(string traceClass, string traceMethod, string message)
        {
            this.Write(new TraceObject(traceClass, traceMethod, "LeaveFunction", message));
        }

        public void LeaveFunction(string traceClass, string traceMethod, string[] names, object[] values)
        {
            this.Write(new TraceObject(traceClass, traceMethod, "LeaveFunction", this.CreateTraceDetail(names, values)));
        }

        public static Tracer NewInstance(string category, Dictionary<string, bool> traceItems, Converter<TraceObject, SortedList> onWriteTrace)
        {
            Tracer tracer = new Tracer();
            tracer.Initialize(category, traceItems, onWriteTrace);
            return tracer;
        }

        private SortedList OnDefaultWriteTrace(TraceObject traceObj)
        {
            SortedList parameters = new SortedList();
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                parameters.Add("RequestType", context.Request.RequestType);
                parameters.Add("HttpMethod", context.Request.HttpMethod);
                parameters.Add("Url", DataConvert.ToString(context.Request.Url));
                parameters.Add("UrlReferrer", DataConvert.ToString(context.Request.UrlReferrer));
                parameters.Add("LogonUser", context.Request.LogonUserIdentity.Name);
                parameters.Add("QueryString", context.Request.QueryString.ToString());
                parameters.Add("Form", context.Request.Form.ToString());
                NameValueCollection cookies = new NameValueCollection();
                for (int i = 0; i < context.Request.Cookies.Count; i++)
                {
                    HttpCookie cookie = context.Request.Cookies[i];
                    cookies.Add(cookie.Name, cookie.Value);
                }
                parameters.Add("Cookies", cookies.ToString());
                if (context.Items.Contains("TraceContext"))
                {
                    parameters.Add("TraceContext", context.Items["TraceContext"]);
                }
            }
            parameters.Add("TraceObject", traceObj);
            return parameters;
        }

        private void OnProxyTrace(object entry)
        {
            if (entry != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(this.Category))
                    {
                        Trace.WriteLine(entry);
                    }
                    else
                    {
                        Trace.WriteLine(entry, this.Category);
                    }
                    Trace.Flush();
                }
                catch
                {
                }
            }
        }

        public void Write(TraceObject traceData)
        {
            if (this.OnWriteTrace != null)
            {
                this.TracerProxy.Process(this.OnWriteTrace(traceData));
            }
        }

        public void Write(object instance, string message)
        {
            if (instance != null)
            {
                Type type = instance.GetType();
                if (this.IsTraced(type))
                {
                    this.Write(new TraceObject(type.Name, string.Empty, message));
                }
            }
        }

        public void Write(string traceClass, string traceMethod, Exception ex)
        {
            this.Write(traceClass, traceMethod, ex, -2147483648);
        }

        public void Write(string traceClass, string traceMethod, object value)
        {
            this.Write(new TraceObject(traceClass, traceMethod, "Trace value", SerializeHelper.XmlSerialize(value).OuterXml));
        }

        public void Write(string traceClass, string traceMethod, string message)
        {
            TraceObject traceObj = new TraceObject(traceClass, traceMethod, message) {
                Level = TraceLevel.Info
            };
            this.Write(traceObj);
        }

        public void Write(string traceClass, string traceMethod, XmlNode node)
        {
            this.Write(new TraceObject(traceClass, traceMethod, "Trace XmlNode", node.OuterXml));
        }

        public void Write(string traceClass, string traceMethod, string[] names, object[] values)
        {
            this.Write(new TraceObject(traceClass, traceMethod, "Trace name-values", this.CreateTraceDetail(names, values)));
        }

        public void Write(string traceClass, string traceMethod, Exception ex, int errorCode)
        {
            TraceObject traceObj;
            if (errorCode > -2147483648)
            {
                traceObj = new TraceObject(traceClass, traceMethod, "Trace exception", ex.ToString());
            }
            else
            {
                traceObj = new TraceObject(traceClass, traceMethod, "Trace exception", string.Concat(new object[] { "ErrorCode=", errorCode, ":", ex.ToString() }));
            }
            traceObj.Level = TraceLevel.Error;
            this.Write(traceObj);
        }

        public bool HasInitialized
        {
            get
            {
                return this._HasInitialized;
            }
        }

        public static Tracer Instance
        {
            get
            {
                return Singleton<Tracer>.GetInstance();
            }
        }
    }
}

