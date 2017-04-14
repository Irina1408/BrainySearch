using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainySearch.Logic.Search.Base;
using System.Net;
using System.Collections.Specialized;
using BrainySearch.Logic.Parser;
using System.Globalization;

namespace BrainySearch.Logic.Search.StartPage
{
    /// <summary>
    /// StartPage search service
    /// </summary>
    public class StartPageService : SearchService<StartPageParameters, ISearchResult>, IStartPageService
    {
        #region Private fields

        // example: https://s10-eu4.startpage.com/do/search?cmd=process_search&language=english&qid=LELOTLQRSSPO257GFUQJUE&rcount=1&rl=NONE&abp=-1&query=asp.net&cat=web&t=&startat=30
        private const string URL = "https://s10-eu4.startpage.com/do/search";

        #endregion

        #region Initialization

        public StartPageService() : base()
        {
            SearchParameters = new StartPageParameters();
        }

        public StartPageService(StartPageParameters searchParameters)
        {
            SearchParameters = searchParameters;
        }

        #endregion

        #region Public methods 

        public override SearchResults<ISearchResult> Search(string searchString)
        {
            // result
            var res = new SearchResults<ISearchResult>();

            try
            {
                // search
                using (var webClient = new WebClient())
                {
                    // prepare web client
                    webClient.Encoding = System.Text.Encoding.UTF8;
                    // get full language name
                    var lang = CultureInfo.GetCultureInfo(SearchParameters.Language).EnglishName.ToLower();

                    // fill attributes
                    NameValueCollection nameValueCollection = new NameValueCollection();
                    nameValueCollection.Add("cmd", "process_search");
                    nameValueCollection.Add("language", lang);
                    nameValueCollection.Add("rcount", "1");
                    nameValueCollection.Add("rl", "NONE");
                    nameValueCollection.Add("abp", "-1");
                    nameValueCollection.Add("query", searchString);
                    nameValueCollection.Add("cat", "web");
                    nameValueCollection.Add("t", "");
                    nameValueCollection.Add("startat", "0");
                    webClient.QueryString.Add(nameValueCollection);
                    
                    // set generated qid
                    // "LELOTLQRSSPO257GFUQJUE"
                    webClient.QueryString.Add("qid", GetQid(webClient));

                    // calculate max pages count -> one StartPage result page contains max 10 results
                    var pageLimit = (int)Math.Round(((double)SearchParameters.Limit) / 10, 0, MidpointRounding.AwayFromZero);
                    var search = true;

                    for(int iPage = 0; iPage < pageLimit; iPage++)
                    {
                        if (1 + iPage * 10 > SearchParameters.Limit || !search) break;

                        // get search result html
                        webClient.QueryString.Set("startat", (iPage * 10).ToString());
                        var html = webClient.DownloadString(URL);

                        // get all results
                        for (int i = 1 + iPage * 10; ; i++)
                        {
                            if (i > SearchParameters.Limit) break;
                            // get result
                            var resultHtml = HtmlParseHelper.GetInnerHtmlByTagId(html, "result" + i);
                            // if result is not found exit for
                            if (string.IsNullOrEmpty(resultHtml)) break;
                            // parse link
                            var link = HtmlParseHelper.GetAttributeByTagId(resultHtml, "title_" + i, "href");
                            // check link does not exist in the collection
                            if(res.Results.Any(item => item.Link == link))
                            {
                                // dublicated links exist when search is ended
                                search = false;
                                break;
                            }
                            // add result to collection
                            res.Results.Add(new SearchResult()
                            {
                                Title = HtmlParseHelper.GetInnerHtmlByTagClassName(resultHtml, "result_url_heading"),
                                Text = SearchParameters.LoadText 
                                            ? HtmlParseHelper.GetInnerHtmlByTagClassName(resultHtml, "desc clk") 
                                            : null,
                                Link = link
                            });
                        }
                    }
                } 
            }
            catch (WebException ex)
            {
                res.ErrorMessage = ex.Message;
            }

            return res;
        }

        #endregion

        #region Private methods

        private string GetQid(WebClient webClient)
        {
            var html = webClient.DownloadString(URL);
            var qidHtml = HtmlParseHelper.GetHtmlByTagNameAndAttribute(html, "input", "name", "qid");

            if (string.IsNullOrEmpty(qidHtml)) return null;

            return HtmlParseHelper.GetAttribute(qidHtml, "value");
        }

        #endregion
    }
}
