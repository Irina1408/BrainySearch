using BrainySearch.Logic.Search.Base;
using BrainySearch.Logic.TextProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Analysis
{
    public class SearchResultPreparer
    {
        #region Private fields
        
        private WordNormalizer wordNormalizer;

        #endregion

        #region Initialization

        public SearchResultPreparer()
        {
            wordNormalizer = new WordNormalizer();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Prepares search results to analyse
        /// </summary>
        public List<SearchResultToAnalyse> Prepare(List<ISearchResult> searchResults)
        {
            var res = new List<SearchResultToAnalyse>();

            foreach(var sr in searchResults)
            {
                // init
                var srToAnalyse = new SearchResultToAnalyse() { SearchResult = sr };
                // get all normalized words in the text
                srToAnalyse.NormalizedWords.AddRange(wordNormalizer.GetNormalizedWords(sr.Text, false));
                // calculate distinct normalized words count in the text
                foreach(var nw in srToAnalyse.NormalizedWords)
                {
                    if (!srToAnalyse.NormalizedWordCount.ContainsKey(nw))
                        srToAnalyse.NormalizedWordCount.Add(nw, 1);
                    else
                        srToAnalyse.NormalizedWordCount[nw] += 1;
                }

                // add prepared SearchResultToAnalyse to list
                res.Add(srToAnalyse);
            }

            return res;
        }

        #endregion
    }
}
