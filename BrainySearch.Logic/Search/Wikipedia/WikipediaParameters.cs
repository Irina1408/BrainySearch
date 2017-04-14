using BrainySearch.Logic.Search.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.Wikipedia
{
    /// <summary>
    /// Text type of result text
    /// </summary>
    public enum TextType
    {
        // First sentence
        ShortDescription,
        FullDescription,
        HtmlPagePart
    }

    /// <summary>
    /// Customized for wikipedia search parameters
    /// </summary>
    public class WikipediaParameters : SearchParameters
    {
        public WikipediaParameters() : base()
        {
            TextType = TextType.ShortDescription;
        }

        /// <summary>
        /// Result search text type
        /// </summary>
        public TextType TextType { get; set; }
    }
}
