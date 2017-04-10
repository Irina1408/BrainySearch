using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Base
{
    public interface ISearchService
    {
        /// <summary>
        /// Language code in ISO 639-1
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Search info
        /// </summary>
        SearchResults Search(string searchString);
    }
}
