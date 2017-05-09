using BrainySearch.Logic.TextProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Filters
{
    /// <summary>
    /// Filters text by key words existance in the text (only english key words)
    /// </summary>
    public class KeyWordsFilter
    {
        #region Private fields
        
        protected WordNormalizer wordNormalizer;
        private List<string> NormalizedKeyWords = new List<string>();

        #endregion

        #region Initialization

        public KeyWordsFilter()
        {
            KeyWords = new List<string>();
            NormalizedKeyWords = new List<string>();
            wordNormalizer = new WordNormalizer();
        }

        #endregion

        #region Public methods and properties

        /// <summary>
        /// Key words for text checking
        /// </summary>
        public List<string> KeyWords { get; private set; }

        /// <summary>
        /// Returns true if text contains all key words else false
        /// </summary>
        public bool IsSuitableText(string text)
        {
            // check any key word exists
            if (KeyWords.Count == 0) return true;
            // normalize key words
            if (NormalizedKeyWords.Count == 0) NormalizeKeyWords();
            // get all normalized words in text
            var normalizedWords = wordNormalizer.GetNormalizedWords(text, true);
            // text should contain all key words
            return NormalizedKeyWords.All(item => normalizedWords.Contains(item));
        }

        /// <summary>
        /// Returns count of contained key words in the text
        /// </summary>
        public int GetContainedKeyWordsCount(string text)
        {
            // check any key word exists
            if (KeyWords.Count == 0) return 0;
            // normalize key words
            if (NormalizedKeyWords.Count == 0) NormalizeKeyWords();
            // get all normalized words in text
            var normalizedWords = wordNormalizer.GetNormalizedWords(text, true);

            // text should contain all key words
            return NormalizedKeyWords.Where(item => normalizedWords.Contains(item)).Count();
        }

        public void NormalizeKeyWords()
        {
            // cleanup 
            NormalizedKeyWords.Clear();
            // fill normalized key words
            foreach (var kw in KeyWords)
                NormalizedKeyWords.AddRange(wordNormalizer.GetNormalizedWords(kw, true));
        }

        #endregion
    }
}
