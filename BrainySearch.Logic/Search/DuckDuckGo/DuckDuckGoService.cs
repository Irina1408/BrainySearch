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
    public class DuckDuckGoService : IDuckDuckGoService
    {
        public DuckDuckGoService()
        {
            Language = "en";
            MaxPagesCount = 30;
        }

        public string Language { get; set; }

        public int MaxPagesCount { get; set; }

        public string URL => "https://duckduckgo.com/";

        public SearchResults Search(string searchString)
        {
            // result
            var res = new SearchResults();

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
                    var resultHtml = ParserBase.GetInnerHtmlByTagId(html, "r1-" + i);
                    // if result is not found exit for
                    if (string.IsNullOrEmpty(resultHtml)) break;
                    // parse result
                    var title = ParserBase.GetInnerHtmlByTagClassName(resultHtml, "result__a");
                    var description = ParserBase.GetInnerHtmlByTagClassName(resultHtml, "result__snippet");
                    var link = ParserBase.GetAttributeByTagClassName(resultHtml, "result__url", "href");
                    // add result to collection
                    res.Results.Add(new SearchResult() { Title = title, Description = description, Link = link });
                }

            }
            catch (WebException ex)
            {
                res.ErrorMessage = ex.Message;
            }

            return res;
        }
    }
}
