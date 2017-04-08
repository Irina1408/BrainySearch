using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Base
{
    public interface ISearchResult
    {
        string Title { get; set; }

        string Description { get; set; }

        string Link { get; set; }
    }
}
