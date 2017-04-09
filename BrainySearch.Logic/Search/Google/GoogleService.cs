using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleSearchApi;
using Newtonsoft.Json;
using System.Net;
using GoogleCSE;
using System.IO;
using Google.Apis.Services;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using GoogleApi.Entities.Search;
using System.Collections.Specialized;
using System.Net.Http;
using CsQuery;
using BrainySearch.Logic.Search.Base;

namespace BrainySearch.Logic.Search
{
    public class GoogleService : IGoogleService
    {
        private const string CX = "015536730667522181619:rlum22alhai";
        private const string API_KEY = "AIzaSyCeDqFm6I3CrUnePq2dZxrrIlorR77gpw8";
        
        public GoogleService()
        {
            Language = "english";
            LanguageCode = "en";
            MaxPagesCount = 10;
        }

        public string Language { get; set; }

        public string LanguageCode { get; set; }

        public int MaxPagesCount { get; set; }

        public string URL { get { return "https://www.google.com/search"; } }

        public SearchResults Search(string searchString)
        {
            var res = new SearchResults();

            var sr = new GoogleCSE.GoogleSearch(CX, LanguageCode, null, 10, MaxPagesCount, 1, GoogleSearchMethod.CSE, API_KEY);
            try
            {
                var r = sr.Search(searchString);
                foreach (var item in r)
                {
                    res.Results.Add(new SearchResult() { Title = item.Title, Link = item.Url, Description = item.Description });
                }
            }
            catch (WebException ex)
            {
                string responseText;

                using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                }

                res.ErrorMessage = responseText;
            }

            return res;
        }
    }    
}
