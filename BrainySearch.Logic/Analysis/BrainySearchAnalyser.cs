using BrainySearch.Logic.Analysis;
using BrainySearch.Logic.Analysis.Analysers;
using BrainySearch.Logic.Search.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private SearchResultPreparer searchResultPreparer;
        private ZipfLawAnalyser zipfLawAnalyser;
        private TextRankingAnalyser textRankingAnalyser;

        #endregion

        #region Initialization

        public BrainySearchAnalyser()
        {
            analysisResults = new List<AnalysisResult>();
            searchResultPreparer = new SearchResultPreparer();
            zipfLawAnalyser = new ZipfLawAnalyser();
            textRankingAnalyser = new TextRankingAnalyser();
            MinNaturalLanguagePercentage = (decimal)0.5; // 50%
            KeyWords = new List<string>();
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
            // prepare search results to analyse
            var preparedSearchResults = searchResultPreparer.Prepare(searchResults);
            // analyse results by all methods
            Analyse(preparedSearchResults);
            // update results sequence
            UpdateResultsSequence();

            return analysisResults;
        }

        #endregion

        #region Private methods

        private void Analyse(List<SearchResultToAnalyse> searchResults)
        {
            // set parameters
            textRankingAnalyser.KeyWords = KeyWords;
            textRankingAnalyser.SearchResultsToAnalyse = searchResults;
            textRankingAnalyser.NormalizeKeyWords();
            // tasks for analysis search results
            var tasks = new List<Task>();

            // calculate the percentages of natural language of the search results
            foreach (var sr in searchResults)
            {
                var tmp = sr;
                tasks.Add(Task.Run(() =>
                {
                    //// calculate natural language percentage by Zipf law
                    //var naturalLanguagePercentageTask = zipfLawAnalyser.AnalyseTask(tmp);
                    //naturalLanguagePercentageTask.Start();
                    //// calculate text score by text ranking
                    //var textScoreTask = textRankingAnalyser.AnalyseTask(tmp);
                    //textScoreTask.Start();
                    //// wait calculation is ended
                    //Task.WaitAll(naturalLanguagePercentageTask, textScoreTask);
                    //// get result
                    //var naturalLanguagePercentage = naturalLanguagePercentageTask.Result;
                    //var textScore = textScoreTask.Result;

                    // calculate natural language percentage by Zipf law
                    var naturalLanguagePercentage = zipfLawAnalyser.Analyse(tmp);
                    // calculate text score by text ranking
                    var textScore = textRankingAnalyser.Analyse(tmp);
                    // add new result
                    var newResult = new AnalysisResult()
                    {
                        SearchResult = tmp.SearchResult,
                        NaturalLanguagePercentage = naturalLanguagePercentage,
                        TextScore = textScore,
                        IsSuitable = naturalLanguagePercentage >= MinNaturalLanguagePercentage
                    };
                    lock(analysisResults) analysisResults.Add(newResult);
                    //// cleanup
                    //naturalLanguagePercentageTask.Dispose();
                    //textScoreTask.Dispose();
                }));
            }

            // wait while all tasks are ended
            Task.WaitAll(tasks.ToArray(), CancellationToken.None);
            // dispose all tasks
            foreach (var t in tasks)
                t.Dispose();
        }

        /// <summary>
        /// Updates results sequence
        /// </summary>
        private void UpdateResultsSequence()
        {
            // get max text score for using as 100%
            decimal maxTextScore = analysisResults.Max(item => item.TextScore);
            if (maxTextScore == 0)
                maxTextScore = 1;

            int index = 1;
            // sort in the arithmetic mean of NaturalLanguagePercentage and TextScore percentage
            foreach (var r in analysisResults.OrderBy(item => (item.NaturalLanguagePercentage + (item.TextScore / maxTextScore)) / 2))
            {
                r.Index = index;
                index += 1;
            }
        }

        #endregion
    }
}
