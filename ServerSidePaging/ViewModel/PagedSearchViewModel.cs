namespace ServerSidePaging.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Windows.Input;

    using ServerSidePaging.Northwind;

    /// <summary>
    /// A view model that demonstrates paging data from the server
    /// </summary>
    public class PagedSearchViewModel : ViewModel
    {
        private string _search;
        private ServerSidePagedCollectionView<Order> _searchResults;

        public PagedSearchViewModel()
        {
            this.ObservePropertyChanged(() => Search)
                .Throttle(TimeSpan.FromSeconds(0.25))
                .ObserveOnDispatcher()
                .Subscribe(_ => this.SearchResults = new ServerSidePagedCollectionView<Order>(new NorthwindDataSource(this.Search)));

            Search = string.Empty;
        }

        public string Search
        {
            get
            {
                return _search;
            }
            set
            {
                _search = value;
                RaisePropertyChanged(() => Search);
            }
        }

        /// <summary>
        /// Gets the paged search results
        /// </summary>
        public ServerSidePagedCollectionView<Order> SearchResults
        {
            get
            {
                return _searchResults;
            }
            private set
            {
                _searchResults = value;
                RaisePropertyChanged(() => SearchResults);
            }
        }
    }

    public static class INotifyPropertyChangedExtensions
    {
        public static IObservable<Unit> ObservePropertyChanged<T>(this INotifyPropertyChanged source, Expression<Func<T>> propertyExpression)
        {
            return ObservePropertyChanged(source, propertyExpression.GetPropertyName());
        }

        public static IObservable<Unit> ObservePropertyChanged(this INotifyPropertyChanged source, string property)
        {
            return Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => (sender, e) => h(e),
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h)
                .Where(e => e.PropertyName == property)
                .Select(_ => Unit.Default);
        }
    }
}
