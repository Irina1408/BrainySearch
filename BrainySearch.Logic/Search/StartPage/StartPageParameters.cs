using BrainySearch.Logic.Search.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.StartPage
{
    /// <summary>
    /// StartPage web search parameters
    /// </summary>
    public class StartPageParameters : SearchParameters
    {
        public StartPageParameters() : base()
        {
            LoadText = true;
        }

        /// <summary>
        /// Returns true if result text should be loaded
        /// </summary>
        public bool LoadText { get; set; }
    }
}
