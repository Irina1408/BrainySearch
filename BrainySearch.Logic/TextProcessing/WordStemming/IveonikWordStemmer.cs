using Iveonik.Stemmers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrainySearch.Logic.TextProcessing.WordStemming
{
    /// <summary>
    /// Stem words
    /// </summary>
    public class IveonikWordStemmer : IWordStemmer
    {
        #region Private fields
        
        private string pattern;
        private string language;

        #endregion

        #region Initialization

        public IveonikWordStemmer()
        {
            // init default stemmer (russian)
            Language = "ru";
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Language code
        /// </summary>
        public string Language
        {
            get { return language; }
            set
            {
                language = value;
                SetPattern();
            }
        }

        #endregion

        #region IWordStemmer implementation

        public string StemWord(string word)
        {
            var stemmer = GetStem();
            return stemmer.Stem(word);
        }

        public List<string> StemPhrase(string phrase, bool distinct)
        {
            // result
            var res = new List<string>();

            // parse phrase on the words
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = r.Match(phrase);

            while (m.Success)
            {
                var stemmedWord = StemWord(m.Value);
                if(!distinct || (distinct && !res.Contains(stemmedWord)))
                    res.Add(StemWord(m.Value));

                m = m.NextMatch();
            }

            return res;
        }

        #endregion

        #region Private methods

        private IStemmer GetStem()
        {
            switch (Language)
            {
                case "ru":
                    return new RussianStemmer();

                case "en":
                    return new EnglishStemmer();

                default:
                    return new EnglishStemmer();
            }
        }

        private void SetPattern()
        {
            switch (Language)
            {
                case "ru":
                    pattern = "([а-я]+)";
                    break;

                case "en":
                    pattern = "([a-z]+)";
                    break;

                default:
                    pattern = "([a-z]+)";
                    break;
            }
        }

        #endregion
    }
}
