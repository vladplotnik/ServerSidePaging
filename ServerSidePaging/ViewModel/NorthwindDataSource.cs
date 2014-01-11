namespace ServerSidePaging.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Linq;
    using System.Threading.Tasks;

    using ServerSidePaging.Northwind;

    /// <summary>
    /// A paged NetFlix movies search datasource.
    /// </summary>
    public class NorthwindDataSource : IPagedDataSource<Order>
    {
        private string _searchString;

        private int _pageSize = 20;

        public NorthwindDataSource(string searchString)
        {
            this._searchString = searchString;
        }

        public void FetchData(int pageNumber, Action<PagedDataResponse<Order>> responseCallback)
        {
            var context = new NorthwindEntities(new Uri("http://localhost/WebApp/Northwind.svc/"));

            var query =
                context.Orders.AddQueryOption("$skip", pageNumber * this._pageSize)
                    .AddQueryOption("$top", this._pageSize)
                    .IncludeTotalCount();

            if (!string.IsNullOrEmpty(_searchString))
            {
                query = query.AddQueryOption("$filter", "(substringof('" + _searchString + "',CustomerID) eq true)");
            }

            Task.Factory.FromAsync<IEnumerable<Order>>(query.BeginExecute, query.EndExecute, null).ContinueWith(
                t =>
                {
                    var result = (QueryOperationResponse<Order>)t.Result;
                    responseCallback(
                        new PagedDataResponse<Order>
                        {
                            Items = result.ToList(),
                            TotalItemCount = (int)result.TotalCount
                        });
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
