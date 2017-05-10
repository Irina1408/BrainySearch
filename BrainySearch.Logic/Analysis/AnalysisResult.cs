using BrainySearch.Logic.Search.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Analysis
{
    public class AnalysisResult
    {
        public AnalysisResult()
        {
            IsSuitable = true;
        }

        /// <summary>
        /// Processed search result
        /// </summary>
        public ISearchResult SearchResult { get; set; }

        /// <summary>
        /// The percentage of natural language of the search result
        /// </summary>
        public decimal NaturalLanguagePercentage { get; set; }

        /// <summary>
        /// The tex score as the result of text ranking
        /// </summary>
        public decimal TextScore { get; set; }

        /// <summary>
        /// True if text passed all the tests with success
        /// </summary>
        public bool IsSuitable { get; set; }

        /// <summary>
        /// Index in the sequence
        /// </summary>
        public int Index { get; set; }
    }
}
