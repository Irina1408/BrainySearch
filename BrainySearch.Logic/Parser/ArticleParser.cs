using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Parser
{
    using BrainySearch.Logic.Parser.Base;
    using CsQuery;

    public class ArticleParser : TextParser
    {
        #region IParser implementation

        /// <summary>
        /// Parse html and returns html string
        /// </summary>
        public override string Parse(string html)
        {
            // clean html
            html = RemoveEmptySpaces(html);
            html = RemoveWhiteSpaces(html);

            // cq main html page document
            var document = CQ.Create(html).Document;
            if (document == null) return null;
            // get html body document
            var bodyDom = document.Body;
            if (bodyDom == null) return null;
            // search by id or class name "content"
            var res = SearchChildren(bodyDom, "content", "content");
            // search by id or class name "article"
            if (res == null || !res.Any())
                res = SearchChildren(bodyDom, "article", "article");
            // search by tag name "article"
            if (res == null || !res.Any())
                res = SearchChildren(bodyDom, false, "article");

            // result string builder
            var sb = new StringBuilder();

            if (res != null && res.Any())
            {
                // loop on every result with children
                foreach(var r in res.Where(item => item.HasChildren))
                {                    
                    // find all tags with children tags "p"
                    foreach(var pTag in SearchChildren(r, false, "p"))
                    {
                        // write all children
                        //foreach (var ch in pTag.ChildNodes)
                        //{
                        //    if (ch.InnerHtmlAllowed)
                        //        sb.AppendLine(ch.Render());
                        //}
                        if (pTag.InnerHtmlAllowed)
                            sb.AppendLine(pTag.Render());
                    }
                }
            }
            else
            {
                // get all tags with text
                foreach (var ch in SearchChildren(bodyDom, false, "p"))
                {
                    if (ch.InnerHtmlAllowed)
                        sb.AppendLine(ch.Render());
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Private functions

        protected string RemoveEmptySpaces(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Replace("\t", null).Replace("\n", null).Replace("\r", null); ;
        }

        protected string RemoveWhiteSpaces(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            while (text.Contains("  ")) text = text.Replace("  ", " ");
            return text.Trim();
        }

        protected IEnumerable<IDomObject> SearchChildren(IDomObject dom, bool tagNamesIsChildTags, params string[] tagNames)
        {
            if (!dom.HasChildren || tagNames == null || !tagNames.Any()) return Enumerable.Empty<IDomObject>();

            var results = new List<IDomObject>();

            if (tagNamesIsChildTags && dom.HasChildren && dom.ChildNodes.Any(ch => tagNames.Any(item => item == ch.NodeName.ToLower())))
                results.Add(dom);

            if (dom.HasChildren)
                foreach (var childNode in dom.ChildNodes)
                {
                    if (!tagNamesIsChildTags && tagNames.Contains(childNode.NodeName))
                        results.Add(childNode);
                    else if (childNode.HasChildren)
                        results.AddRange(SearchChildren(childNode, tagNamesIsChildTags, tagNames));
                }

            return results;
        }

        protected IEnumerable<IDomObject> SearchChildren(IDomObject dom, string patternID = null, string patternClass = null)
        {
            List<IDomObject> result = new List<IDomObject>();
            foreach (IDomObject childNode in dom.ChildNodes)
            {
                bool matchById = NodeHasAttribute(childNode, "id", patternID);
                bool matchByClass = NodeHasAttribute(childNode, "class", patternClass);

                if (matchById || matchByClass)
                    result.Add(childNode);

                if (childNode.HasChildren)
                    result.AddRange(SearchChildren(childNode, patternID, patternClass));
            }
            return result;
        }

        protected bool NodeHasAttribute(IDomObject dom, string attribute, string pattern)
        {
            // check input parameters
            if (string.IsNullOrEmpty(attribute)) return false;
            if (string.IsNullOrEmpty(pattern)) return false;
            // get attribute value
            string value = dom.GetAttribute(attribute);
            if (string.IsNullOrEmpty(value)) return false;
            // check pattern in the value
            return Regex.Match(value, pattern, RegexOptions.IgnoreCase).Success;
        }

        #endregion
    }
}
