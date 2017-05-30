using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.TextProcessing.WordStemming
{
    public interface IWordStemmer
    {
        string StemWord(string word);

        List<string> StemPhrase(string phrase, bool distinct);
    }
}
