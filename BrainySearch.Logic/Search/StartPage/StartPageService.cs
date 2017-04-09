using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainySearch.Logic.Search.Base;
using System.Net;
using System.Collections.Specialized;
using BrainySearch.Logic.Parser;

namespace BrainySearch.Logic.Search.StartPage
{
    public class StartPageService : IStartPageService
    {
        public StartPageService()
        {
            Language = "english";
            LanguageCode = "en";
            MaxPagesCount = 10;
        }

        public string Language { get; set; }

        public string LanguageCode { get; set; }

        public int MaxPagesCount { get; set; }

        // example: https://s10-eu4.startpage.com/do/search?cmd=process_search&language=english&qid=LELOTLQRSSPO257GFUQJUE&rcount=1&rl=NONE&abp=-1&query=asp.net&cat=web&t=&startat=30
        public string URL { get { return "https://s10-eu4.startpage.com/do/search"; } }
        
        public SearchResults Search(string searchString)
        {
            // result
            var res = new SearchResults();

            try
            {
                // search
                using (var webClient = new WebClient())
                {
                    // fill attributes
                    NameValueCollection nameValueCollection = new NameValueCollection();
                    nameValueCollection.Add("cmd", "process_search");
                    nameValueCollection.Add("language", Language.ToLower());
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
                    var maxPagesCount = (int)Math.Round(((double)MaxPagesCount) / 10, 0, MidpointRounding.AwayFromZero);
                    var search = true;

                    for(int iPage = 0; iPage < maxPagesCount; iPage++)
                    {
                        if (1 + iPage * 10 > MaxPagesCount || !search) break;

                        // get search result html
                        webClient.QueryString.Set("startat", (iPage * 10).ToString());
                        var html = webClient.DownloadString(URL);

                        // get all results
                        for (int i = 1 + iPage * 10; ; i++)
                        {
                            if (i > MaxPagesCount) break;
                            // get result
                            var resultHtml = ParserBase.GetInnerHtmlByTagId(html, "result" + i);
                            // if result is not found exit for
                            if (string.IsNullOrEmpty(resultHtml)) break;
                            // parse link
                            var link = ParserBase.GetAttributeByTagId(resultHtml, "title_" + i, "href");
                            // check link does not exist in the collection
                            if(res.Results.Any(item => item.Link == link))
                            {
                                // dublicated links exist when search is ended
                                search = false;
                                break;
                            }
                            // parse other parts
                            var title = ParserBase.GetInnerHtmlByTagClassName(resultHtml, "result_url_heading");
                            var description = ParserBase.GetInnerHtmlByTagClassName(resultHtml, "desc clk");
                            // add result to collection
                            res.Results.Add(new SearchResult() { Title = title, Description = description, Link = link });
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

        private string GetQid(WebClient webClient)
        {
            var html = webClient.DownloadString(URL);
            var qidHtml = ParserBase.GetHtmlByTagNameAndAttribute(html, "input", "name", "qid");

            if (string.IsNullOrEmpty(qidHtml)) return null;

            return ParserBase.GetAttribute(qidHtml, "value");
        }
    }
}
