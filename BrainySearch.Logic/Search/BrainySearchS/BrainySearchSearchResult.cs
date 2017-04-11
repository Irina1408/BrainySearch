using BrainySearch.Logic.Search.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.BrainySearchS
{
    public class BrainySearchSearchResult : SearchResult
    {
        public BrainySearchSearchResult() : base()
        {
            ParsePage = true;
        }

        public BrainySearchSearchResult(ISearchResult searchResult) : this()
        {
            Title = searchResult.Title;
            Description = searchResult.Description;
            Link = searchResult.Link;
        }

        public bool ParsePage { get; set; }
    }
}
