﻿using BrainySearch.Logic.Parser;
using BrainySearch.Logic.Search.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;

namespace BrainySearch.Logic.Search.Gigablast
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class GigablastService : IGigablastService
    {
        public GigablastService()
        {
            Language = "en";
            MaxPagesCount = 10;
        }

        public string Language { get; set; }

        public int MaxPagesCount { get; set; }

        public string URL => "https://www.gigablast.com/search";

        public SearchResults Search(string searchString)
        {
            // result
            var res = new SearchResults();

            try
            {
                string json = "";

                // get json data
                using (var webClient = new WebClient())
                {
                    NameValueCollection nameValueCollection = new NameValueCollection();
                    nameValueCollection.Add("format", "json");
                    nameValueCollection.Add("c", "main");
                    nameValueCollection.Add("q", searchString);
                    nameValueCollection.Add("n", MaxPagesCount.ToString());
                    nameValueCollection.Add("qlang", Language);
                    nameValueCollection.Add("rand", "1491590369709");
                    nameValueCollection.Add("rxieu", "3784892529");

                    webClient.QueryString.Add(nameValueCollection);
                    json = webClient.DownloadString(URL);
                }

                // deserialize json
                var resp = JsonConvert.DeserializeObject<GigablastResponse>(json);

                foreach (var r in resp.results)
                {
                    res.Results.Add(new SearchResult() { Title = r.title, Description = r.sum, Link = r.url });
                }

            }
            catch(WebException ex)
            {
                res.ErrorMessage = ex.Message;
            }

            return res;
        }
    }
}
