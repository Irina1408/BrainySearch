using BrainySearch.Logic.Analysis.Base;
using BrainySearch.Logic.TextProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Analysis.Analysers
{
    /// <summary>
    /// Analyse text by Zipf law by count of words in the text.
    /// </summary>
    public class ZipfLawAnalyser : TextAnalyserBase
    {
        /// <summary>
        /// Calculates the percentage of natural language of the text (result is in decimals: 1 = 100%, 0.1 = 10%)
        /// </summary>
        public override decimal Analyse(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            
            // 1. Get all normalized words in the text and calculate its count in the text
            var wordCount = new Dictionary<string, int>();
            foreach (var word in wordNormalizer.GetNormalizedWords(text, false))
            {
                // exclude stop words
                if (StopWords.Contains(word) || word.Length <= 2) continue;

                if (!wordCount.ContainsKey(word))
                    wordCount.Add(word, 1);
                else
                    wordCount[word] += 1;
            }

            // 2. Get sorted descending list of word frequencies in the text
            var ranks = wordCount.OrderByDescending(item => item.Value).Select(item => item.Value).Distinct().ToList();

            // 3. Calculate words count with "correct" constant
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
    }
}
