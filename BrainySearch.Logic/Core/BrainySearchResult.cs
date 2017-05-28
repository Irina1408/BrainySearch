using BrainySearch.Logic.Search.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Core
{
    public class BrainySearchResult : SearchResult
    {
        /// <summary>
        /// Index in the sequence
        /// </summary>
        public int Index { get; set; }
    }
}
