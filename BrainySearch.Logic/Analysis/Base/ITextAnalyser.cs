using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Analysis.Base
{
    public interface ITextAnalyser
    {
        /// <summary>
        /// List of stop words for current analysis
        /// </summary>
        List<string> StopWords { get; set; }

        /// <summary>
        /// Returns percetage of text relevance
        /// </summary>
        decimal Analyse(string text);
    }
}
