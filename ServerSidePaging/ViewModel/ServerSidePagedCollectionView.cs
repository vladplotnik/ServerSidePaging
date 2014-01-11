namespace ServerSidePaging.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// A paged collection view that uses a IPagedDataSource instance to asynchronously
    /// retrieve paged data from a server
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServerSidePagedCollectionView<T> : ObservableCollection<T>, IPagedCollectionView
    {
        #region fields

        private IPagedDataSource<T> _pagedDataSource;

        private int _pageSize;

        private bool _canChangedPage = true;

        private bool _isPageChanging;

        private int _itemCount;

        private int _totalItemCount;

        private int _pageIndex = 0;

        #endregion

        #region constructor

        public ServerSidePagedCollectionView(IPagedDataSource<T> pagedDataSource)
        {
            this.PageSize = 20;
            this._pagedDataSource = pagedDataSource;
            this.RefreshData(0);
        }

        #endregion

        #region public properties

        public bool CanChangePage
        {
            private set
            {
                this.SetField(ref this._canChangedPage, value, "CanChangePage");
            }
            get
            {
                return this._canChangedPage;
            }
        }

        public bool IsPageChanging
        {
            private set
            {
                this.SetField(ref this._isPageChanging, value, "IsPageChanging");
            }
            get
            {
                return this._isPageChanging;
            }
        }

        public int ItemCount
        {
            private set
            {
                this.SetField(ref this._itemCount, value, "ItemCount");
            }
            get
            {
                return this._itemCount;
            }
        }

        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((double)this.TotalItemCount / (double)this.PageSize);
            }
        }

        public int PageIndex
        {
            private set
            {
                this.SetField(ref this._pageIndex, value, "PageIndex");
            }
            get
            {
                return this._pageIndex;
            }
        }

        public int PageSize
        {
            set
            {
                this.SetField(ref this._pageSize, value, "PageSize");
            }
            get
            {
                return this._pageSize;
            }
        }

        public int TotalItemCount
        {
            private set
            {
                this.SetField(ref this._totalItemCount, value, "TotalItemCount");
            }
            get
            {
                return this._totalItemCount;
            }
        }

        #endregion

        #region public methods

        public bool MoveToFirstPage()
        {
            this.RefreshData(0);
            return true;
        }

        public bool MoveToLastPage()
        {
            this.RefreshData(this.TotalPages - 1);
            return true;
        }

        public bool MoveToNextPage()
        {
            this.RefreshData(this.PageIndex + 1);
            return true;
        }

        public bool MoveToPage(int pageIndex)
        {
            this.RefreshData(pageIndex);
            return true;
        }

        public bool MoveToPreviousPage()
        {
            this.RefreshData(this.PageIndex - 1);
            return true;
        }

        #endregion

        #region events

        public event EventHandler<EventArgs> PageChanged;

        public event EventHandler<PageChangingEventArgs> PageChanging;

        protected void OnPageChanged()
        {
            if (this.PageChanged != null)
            {
                this.PageChanged(this, EventArgs.Empty);
            }
        }

        protected void OnPageChanging(int newPageIndex)
        {
            if (this.PageChanging != null)
            {
                this.PageChanging(this, new PageChangingEventArgs(newPageIndex));
            }
        }

        #endregion

        #region private implementation

        /// <summary>
        /// Fetches the data for the given page
        /// </summary>
        private void RefreshData(int newPageIndex)
        {
            // set the pre-fetch state
            this.CanChangePage = false;
            this.OnPageChanging(newPageIndex);

            this._pagedDataSource.FetchData(
                newPageIndex,
                response =>
                {
                    // process the received data
                    this.DataReceived(response);

                    // set the post-fetch state
                    this.PageIndex = newPageIndex;
                    this.OnPageChanged();
                    this.CanChangePage = response.TotalItemCount > 0;
                });
        }

        /// <summary>
        /// Updates the items exposed by this view with the given data
        /// </summary>
        private void DataReceived(PagedDataResponse<T> response)
        {
            this.TotalItemCount = this.ItemCount = response.TotalItemCount;
            this.ClearItems();

            foreach (var item in response.Items) this.Add(item);
        }

        /// <summary>
        /// Sets the given field, raising a PropertyChanged event
        /// </summary>
        private void SetField<T>(ref T field, T value, string propertyName)
        {
            if (object.Equals(field, value)) return;

            field = value;
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
