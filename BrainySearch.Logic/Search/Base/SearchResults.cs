using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Base
{
    public class SearchResults
    {
        public SearchResults()
        {
            Results = new List<ISearchResult>();
        }

        public List<ISearchResult> Results { get; }

        public string ErrorMessage { get; set; }

        public bool HasErrors { get { return !string.IsNullOrEmpty(ErrorMessage); } }
    }
}
