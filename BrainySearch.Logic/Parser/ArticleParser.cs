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
    using System.Web;

    public class ArticleParser : TextParser
    {
        #region IParser implementation

        /// <summary>
        /// Parse html and returns result text
        /// </summary>
        public override string Parse(string html)
        {
            // clean html
            html = RemoveEmptySpaces(html);
            html = RemoveWhiteSpaces(html);
            html = RemoveComments(html);

            // cq main html page document
            var document = CQ.Create(html).Document;
            if (document == null) return null;
            // get html body document
            IDomObject bodyDom = document.Body;
            if (bodyDom == null) bodyDom = document;
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
                // loop on all results with children
                foreach(var r in res.Where(item => item.HasChildren))
                {                    
                    // find all tags with children tags "p"
                    foreach(var pTag in SearchChildren(r, true, "p"))
                    {
                        sb.AppendLine(HttpUtility.HtmlDecode(HtmlParseHelper.GetInnerText(pTag)));
                    }
                }
            }

            if (sb.Length == 0)
            {
                // get all tags with text
                foreach (var ch in SearchChildren(bodyDom, true, "p"))
                {
                    sb.AppendLine(HttpUtility.HtmlDecode(HtmlParseHelper.GetInnerText(ch)));
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Private functions

        protected string RemoveEmptySpaces(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Replace("\t", " ").Replace("\n", " ").Replace("\r", " "); ;
        }

        protected string RemoveWhiteSpaces(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            while (text.Contains("  ")) text = text.Replace("  ", " ");
            return text.Trim();
        }

        protected string RemoveComments(string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;
            return CQ.Create(html).Render(DomRenderingOptions.RemoveComments);
        }

        protected IEnumerable<IDomObject> SearchChildren(IDomObject dom, bool tagNamesIsChildTags, params string[] tagNames)
        {
            if (!dom.HasChildren || tagNames == null || !tagNames.Any() || !IsValidForSearchDom(dom)) return Enumerable.Empty<IDomObject>();

            var results = new List<IDomObject>();

            if (tagNamesIsChildTags && dom.HasChildren && dom.ChildNodes.Any(ch => tagNames.Any(item => item == ch.NodeName.ToLower())))
                results.Add(dom);
            else if(dom.HasChildren)
                foreach (var childNode in dom.ChildNodes)
                {
                    if (!tagNamesIsChildTags && tagNames.Contains(childNode.NodeName.ToLower()))
                        results.Add(childNode);
                    else if (childNode.HasChildren)
                        results.AddRange(SearchChildren(childNode, tagNamesIsChildTags, tagNames));
                }

            return results;
        }

        protected IEnumerable<IDomObject> SearchChildren(IDomObject dom, string patternID = null, string patternClass = null)
        {
            if (!IsValidForSearchDom(dom)) return Enumerable.Empty<IDomObject>();

            List<IDomObject> result = new List<IDomObject>();
            foreach (IDomObject childNode in dom.ChildNodes)
            {
                bool matchById = NodeHasAttribute(childNode, "id", patternID);
                bool matchByClass = NodeHasAttribute(childNode, "class", patternClass);

                if (matchById || matchByClass)
                    result.Add(childNode);
                else if (childNode.HasChildren)
                    result.AddRange(SearchChildren(childNode, patternID, patternClass));
            }
            return result;
        }

        protected bool IsValidForSearchDom(IDomObject dom)
        {
            string[] excludedFromSearchItems = new[] { "footer", "script", "iframe", "select" };

            if (!string.IsNullOrEmpty(dom.Id) && excludedFromSearchItems.Any(item => dom.Id.ToLower().Contains(item)))
                return false;

            if (!string.IsNullOrEmpty(dom.ClassName) && excludedFromSearchItems.Any(item => dom.ClassName.ToLower().Contains(item)))
                return false;

            if (!string.IsNullOrEmpty(dom.NodeName) && excludedFromSearchItems.Any(item => dom.NodeName.ToLower().Contains(item)))
                return false;

            return true;
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
