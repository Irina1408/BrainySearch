using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Parser
{
    using CsQuery;

    public static class HtmlParseHelper
    {
        #region Get inner html

        /// <summary>
        /// Returns inner html of tag with entered id
        /// </summary>
        public static string GetInnerHtmlByTagId(string html, string id)
        {
            var cq = CQ.Create(html);
            return cq.Document.GetElementById(id)?.InnerHTML;
        }

        /// <summary>
        /// Returns inner html of tag with entered class name
        /// </summary>
        public static string GetInnerHtmlByTagClassName(string html, string className, bool strictEquality = true)
        {
            var cq = CQ.Create(html);
            for(int i = 0; i < cq.Length ; i++)
            {
                if(strictEquality && cq[i].ClassName == className || !strictEquality && cq[i].ClassName.Contains(className))
                {
                    return cq[i].InnerHTML;
                }
                else
                {
                    if(cq[i].InnerHtmlAllowed)
                    {
                        var innerHtml = GetInnerHtmlByTagClassName(cq[i].InnerHTML, className);
                        if(!string.IsNullOrEmpty(innerHtml))
                        {
                            return innerHtml;
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region Get html

        /// <summary>
        /// Returns html of tag with entered id
        /// </summary>
        public static string GetHtmlByTagId(string html, string id)
        {
            var cq = CQ.Create(html);
            return cq.Document.GetElementById(id)?.Render();
        }

        /// <summary>
        /// Returns html of tag with entered name and attribute name and value
        /// </summary>
        public static string GetHtmlByTagNameAndAttribute(string html, string tagName, string attributeName, string attributeValue)
        {
            var cq = CQ.Create(html);
            var elements = cq.Document.GetElementsByTagName(tagName);

            foreach(var elem in elements)
            {
                if (elem.GetAttribute(attributeName) == attributeValue)
                    return elem.Render();
            }

            return null;
        }

        public static List<string> GetHtmlTags(string html, string tagName)
        {
            var cq = CQ.Create(html);
            return cq.Document.GetElementsByTagName(tagName).Select(item => item.Render()).ToList();
        }

        #endregion

        #region Get attribute

        /// <summary>
        /// Returns attribute value of tag with class name and attribute name
        /// </summary>
        public static string GetAttributeByTagClassName(string html, string className, string attributeName)
        {
            var dom = GetDomObjectByTagClassName(html, className);
            return dom?[attributeName];
        }

        /// <summary>
        /// Returns attribute value of tag with tag id and attribute name
        /// </summary>
        public static string GetAttributeByTagId(string html, string id, string attributeName)
        {
            var cq = CQ.Create(html);
            return cq.Document.GetElementById(id)?[attributeName];
        }

        /// <summary>
        /// Returns attribute value of tag with attribute name
        /// </summary>
        public static string GetAttribute(string html, string attributeName)
        {
            var cq = CQ.Create(html);
            return cq.Attr(attributeName);
        }

        #endregion

        #region Get content

        public static string GetFullContent(string html)
        {
            var cq = CQ.Create(html);

            return cq.Document.InnerText;
        }

        #endregion

        #region Get Dom Object

        /// <summary>
        /// Returns dom object of tag with entered class name
        /// </summary>
        public static IDomObject GetDomObjectByTagClassName(string html, string className)
        {
            var cq = CQ.Create(html);
            for (int i = 0; i < cq.Length; i++)
            {
                if (cq[i].ClassName == className)
                {
                    return cq[i];
                }
                else
                {
                    if (cq[i].InnerHtmlAllowed)
                    {
                        var dom = GetDomObjectByTagClassName(cq[i].Render(), className);
                        if (dom != null)
                        {
                            return dom;
                        }
                    }
                }
            }

            return null;
        }

        public static IDomObject GetDomObjectByTagId(string html, string id)
        {
            var cq = CQ.Create(html);
            return cq.Document.GetElementById(id);
        }

        #endregion
    }
}
