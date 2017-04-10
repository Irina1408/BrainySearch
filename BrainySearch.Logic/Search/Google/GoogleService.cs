using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using GoogleCSE;
using System.IO;
using BrainySearch.Logic.Search.Base;

namespace BrainySearch.Logic.Search
{
    public class GoogleService : IGoogleService
    {
        private const string CX = "015536730667522181619:rlum22alhai";
        private const string API_KEY = "AIzaSyCeDqFm6I3CrUnePq2dZxrrIlorR77gpw8";
        
        public GoogleService()
        {
            Language = "en";
            MaxPagesCount = 10;
        }

        public string Language { get; set; }

        public int MaxPagesCount { get; set; }

        public string URL { get { return "https://www.google.com/search"; } }

        public SearchResults Search(string searchString)
        {
            var res = new SearchResults();

            var sr = new GoogleCSE.GoogleSearch(CX, Language, null, 10, MaxPagesCount, 1, GoogleSearchMethod.CSE, API_KEY);
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
