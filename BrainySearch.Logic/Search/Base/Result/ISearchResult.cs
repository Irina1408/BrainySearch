﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Base
{
    /// <summary>
    /// Serach result
    /// </summary>
    public interface ISearchResult
    {
        /// <summary>
        /// Result title
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Result text (optional)
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Result source link
        /// </summary>
        string Link { get; set; }
    }
}
