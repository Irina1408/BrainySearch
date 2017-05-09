using BrainySearch.Logic.Analysis.Base;
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
    public class TextRankingAnalyser : TextAnalyserBase
    {
        public TextRankingAnalyser() : base()
        {
            KeyWords = new List<string>();
        }

        /// <summary>
        /// Key words for text analysis
        /// </summary>
        public List<string> KeyWords { get; private set; }

        /// <summary>
        /// Calculates the percentage of the text ranking (result is in decimals: 1 = 100%, 0.1 = 10%)
        /// </summary>
        public override decimal Analyse(string text)
        {
            // if no one key words result = 100%
            if (KeyWords.Count == 0) return 1;


            return 0;
        }
    }
}
