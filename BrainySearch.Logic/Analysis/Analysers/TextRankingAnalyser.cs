using BrainySearch.Logic.TextProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Analysis.Analysers
{
    /// <summary>
    /// Rank text by key words including
    /// </summary>
    public class TextRankingAnalyser
    {
        #region Private fields
        
        private WordNormalizer wordNormalizer;
        private Dictionary<string, List<string>> normalizedKeyWords = new Dictionary<string, List<string>>();

        #endregion

        #region Initialization

        public TextRankingAnalyser()
        {
            normalizedKeyWords = new Dictionary<string, List<string>>();
            wordNormalizer = new WordNormalizer();
        }

        #endregion

        #region Public properties and methods

        /// <summary>
        /// Key words for text analysis
        /// </summary>
        public List<string> KeyWords { get; set; }

        /// <summary>
        /// All search results to analyse
        /// </summary>
        public List<SearchResultToAnalyse> SearchResultsToAnalyse { get; set; }

        /// <summary>
        /// Calculates the score of the text
        /// </summary>
        public decimal Analyse(SearchResultToAnalyse searchResultToAnalyse)
        {
            // if no one key words and all search results list -> result = 0
            if (KeyWords == null || KeyWords.Count == 0 || SearchResultsToAnalyse == null || SearchResultsToAnalyse.Count == 0) return 0;
            // normalize key words
            if (normalizedKeyWords.Count == 0) NormalizeKeyWords();

            double Score = 0;

            // for every key word
            foreach (var keyWord in normalizedKeyWords)
            {
                // local variables
                double TF = 0;

                // ------------ W single ------------
                double WSingle = 0;
                // sum(log(pi)), i - key word index
                double sumLogPi = 0;
                foreach (var kw in keyWord.Value)
                {
                    // 1. TF is the number of occurrences of the word in the text 
                    TF = searchResultToAnalyse.NormalizedWordCount.ContainsKey(kw) ? searchResultToAnalyse.NormalizedWordCount[kw] : 0;
                    // 2. CF is the number of occurrences of the word in the documents
                    double CF = SearchResultsToAnalyse.Sum(
                        item => item.NormalizedWordCount.ContainsKey(kw) ? item.NormalizedWordCount[kw] : 0);
                    // 3. p = 1 - exp(-1.5 * CF / D), where D - the number of documents in the collection
                    double p = (1 - Math.Exp(-1.5 * CF / SearchResultsToAnalyse.Count)) * 100;
                    // 4. TF1 = TF / (TF + k1 + k2 * DocLength), where k1 = 1, k2 = 1/350, DocLength - total count words in the text
                    double TF1 = TF / (TF + 1 + 1.0 / 350 * searchResultToAnalyse.NormalizedWords.Count);
                    // 5. WSingle = log(p) * (TF1 + 0.2 *TF2), where TF2 = 0
                    WSingle += Math.Log(p) * TF1;
                    // update sum(log(pi))
                    sumLogPi += Math.Log(p);
                }

                // ------------ W pair ------------
                double WPair = 0;
                // Calculate Wpair only for complex key words
                if (keyWord.Value.Count > 1)
                {
                    // loop on pair of words (first and second, second and third etc.)
                    for (int iPair = 0; iPair < keyWord.Value.Count - 1; iPair++)
                    {
                        // 1. TF is the number of occurrences of the word pair in the text 
                        TF = CalculateWordPairOccurrences(keyWord.Value[iPair], keyWord.Value[iPair + 1], searchResultToAnalyse.NormalizedWords);
                        // 2. CF1 and CF2 are the number of occurrences of the word in the documents
                        double CF1 = SearchResultsToAnalyse.Sum(item => 
                            item.NormalizedWordCount.ContainsKey(keyWord.Value[iPair]) ? item.NormalizedWordCount[keyWord.Value[iPair]] : 0);
                        double CF2 = SearchResultsToAnalyse.Sum(
                            item => item.NormalizedWordCount.ContainsKey(keyWord.Value[iPair + 1]) ? item.NormalizedWordCount[keyWord.Value[iPair + 1]] : 0);
                        // 3. p1 and p2 like p (p = 1 - exp(-1.5 * CF / D)), where D - the number of documents in the collection
                        double p1 = 1 - Math.Exp(-1.5 * CF1 / SearchResultsToAnalyse.Count);
                        double p2 = 1 - Math.Exp(-1.5 * CF2 / SearchResultsToAnalyse.Count);
                        // 4. WPair = 0.3 * (log(p1) + log(p2)) * TF / (1 + TF)
                        WPair += 0.3 * (Math.Log(p1) + Math.Log(p2)) * TF / (1 + TF);
                    }
                }

                // ------------ W all words ------------
                // 1. Nmiss - the number of words which are not found in the document
                int Nmiss = keyWord.Value.Where(item => !searchResultToAnalyse.NormalizedWords.Contains(item)).Count();
                // 2. WAllWords = 0.2 * sum(log(pi)) * 0.03^Nmiss
                double WAllWords = 0.2 * sumLogPi * Math.Pow(0.03, Nmiss);

                // ------------ W phrase ------------
                // 1. TF is the number of occurrences of the full phrase in the text
                TF = -1;
                int index = 0;
                while (index >= 0 && index < searchResultToAnalyse.SearchResult.Text.Length - 1)
                {
                    index = searchResultToAnalyse.SearchResult.Text.IndexOf(keyWord.Key, index);
                    if (index >= 0) index += 1;
                    TF += 1;
                }
                // 2. WPhrase = 0.1 * sum(log(pi)) * TF / (1 + TF)
                double WPhrase = 0.1 * sumLogPi * TF / (1 + TF);

                // ------------ Score ------------
                Score += Math.Abs(WSingle + WPair + WAllWords + WPhrase);
            }

            return Convert.ToDecimal(Score);
        }

        public void NormalizeKeyWords()
        {
            // cleanup 
            normalizedKeyWords.Clear();
            // fill normalized key words
            foreach (var keyWord in KeyWords)
                normalizedKeyWords.Add(keyWord.ToLower(), wordNormalizer.GetNormalizedWords(keyWord, true));
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Calculates count of words pair (word1 and word2) occurrences in words
        /// </summary>
        private double CalculateWordPairOccurrences(string word1, string word2, List<string> words)
        {
            double wordPairOccurrences = 0;

            // loop on normalized words in the text
            for (int iWord = 0; iWord < words.Count - 1; iWord++)
            {
                // if current and next words are the same as key words pair
                if (words[iWord] == word1 && words[iWord + 1] == word2)
                    wordPairOccurrences += 1;
                // if current and next words are the same as key words pair in reverse order
                else if (words[iWord] == word2 && words[iWord + 1] == word1)
                    wordPairOccurrences += 0.5;
                // if the third words after current exists and current and next words are the same as key words pair through one word
                else if (iWord + 2 < words.Count && words[iWord] == word1 && words[iWord + 2] == word2)
                    wordPairOccurrences += 0.5;
            }

            return wordPairOccurrences;
        }

        #endregion
    }
}
