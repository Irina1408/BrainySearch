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
        /// <summary>
        /// Processed search result
        /// </summary>
        public ISearchResult SearchResult { get; set; }

        /// <summary>
        /// The percentage of natural language of the search result
        /// </summary>
        public decimal NaturalLanguagePercentage { get; set; }

        /// <summary>
        /// Index in the sequence
        /// </summary>
        public int Index { get; set; }
    }
}
