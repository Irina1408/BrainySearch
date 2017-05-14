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
using System.Threading;

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
        private int searchResultsLimit;
        private SearchResults<ISearchResult> res;
        private bool search;

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
                    webClient.QueryString.Add("qid", GetQid(webClient).Result);

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

        public SearchResults<ISearchResult> SearchAsync(string searchString)
        {
            // preparing
            res = new SearchResults<ISearchResult>();
            search = true;

            try
            {
                // nameValueCollection for search
                var nameValueCollection = PrepareSearchParameters(searchString);
                // create tasks for seacrh requests
                var searchTasks = new List<Task>();
                // calculate max pages count -> one StartPage result page contains max 10 results
                var pageLimit = (int)Math.Ceiling(((double)searchResultsLimit) / 10);
                // create tasks for all requests
                for (int iPage = 0; iPage < pageLimit; iPage++)
                {
                    if (1 + iPage * 10 > searchResultsLimit || !search) break;
                    int tmp = iPage;

                    searchTasks.Add(Task.Run(() => Search(nameValueCollection, tmp)));
                }

                // wait all tasks are ended
                Task.WaitAll(searchTasks.ToArray(), CancellationToken.None);
                // dispose all tasks
                foreach (var t in searchTasks)
                    t.Dispose();

                if(SearchParameters.Limit <= 0 && searchResultsLimit == 80 && search)
                {
                    int iPage = pageLimit;

                    while (search)
                    {
                        searchResultsLimit += 10;
                        Search(nameValueCollection, iPage);
                        iPage += 1;
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

        private Task SearchAsync(NameValueCollection nameValueCollection, int iPage)
        {
            return Task.Run(() => Search(nameValueCollection, iPage));
        }

        private void Search(NameValueCollection nameValueCollection, int iPage)
        {
            using (var webClient = new WebClient())
            {
                // prepare web client
                webClient.Encoding = System.Text.Encoding.UTF8;
                webClient.QueryString.Add(nameValueCollection);
                webClient.QueryString.Set("startat", (iPage * 10).ToString());
                // get page html
                var html = webClient.DownloadString(URL);
                // get all results
                for (int i = 1 + iPage * 10; i < (iPage + 1) * 10; i++)
                {
                    if (i > searchResultsLimit)
                    {
                        search = false;
                        break;
                    }
                    // parse result
                    var resultHtml = HtmlParseHelper.GetInnerHtmlByTagId(html, "result" + i);
                    // if result is not found exit for
                    if (string.IsNullOrEmpty(resultHtml)) break;
                    // parse link
                    var link = HtmlParseHelper.GetAttributeByTagId(resultHtml, "title_" + i, "href");
                    // check link does not exist in the collection
                    lock(res)
                    {
                        if (res.Results.Any(item => item.Link == link))
                        {
                            // dublicated links exist when search is ended
                            search = false;
                            break;
                        }
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

        private NameValueCollection PrepareSearchParameters(string searchString)
        {
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

            using (var webClient = new WebClient())
            {
                // prepare web client
                webClient.Encoding = System.Text.Encoding.UTF8;
                webClient.QueryString.Add(nameValueCollection);
                // get html page for getting parameters
                var html = webClient.DownloadString(URL);
                // set generated qid
                nameValueCollection.Add("qid", GetQid(html));
                // update search limit
                UpdateSearchLimit(html);
            }

            return nameValueCollection;
        }

        private string GetQid(string html)
        {
            // parse qid html page part
            var qidHtml = HtmlParseHelper.GetHtmlByTagNameAndAttribute(html, "input", "name", "qid");
            // check qid html part was found
            if (string.IsNullOrEmpty(qidHtml)) return null;
            // parse qid value
            return HtmlParseHelper.GetAttribute(qidHtml, "value");
        }

        private void UpdateSearchLimit(string html)
        {
            if (SearchParameters.Limit > 0)
            {
                searchResultsLimit = SearchParameters.Limit;
                return;
            }
            // set search limit 0
            searchResultsLimit = 0;
            // parse qid html page part
            var numbersStList = HtmlParseHelper.GetHtmlTagsByClassName(html, "numbers_st");
            foreach(var numst in numbersStList)
            {
                var val = HtmlParseHelper.GetAttribute(numst, "id");
                int num = 0;
                if(int.TryParse(val, out num) && num * 10 > searchResultsLimit)
                {
                    searchResultsLimit = num * 10;
                }
            }
        }

        private async Task<string> GetQid(WebClient webClient)
        {
            // download html page
            var html = await webClient.DownloadStringTaskAsync(URL);
            // parse qid html page part
            var qidHtml = HtmlParseHelper.GetHtmlByTagNameAndAttribute(html, "input", "name", "qid");
            // check qid html part was found
            if (string.IsNullOrEmpty(qidHtml)) return null;
            // parse qid value
            return HtmlParseHelper.GetAttribute(qidHtml, "value");
        }

        #endregion
    }
}
