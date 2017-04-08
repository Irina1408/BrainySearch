using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Base
{
    public class SearchResult : ISearchResult
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }
    }
}
