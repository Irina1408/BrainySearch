using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainySearch.Logic.Search.Base;
using System.Net;
using System.Collections.Specialized;
using BrainySearch.Logic.Parser;

namespace BrainySearch.Logic.Search.DuckDuckGo
{
    /// <summary>
    /// DOES NOT WORK
    /// </summary>
    public class DuckDuckGoService : SearchService, IDuckDuckGoService
    {
        #region Private fields

        private const string URL = "https://duckduckgo.com/";

        #endregion

        #region Initialization

        public DuckDuckGoService() : base()
        {
            SearchParameters.Limit = 30;
        }

        public DuckDuckGoService(ISearchParameters searchParameters) : base(searchParameters)
        { }

        #endregion

        #region Public methods 

        public override SearchResults<ISearchResult> Search(string searchString)
        {
            // result
            var res = new SearchResults<ISearchResult>();

            try
            {
                string html = "";

                // get json data
                using (var webClient = new WebClient())
                {
                    NameValueCollection nameValueCollection = new NameValueCollection();
                    //nameValueCollection.Add("format", "json");
                    nameValueCollection.Add("q", searchString);
                    //nameValueCollection.Add("t", "h_");
                    //nameValueCollection.Add("ia", "web");
                    //nameValueCollection.Add("kl", "ru-ru");

                    webClient.QueryString.Add(nameValueCollection);
                    html = webClient.DownloadString(URL);
                }

                // get all results
                for(int i = 0; ; i++)
                {
                    // get result
                    var resultHtml = HtmlParseHelper.GetInnerHtmlByTagId(html, "r1-" + i);
                    // if result is not found exit for
                    if (string.IsNullOrEmpty(resultHtml)) break;
                    // parse result
                    var title = HtmlParseHelper.GetInnerHtmlByTagClassName(resultHtml, "result__a");
                    var text = HtmlParseHelper.GetInnerHtmlByTagClassName(resultHtml, "result__snippet");
                    var link = HtmlParseHelper.GetAttributeByTagClassName(resultHtml, "result__url", "href");
                    // add result to collection
                    res.Results.Add(new SearchResult() { Title = title, Text = text, Link = link });
                }

            }
            catch (WebException ex)
            {
                res.ErrorMessage = ex.Message;
            }

            return res;
        }

        #endregion
    }
}
