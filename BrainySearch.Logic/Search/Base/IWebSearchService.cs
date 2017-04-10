using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Base
{
    public interface IWebSearchService : ISearchService
    {
        int MaxPagesCount { get; set; }
        string URL { get; }
    }
}
