using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Parser
{
    using CsQuery;

    public class WikipediaParser : ParserBase
    {
        /// <summary>
        /// Returns part of page with definition (part before Contents)
        /// </summary>
        /// <param name="html">Full html of wikipedia page</param>
        /// <returns></returns>
        public string GetDefinitionHtml(string html)
        {
            var contentTextDom = GetDomObjectByTagId(html, "mw-content-text");
            var stringBuilder = new StringBuilder();

            foreach(var tag in contentTextDom.ChildNodes)
            {
                if (tag.Id == "toc") break;

                if (tag.NodeName.ToLower() == "p")
                {
                    stringBuilder.AppendLine(tag.Render());
                }
            }

            return stringBuilder.ToString();
        }
    }
}
