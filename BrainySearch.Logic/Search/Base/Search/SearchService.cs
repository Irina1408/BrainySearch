using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Base
{
    /// <summary>
    /// Search service with base implementation
    /// </summary>
    public abstract class SearchService<TParameters, TResult> : ISearchService<TParameters, TResult>
        where TParameters : ISearchParameters
        where TResult : ISearchResult
    {
        public SearchService()
        {
        }

        public SearchService(TParameters searchParameters)
        {
            SearchParameters = searchParameters;
        }

        /// <summary>
        /// Search parameters
        /// </summary>
        public TParameters SearchParameters { get; protected set; }

        /// <summary>
        /// Process search
        /// </summary>
        public abstract SearchResults<TResult> Search(string searchString);
    }

    /// <summary>
    /// Base implementstion of parametrized SearchService<TParameters, TResult>
    /// </summary>
    public abstract class SearchService : SearchService<ISearchParameters, ISearchResult>
    {
        public SearchService()
        {
            SearchParameters = new SearchParameters();
        }

        public SearchService(ISearchParameters searchParameters)
        {
            SearchParameters = searchParameters;
        }
    }
}
