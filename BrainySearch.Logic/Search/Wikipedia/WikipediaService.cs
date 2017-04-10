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

namespace BrainySearch.Logic.Search.Wikipedia
{
    public class WikipediaService : IWikipediaService
    {
        public WikipediaService()
        {
            Language = "en";
            MaxPagesCount = 10;
            SearchFullDescription = false;
        }

        public string Language { get; set; }

        public int MaxPagesCount { get; set; }

        public string URL => "https://{0}.wikipedia.org/w/api.php";

        public bool SearchFullDescription { get; set; }

        public SearchResults Search(string searchString)
        {
            var res = new SearchResults();

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
                    nameValueCollection.Add("limit", MaxPagesCount.ToString());
                    nameValueCollection.Add("namespace", "0");
                    nameValueCollection.Add("format", "json");
                    webClient.QueryString.Add(nameValueCollection);

                    // get json of all pages
                    var json = webClient.DownloadString(string.Format(URL, Language));
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

                    if(SearchFullDescription)
                    {
                        // prepare web client
                        webClient.QueryString.Set("action", "query");
                        webClient.QueryString.Add("prop", "extracts");
                        webClient.QueryString.Add("explaintext", "");
                        webClient.QueryString.Add("exintro", "");
                        webClient.QueryString.Add("redirects", "");
                    }

                    for (int i = 0; i < titles.Count; i++)
                    {
                        res.Results.Add(new SearchResult()
                        {
                            Title = titles[i].ToString(),
                            Description = SearchFullDescription ? GetFullDescription(webClient, titles[i].ToString()) : descriptions[i].ToString(),
                            Link = links[i].ToString()
                        });
                    }
                }
            }
            catch (WebException ex)
            {
                res.ErrorMessage = ex.Message;
            }

            return res;
        }
        
        private string GetFullDescription(WebClient webClient, string title)
        {
            webClient.QueryString.Set("titles", title);

            // get json
            var json = webClient.DownloadString(string.Format(URL, Language));
            // parse json
            JObject res = JObject.Parse(json);
            return res["query"]["pages"].First.First["extract"].ToString();
        }
    }
}
