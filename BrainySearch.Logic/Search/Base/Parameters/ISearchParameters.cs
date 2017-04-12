using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Base
{
    /// <summary>
    /// Search parameters
    /// </summary>
    public interface ISearchParameters
    {
        /// <summary>
        /// Language code in ISO 639-1
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Max results count
        /// </summary>
        int Limit { get; set; }
    }
}
