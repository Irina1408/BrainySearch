using BrainySearch.Logic.Analysis;
using BrainySearch.Logic.Analysis.Analysers;
using BrainySearch.Logic.Analysis.Base;
using BrainySearch.Logic.Search.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Analysis
{
    /// <summary>
    /// Applies all necessary analysers
    /// </summary>
    public class BrainySearchAnalyser
    {
        #region Private fields

        private List<AnalysisResult> analysisResults;
        private ITextAnalyser zipfLawAnalyser;

        #endregion

        #region Initialization

        public BrainySearchAnalyser()
        {
            analysisResults = new List<AnalysisResult>();
            zipfLawAnalyser = new ZipfLawAnalyser();
            MinNaturalLanguagePercentage = (decimal)0.5; // 50%
            KeyWords = new List<string>();

            // init stop words
            zipfLawAnalyser.StopWords = TextProcessing.TextProcessingUtils.GetStopWords();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The min suitable percentage of natural language of the search results
        /// </summary>
        public decimal MinNaturalLanguagePercentage { get; set; }

        /// <summary>
        /// Key words for text analysis
        /// </summary>
        public List<string> KeyWords { get; private set; }

        #endregion

        #region Public methods

        public List<AnalysisResult> Analyse(List<ISearchResult> searchResults)
        {
            // cleanup before processing new results
            analysisResults.Clear();

            // 1. Analyse results by Zipf law
            ZipfLawAnalyse(searchResults);

            return analysisResults;
        }

        #endregion

        #region Private methods

        private void ZipfLawAnalyse(List<ISearchResult> searchResults)
        {
            // calculate the percentages of natural language of the search results
            foreach (var sr in searchResults)
            {
                // natural language percentage
                var naturalLanguagePercentage = zipfLawAnalyser.Analyse(sr.Text);
                // add in the result only if it is not less then MinNaturalLanguagePercentage
                if(naturalLanguagePercentage >= MinNaturalLanguagePercentage)
                {
                    analysisResults.Add(new AnalysisResult()
                    {
                        SearchResult = sr,
                        NaturalLanguagePercentage = naturalLanguagePercentage
                    });
                }
            }
            
            // update results sequence
            int index = 1;
            foreach (var r in analysisResults.OrderBy(item => item.NaturalLanguagePercentage))
            {
                r.Index = index;
                index += 1;
            }
        }

        #endregion
    }
}
