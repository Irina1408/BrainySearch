using BrainySearch.Logic.TextProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Analysis.Base
{
    public abstract class TextAnalyserBase : ITextAnalyser
    {
        #region Private fields

        private List<string> stopWords;
        protected WordNormalizer wordNormalizer;

        #endregion

        #region Initialization

        public TextAnalyserBase()
        {
            wordNormalizer = new WordNormalizer();
        }

        #endregion

        #region ITextAnalyser implementation

        public List<string> StopWords
        {
            get
            {
                // load stop words if it is not initialized
                if(stopWords == null)
                    stopWords = TextProcessingUtils.GetStopWords();

                return stopWords;
            }
            set
            {
                stopWords = value;
            }
        }

        /// <summary>
        /// Calculates the percentage of text suitability (result is in decimals: 1 = 100%, 0.1 = 10%)
        /// </summary>
        public abstract decimal Analyse(string text);

        #endregion
    }
}
