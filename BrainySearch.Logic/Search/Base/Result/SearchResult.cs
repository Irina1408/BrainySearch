using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Base
{
    /// <summary>
    /// Base search result
    /// </summary>
    public class SearchResult : ISearchResult
    {
        /// <summary>
        /// Result title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Result description (optional)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Result source link
        /// </summary>
        public string Link { get; set; }
    }
}
