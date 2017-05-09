using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexMystem.Wrapper;

namespace BrainySearch.Logic.TextProcessing
{
    /// <summary>
    /// Normalize words form
    /// </summary>
    public class WordNormalizer
    {
        #region Private fields

        private Mysteam mysteam;

        #endregion

        #region Initialization

        public WordNormalizer()
        {
            mysteam = new Mysteam();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns normalized word
        /// </summary>
        public string NormalizeWord(string word)
        {
            // get normalization result
            var res = mysteam.GetWords(word);
            // check result
            if (res.Count == 0)
                return word;

            if (res[0].Lexems.Count > 0)
                return res[0].Lexems[0].Lexeme;

            return string.Empty;
        }

        /// <summary>
        /// Returns list of normalized words in text
        /// </summary>
        public List<string> GetNormalizedWords(string text, bool distinct)
        {
            var words = new List<string>();
            
            // get normalized words list
            foreach (var r in mysteam.GetWords(text.Replace("\n", " ").Replace("\r", " ")))
            {
                // if normalizer did no find any normalized word add source word
                if(r.Lexems.Count == 0)
                    words.Add(r.SourceWord.Text);
                else
                    words.Add(r.Lexems[0].Lexeme);
            }

            return distinct ? words.Distinct().ToList() : words;
        }

        #endregion
    }
}
