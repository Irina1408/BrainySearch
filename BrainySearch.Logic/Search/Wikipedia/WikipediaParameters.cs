using BrainySearch.Logic.Search.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Wikipedia
{
    /// <summary>
    /// Description type of result description
    /// </summary>
    public enum DescriptionType
    {
        // First sentence
        Short,
        Full,
        WithTags
    }

    /// <summary>
    /// Customized for wikipedia search parameters
    /// </summary>
    public class WikipediaParameters : SearchParameters
    {
        public WikipediaParameters() : base()
        {
            DescriptionType = DescriptionType.Short;
        }

        /// <summary>
        /// Result search description type
        /// </summary>
        public DescriptionType DescriptionType { get; set; }
    }
}
