using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Base
{
    public interface ISearchService
    {
        string URL { get; }
        int MaxPagesCount { get; set; }
        string Language { get; set; }
        string LanguageCode { get; set; }
        SearchResults Search(string searchString);
    }
}
