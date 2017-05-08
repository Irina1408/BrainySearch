using BrainySearch.Logic.Analysis;
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
        private List<AnalysisResult> analysisResult;
        private ZipfLawAnalyser zipfLawAnalyser;

        public BrainySearchAnalyser()
        {
            analysisResult = new List<AnalysisResult>();
            zipfLawAnalyser = new ZipfLawAnalyser();
            MinNaturalLanguagePercentage = (decimal)0.5; // 50%
        }

        /// <summary>
        /// The min suitable percentage of natural language of the search results
        /// </summary>
        public decimal MinNaturalLanguagePercentage { get; set; }

        public List<AnalysisResult> Analyse(List<ISearchResult> searchResults)
        {
            // cleanup before processing new results
            analysisResult.Clear();

            // 1. Analyse results by Zipf law
            ZipfLawAnalyse(searchResults);

            return analysisResult;
        }

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
                    analysisResult.Add(new AnalysisResult()
                    {
                        SearchResult = sr,
                        NaturalLanguagePercentage = naturalLanguagePercentage
                    });
                }
            }
            
            // update results sequence
            int index = 1;
            foreach (var r in analysisResult.OrderBy(item => item.NaturalLanguagePercentage))
            {
                r.Index = index;
                index += 1;
            }
        }
    }
}
