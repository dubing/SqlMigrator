using SqlMigratorWinform.Utility;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace SqlMigratorWinform.DataAccess
{
    [Serializable]
    public class DbDataPager
    {
        private int _GroupCount = 0;
        private int _GroupIndex = -1;
        private int _GroupSize = 10;
        private bool _HasSetRecordCount = false;
        private int _PageCount = 0;
        private int _PageIndex = -1;
        private int _PageSize = 10;
        private int _RecordCount = 0;
        private NameValueCollection _SearchParameters = new NameValueCollection();
        private string _SortFieldName = string.Empty;
        private bool _SortIsAsc = true;

        public event EventHandler ExecuteDataSource;

        public void FromString(string str)
        {
            ArrayList list = SerializeHelper.StringDeserialize(str) as ArrayList;
            this._RecordCount = (int) list[0];
            this._PageSize = (int) list[1];
            this._PageIndex = (int) list[2];
            this._GroupSize = (int) list[3];
            this._GroupIndex = (int) list[4];
            this._HasSetRecordCount = (bool) list[5];
            this._SearchParameters = (NameValueCollection) list[6];
            this._SortFieldName = (string) list[0];
            this._SortIsAsc = (bool) list[0];
            if (this._HasSetRecordCount)
            {
                this.SetRecordCount(this._RecordCount);
            }
        }

        public void GotoFirstGroup()
        {
            this.GotoIndexPage(0);
        }

        public void GotoFirstPage()
        {
            this._PageIndex = 0;
            this._GroupIndex = 0;
            this.OnExecuteDataSource(EventArgs.Empty);
        }

        public void GotoIndexPage(int pageIndex)
        {
            if (!this._HasSetRecordCount)
            {
                this.OnExecuteDataSource(EventArgs.Empty);
            }
            else if (pageIndex <= 0)
            {
                this.GotoFirstPage();
            }
            else if (pageIndex >= (this._PageCount - 1))
            {
                this.GotoLastPage();
            }
            else
            {
                this._PageIndex = pageIndex;
                if (this._GroupSize > 0)
                {
                    int num = this._PageIndex / this._GroupSize;
                    this._GroupIndex = ((this._PageIndex % this._GroupSize) == 0) ? num : (num + 1);
                }
                this.OnExecuteDataSource(EventArgs.Empty);
            }
        }

        public void GotoLastGroup()
        {
            this.GotoIndexPage(this._PageIndex + (((this._GroupCount - this._GroupIndex) - 1) * this._GroupSize));
        }

        public void GotoLastPage()
        {
            if (!this._HasSetRecordCount)
            {
                this.OnExecuteDataSource(EventArgs.Empty);
            }
            else
            {
                this._PageIndex = (this._PageCount > 0) ? (this._PageCount - 1) : 0;
                this._GroupIndex = (this._GroupCount > 0) ? (this._GroupCount - 1) : 0;
                this.OnExecuteDataSource(EventArgs.Empty);
            }
        }

        public void GotoNextGroup()
        {
            this.GotoIndexPage(this._PageIndex + this._GroupSize);
        }

        public void GotoNextPage()
        {
            if (!this._HasSetRecordCount)
            {
                this.OnExecuteDataSource(EventArgs.Empty);
            }
            else if (this._PageIndex >= (this._PageCount - 1))
            {
                this.GotoLastPage();
            }
            else
            {
                this._PageIndex++;
                int lastIndex = ((this._GroupIndex + 1) * this._GroupSize) - 1;
                if ((this._PageIndex > lastIndex) && (this._GroupIndex < (this._GroupCount - 2)))
                {
                    this._GroupIndex++;
                }
                this.OnExecuteDataSource(EventArgs.Empty);
            }
        }

        public void GotoPriviousGroup()
        {
            this.GotoIndexPage(this._PageIndex - this._GroupSize);
        }

        public void GotoPriviousPage()
        {
            if (!this._HasSetRecordCount)
            {
                this.OnExecuteDataSource(EventArgs.Empty);
            }
            else if (this._PageIndex <= 0)
            {
                this.GotoFirstPage();
            }
            else
            {
                this._PageIndex--;
                int firstIndex = this._GroupIndex * this._GroupSize;
                if ((this._PageIndex < firstIndex) && (this._GroupIndex > 0))
                {
                    this._GroupIndex--;
                }
                this.OnExecuteDataSource(EventArgs.Empty);
            }
        }

        public void GotoSearchPage(NameValueCollection searchParameters, bool isSearchResult)
        {
            if (!isSearchResult)
            {
                this._SearchParameters.Clear();
            }
            if (searchParameters != null)
            {
                this._SearchParameters.Add(searchParameters);
            }
            this._SortFieldName = string.Empty;
            this.GotoFirstPage();
        }

        public void GotoSearchPage(string name, string value, bool isSearchResult)
        {
            if (!isSearchResult)
            {
                this._SearchParameters.Clear();
            }
            if (!string.IsNullOrEmpty(name))
            {
                this._SearchParameters.Add(name, value);
            }
            this._SortFieldName = string.Empty;
            this.GotoFirstPage();
        }

        public void GotoSortPage(string sortFieldName)
        {
            if (string.Equals(sortFieldName, this._SortFieldName, StringComparison.InvariantCultureIgnoreCase))
            {
                this._SortIsAsc = !this._SortIsAsc;
            }
            else
            {
                this._SortIsAsc = true;
            }
            this._SortFieldName = sortFieldName;
            this.GotoFirstPage();
        }

        private void OnExecuteDataSource(EventArgs e)
        {
            if (this.ExecuteDataSource != null)
            {
                this.ExecuteDataSource(this, e);
            }
        }

        public void SetRecordCount(int recordCount)
        {
            if (recordCount >= 0)
            {
                int num;
                this._RecordCount = recordCount;
                if (this._PageSize > 0)
                {
                    num = this._RecordCount / this._PageSize;
                    this._PageCount = ((this._RecordCount % this._PageSize) == 0) ? num : (num + 1);
                }
                else
                {
                    this._PageCount = 0;
                }
                if (this._GroupSize > 0)
                {
                    num = this._PageCount / this._GroupSize;
                    this._GroupCount = ((this._PageCount % this._GroupSize) == 0) ? num : (num + 1);
                }
                else
                {
                    this._GroupCount = 0;
                }
                if (this._PageIndex < 0)
                {
                    this._PageIndex = 0;
                }
                if (this._GroupIndex < 0)
                {
                    this._GroupIndex = 0;
                }
                this._HasSetRecordCount = true;
            }
        }

        public override string ToString()
        {
            ArrayList list = new ArrayList();
            list.AddRange(new object[] { this._RecordCount, this._PageSize, this._PageIndex, this._GroupSize, this._GroupIndex, this._HasSetRecordCount, this._SearchParameters, this._SortFieldName, this._SortIsAsc });
            return SerializeHelper.StringSerialize(list);
        }

        public int GroupCount
        {
            get
            {
                return this._GroupCount;
            }
        }

        public int GroupIndex
        {
            get
            {
                return this._GroupIndex;
            }
        }

        public int GroupSize
        {
            get
            {
                return this._GroupSize;
            }
            set
            {
                this._GroupSize = (value > 0) ? value : 0;
            }
        }

        public bool HasSetRecordCount
        {
            get
            {
                return this._HasSetRecordCount;
            }
        }

        public bool IsFirstGroup
        {
            get
            {
                return (this._GroupIndex == 0);
            }
        }

        public bool IsFirstPage
        {
            get
            {
                return (this._PageIndex == 0);
            }
        }

        public bool IsLastGroup
        {
            get
            {
                return ((this._GroupIndex >= 0) && (this._GroupIndex == (this._GroupCount - 1)));
            }
        }

        public bool IsLastPage
        {
            get
            {
                return ((this._PageIndex >= 0) && (this._PageIndex == (this._PageCount - 1)));
            }
        }

        public int PageCount
        {
            get
            {
                return this._PageCount;
            }
        }

        public int PageIndex
        {
            get
            {
                return this._PageIndex;
            }
        }

        public int PageSize
        {
            get
            {
                return this._PageSize;
            }
            set
            {
                this._PageSize = (value > 0) ? value : 0;
            }
        }

        public int RecordCount
        {
            get
            {
                return this._RecordCount;
            }
        }

        public NameValueCollection SearchParameters
        {
            get
            {
                return this._SearchParameters;
            }
        }

        public string SortFieldName
        {
            get
            {
                return this._SortFieldName;
            }
            set
            {
                this._SortFieldName = value;
            }
        }

        public bool SortIsAsc
        {
            get
            {
                return this._SortIsAsc;
            }
            set
            {
                this._SortIsAsc = value;
            }
        }

        public int StartIndex
        {
            get
            {
                return (this._PageIndex * this._PageSize);
            }
        }
    }
}

