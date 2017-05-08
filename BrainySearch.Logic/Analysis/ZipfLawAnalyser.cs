using BrainySearch.Logic.TextProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Analysis
{
    /// <summary>
    /// Analyse text by Zipf law by count of words in the text.
    /// </summary>
    public class ZipfLawAnalyser
    {
        private WordNormalizer wordNormalizer;
        private List<string> stopWords;

        public ZipfLawAnalyser()
        {
            wordNormalizer = new WordNormalizer();
            // load stop words
            stopWords = GetStopWords();
        }

        /// <summary>
        /// Calculates the percentage of natural language of the text (result is in decimals: 1 = 100%, 0.1 = 10%)
        /// </summary>
        public decimal Analyse(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            
            // 1. Get all normalized words in the text and calculate its count in the text
            var wordCount = new Dictionary<string, int>();
            var r = wordNormalizer.GetNormalizedWords(text, false);
            var t = r.Where(item => item.Contains("документ")).ToList();
            foreach (var word in wordNormalizer.GetNormalizedWords(text, false))
            {
                // exclude stop words
                if (stopWords.Contains(word) || word.Length <= 2) continue;

                if (!wordCount.ContainsKey(word))
                    wordCount.Add(word, 1);
                else
                    wordCount[word] += 1;
            }

            // 2. Get sorted descending list of word frequencies in the text
            var ranks = wordCount.OrderByDescending(item => item.Value).Select(item => item.Value).Distinct().ToList();

            // 3. Calculate words count with "correct" constant
            //decimal c = -1;
            //int constWordCount = 1;
            //decimal totc = 0;
            //for(int i = 0; i < ranks.Count; i++)
            //{
            //    // constant for current word = rank * probability, where probability = frequency / the total number of words
            //    decimal currc = (i + 1) * ((decimal)ranks[i] / ranks.Count);
            //    // convert to integer
            //    decimal currcInt = currc > c ? Math.Floor(currc) : Math.Ceiling(currc);
            //    // calculate constant if it was not initialized
            //    if (c == -1)
            //        c = currc;
            //    else if (Math.Abs(currcInt - c) < 1)
            //        constWordCount += 1;

            //    totc += (currc / c) > 1 ? 1 - (currc / c) : currc / c;
            //}
            // initial words count (top in the list)
            int initWordCount = -1;
            decimal totPercentage = 1;
            for (int i = 0; i < ranks.Count; i++)
            {
                // recommended word count in text
                decimal recommendedWordCount = (decimal)initWordCount / (i + 1);
                
                if (initWordCount == -1)
                    initWordCount = ranks[i];
                // calculate the percentage of compliance
                else if (ranks[i] > recommendedWordCount)
                    totPercentage += recommendedWordCount / ranks[i];
                else
                    totPercentage += (decimal)ranks[i] / recommendedWordCount;
            }

            // 4. Return result as constWordCount / total ranks count
            return ranks.Count == 0 ? 0 : totPercentage / ranks.Count;
        }

        /// <summary>
        /// Gets list of stop words
        /// </summary>
        private List<string> GetStopWords()
        {
            // get executing assembly
            var assembly = Assembly.GetExecutingAssembly();
            // get file
            var sr = new StreamReader(assembly.GetManifestResourceStream("BrainySearch.Logic.Analysis.stop-words.txt"));
            // stop words
            var stopWords = new List<string>();
            // read all stop words
            var line = sr.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                stopWords.Add(line);
                line = sr.ReadLine();
            }
            // cleanup
            sr.Close();
            sr.Dispose();

            return stopWords;
        }
    }
}
