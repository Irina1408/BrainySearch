using BrainySearch.Logic.Search.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.BrainySearchS
{
    public interface IBrainySearchService : ISearchService
    {
        SearchResults Search(string searchString, string[] keyWords);
    }
}
