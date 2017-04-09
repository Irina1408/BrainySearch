using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Base
{
    public interface ISearchService
    {
        SearchResults Search(string searchString);
    }
}
