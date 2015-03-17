using SqlMigratorWinform.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace SqlMigratorWinform.DataAccess
{
    [Serializable]
    public sealed class DbAccessParameterCollection : MarshalByRefObject, IDataParameterCollection, IList, ICollection, IEnumerable
    {
        private List<DbAccessParameter> Parameters = new List<DbAccessParameter>();

        static DbAccessParameterCollection()
        {
            Type type = typeof(DbAccessParameterCollection);
            SerializeHelper.KnownTypes[type.Name] = type;
        }

        public void Add(DbAccessParameter value)
        {
            this.Parameters.Add(value);
        }

        public void AddRange(params DbAccessParameter[] values)
        {
            if ((values != null) && (values.Length > 0))
            {
                this.Parameters.AddRange(values);
            }
        }

        public void Clear()
        {
            this.Parameters.Clear();
        }

        public bool Contains(DbAccessParameter value)
        {
            return this.Parameters.Contains(value);
        }

        public bool Contains(string parameterName)
        {
            return (this.IndexOf(parameterName) >= 0);
        }

        public IEnumerator GetEnumerator()
        {
            return this.Parameters.GetEnumerator();
        }

        public int IndexOf(DbAccessParameter value)
        {
            return this.Parameters.IndexOf(value);
        }

        public int IndexOf(string parameterName)
        {
            int i = 0;
            int j = this.Parameters.Count;
            while (i < j)
            {
                if (string.Equals(parameterName, this.Parameters[i].ParameterName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        public void Insert(int index, DbAccessParameter value)
        {
            this.Parameters.Insert(index, value);
        }

        public void Remove(DbAccessParameter value)
        {
            this.Parameters.Remove(value);
        }

        public void RemoveAt(int index)
        {
            this.Parameters.RemoveAt(index);
        }

        public void RemoveAt(string parameterName)
        {
            int index = this.IndexOf(parameterName);
            if (index >= 0)
            {
                this.Parameters.RemoveAt(index);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.Parameters.CopyTo((DbAccessParameter[]) array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        int IList.Add(object value)
        {
            this.Add(value as DbAccessParameter);
            return this.Count;
        }

        void IList.Clear()
        {
            this.Clear();
        }

        bool IList.Contains(object value)
        {
            return this.Contains(value as DbAccessParameter);
        }

        int IList.IndexOf(object value)
        {
            return this.IndexOf(value as DbAccessParameter);
        }

        void IList.Insert(int index, object value)
        {
            this.Insert(index, value as DbAccessParameter);
        }

        void IList.Remove(object value)
        {
            this.Remove(value as DbAccessParameter);
        }

        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        bool IDataParameterCollection.Contains(string parameterName)
        {
            return this.Contains(parameterName);
        }

        int IDataParameterCollection.IndexOf(string parameterName)
        {
            return this.IndexOf(parameterName);
        }

        void IDataParameterCollection.RemoveAt(string parameterName)
        {
            this.RemoveAt(parameterName);
        }

        public DbAccessParameter[] ToArray()
        {
            return this.Parameters.ToArray();
        }

        public int Count
        {
            get
            {
                return this.Parameters.Count;
            }
        }

        public DbAccessParameter this[int index]
        {
            get
            {
                return this.Parameters[index];
            }
            set
            {
                this.Parameters[index] = value;
            }
        }

        public DbAccessParameter this[string parameterName]
        {
            get
            {
                int index = this.IndexOf(parameterName);
                return ((index >= 0) ? this[index] : null);
            }
            set
            {
                int index = this.IndexOf(parameterName);
                if (index >= 0)
                {
                    this.Parameters[index] = value;
                }
                else
                {
                    this.Parameters.Add(value);
                }
            }
        }

        int ICollection.Count
        {
            get
            {
                return this.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = value as DbAccessParameter;
            }
        }

        object IDataParameterCollection.this[string parameterName]
        {
            get
            {
                return this[parameterName];
            }
            set
            {
                this[parameterName] = value as DbAccessParameter;
            }
        }
    }
}

