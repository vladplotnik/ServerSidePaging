namespace ServerSidePaging.ViewModel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a source of data that can be paged.
    /// </summary>
    public interface IPagedDataSource<TDataType>
    {
        /// <summary>
        /// Asynchronously returns the data for the given page
        /// </summary>
        void FetchData(int pageNumber, Action<PagedDataResponse<TDataType>> responseCallback);
    }

    /// <summary>
    /// The items returned as a result of a paged data request.
    /// </summary>
    public class PagedDataResponse<TDataType>
    {
        /// <summary>
        /// The items contained within the requested page
        /// </summary>
        public List<TDataType> Items { get; set; }

        /// <summary>
        /// The total count of all available items
        /// </summary>
        public int TotalItemCount { get; set; }
    }
}
