using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Base
{
    /// <summary>
    /// Base search parameters
    /// </summary>
    public class SearchParameters : ISearchParameters
    {
        public SearchParameters()
        {
            // set defaults
            Language = "en";
            Limit = 10;
        }

        /// <summary>
        /// Language code in ISO 639-1
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Max results count
        /// </summary>
        public int Limit { get; set; }

        
    }
}
