using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Base
{
    /// <summary>
    /// All search results
    /// </summary>
    public class SearchResults<TResult>
        where TResult : ISearchResult
    {
        public SearchResults()
        {
            Results = new List<TResult>();
        }

        /// <summary>
        /// Search results list
        /// </summary>
        public List<TResult> Results { get; }

        /// <summary>
        /// Error message if search was failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Returns true if search was failed
        /// </summary>
        public bool HasErrors { get { return !string.IsNullOrEmpty(ErrorMessage); } }
    }
}
