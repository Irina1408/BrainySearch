using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexMystem.Wrapper;

namespace BrainySearch.Logic.TextProcessing
{
    public class WordNormalizer
    {
        private Mysteam mysteam;

        public WordNormalizer()
        {
            mysteam = new Mysteam();
            OnlySingleResults = false;
        }

        /// <summary>
        /// Gets true if word should no be normilized in case when normalizator found more then one normalized word
        /// </summary>
        public bool OnlySingleResults { get; set; }

        /// <summary>
        /// Returns normalized word
        /// </summary>
        public string NormalizeWord(string word)
        {
            // get normalization result
            var res = mysteam.GetWords(word);
            // check result
            if (res.Count == 0)
                return string.Empty;

            // if normalizator found more then one normalized word and OnlySingleResults return the same word
            if (OnlySingleResults && res[0].Lexems.Count > 1)
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
                // if normalizator found more then one normalized word and OnlySingleResults add the same word
                if (r.Lexems.Count > 1 && OnlySingleResults)
                    words.Add(r.SourceWord.Text);
                else if(r.Lexems.Count == 0)
                    words.Add(r.SourceWord.Text);
                else
                    words.Add(r.Lexems[0].Lexeme);
            }

            return distinct ? words.Distinct().ToList() : words;
        }
    }
}
