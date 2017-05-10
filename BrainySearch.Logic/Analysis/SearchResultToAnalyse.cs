using BrainySearch.Logic.Search.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Analysis
{
    /// <summary>
    /// Prepared search result to analyse
    /// </summary>
    public class SearchResultToAnalyse
    {
        public SearchResultToAnalyse()
        {
            NormalizedWords = new List<string>();
            NormalizedWordCount = new Dictionary<string, int>();
        }

        /// <summary>
        /// Search result to process
        /// </summary>
        public ISearchResult SearchResult { get; set; }

        /// <summary>
        /// List of all normalized words in the text
        /// </summary>
        public List<string> NormalizedWords { get; private set; }

        /// <summary>
        /// All distinct normalized words in the text and its count in the text
        /// </summary>
        public Dictionary<string, int> NormalizedWordCount { get; private set; }
    }
}
