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

        private List<string> keyWords;

        #endregion

        #region Initialization

        public KeyWordsFilter()
        {
            this.keyWords = new List<string>();
        }

        #endregion

        #region Public methods and properties

        public List<string> KeyWords
        {
            get { return keyWords; }
            set { FillKeyWords(value); }
        }

        /// <summary>
        /// Returns true if text contains all key words else false
        /// </summary>
        public bool IsSuitableText(string text)
        {
            // check any key word exists
            if (keyWords.Count == 0) return true;
            // text should contain all key words
            return keyWords.All(item => text.Contains(item));
        }

        #endregion

        #region Private methods

        private void FillKeyWords(List<string> keyWords)
        {
            // clear existing key words
            this.keyWords.Clear();
            // return is any key word does not exist
            if (keyWords == null) return;

            // filter key words by language (only english key words)
            foreach(var keyWord in keyWords)
            {
                if(LangDetection.LangDetector.Detect(keyWord) == "en")
                    this.keyWords.Add(keyWord);
            }
        }

        #endregion
    }
}
