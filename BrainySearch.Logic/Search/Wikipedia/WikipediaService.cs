using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainySearch.Logic.Search.Base;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BrainySearch.Logic.Parser;

namespace BrainySearch.Logic.Search.Wikipedia
{
    public class WikipediaService : SearchService<WikipediaParameters, ISearchResult>, IWikipediaService
    {
        #region Private fields

        private const string URL = "https://{0}.wikipedia.org/w/api.php";

        #endregion

        #region Initialization

        public WikipediaService() : base()
        {
            SearchParameters = new WikipediaParameters();
        }

        public WikipediaService(WikipediaParameters searchParameters)
        {
            SearchParameters = searchParameters;
        }

        #endregion

        #region Public methods 

        public override SearchResults<ISearchResult> Search(string searchString)
        {
            var res = new SearchResults<ISearchResult>();

            try
            {
                // search
                using (var webClient = new WebClient())
                {
                    // prepare web client
                    webClient.Encoding = System.Text.Encoding.UTF8;

                    // fill attributes
                    NameValueCollection nameValueCollection = new NameValueCollection();
                    nameValueCollection.Add("action", "opensearch");
                    nameValueCollection.Add("search", searchString);
                    nameValueCollection.Add("limit", SearchParameters.Limit.ToString());
                    nameValueCollection.Add("namespace", "0");
                    nameValueCollection.Add("format", "json");
                    webClient.QueryString.Add(nameValueCollection);

                    // get json of all pages
                    var json = webClient.DownloadString(string.Format(URL, SearchParameters.Language));
                    // deserialize json
                    var resp = JsonConvert.DeserializeObject<object[]>(json);

                    if(resp.Length < 4)
                    {
                        res.ErrorMessage = "Responce does not have a correct format.";
                        return res;
                    }

                    var titles = (Newtonsoft.Json.Linq.JArray)resp[1];
                    var descriptions = (Newtonsoft.Json.Linq.JArray)resp[2];
                    var links = (Newtonsoft.Json.Linq.JArray)resp[3];

                    for (int i = 0; i < titles.Count; i++)
                    {
                        res.Results.Add(new SearchResult()
                        {
                            Title = titles[i].ToString(),
                            Description = descriptions[i].ToString(),
                            Link = links[i].ToString()
                        });
                    }

                    // refill definitions correspond to type
                    RefillDescriptions(webClient, res);
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

        private void RefillDescriptions(WebClient webClient, SearchResults<ISearchResult> res)
        {
            // refill descriptions
            switch (SearchParameters.DescriptionType)
            {
                case DescriptionType.Full:
                    // prepare web client
                    webClient.QueryString.Set("action", "query");
                    webClient.QueryString.Add("prop", "extracts");
                    webClient.QueryString.Add("explaintext", "");
                    webClient.QueryString.Add("exintro", "");
                    webClient.QueryString.Add("redirects", "");
                    // fill descriptions
                    foreach (var r in res.Results)
                    {
                        r.Description = GetFullDescription(webClient, r.Title);
                    }
                    break;

                case DescriptionType.WithTags:
                    webClient.QueryString.Clear();
                    // fill descriptions
                    foreach (var r in res.Results)
                    {
                        r.Description = GetDescriptionWithTags(webClient, r.Link);
                    }
                    break;
            }
        }

        private string GetFullDescription(WebClient webClient, string title)
        {
            webClient.QueryString.Set("titles", title);

            // get json
            var json = webClient.DownloadString(string.Format(URL, SearchParameters.Language));
            // parse json
            JObject res = JObject.Parse(json);
            return res["query"]["pages"].First.First["extract"].ToString();
        }

        private string GetDescriptionWithTags(WebClient webClient, string link)
        {
            var wikiParser = new WikipediaParser();
            var html = webClient.DownloadString(link);

            return wikiParser.GetDefinitionHtml(html);
        }

        #endregion
    }
}
