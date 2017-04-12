using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Base
{
    /// <summary>
    /// Search service with parametrized parameters and results
    /// </summary>
    /// <typeparam name="TParameters"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface ISearchService<TParameters, TResult>
        where TParameters : ISearchParameters
        where TResult : ISearchResult
    {
        /// <summary>
        /// Search parameters
        /// </summary>
        TParameters SearchParameters { get; }

        /// <summary>
        /// Search info by search string
        /// </summary>
        SearchResults<TResult> Search(string searchString);
    }

    /// <summary>
    /// Search service with standard search parameters and search results
    /// </summary>
    public interface ISearchService : ISearchService<ISearchParameters, ISearchResult>
    {

    }
}
