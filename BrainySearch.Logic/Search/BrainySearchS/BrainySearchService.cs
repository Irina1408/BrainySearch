using BrainySearch.Logic.Search.Base;
using BrainySearch.Logic.Search.StartPage;
using BrainySearch.Logic.Search.Wikipedia;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace BrainySearch.Logic.Search.BrainySearchS
{
    public class BrainySearchService : IBrainySearchService
    {
        public string Language { get; set; }

        public SearchResults Search(string searchString, string[] keyWords)
        {
            // result
            var searchResults = new SearchResults();

            // detect search language
            DetectLanguage(searchString);

            if(keyWords != null)
            {
                var wikiService = new WikipediaService()
                {
                    SearchFullDescription = true,
                    MaxPagesCount = 1
                };

                // search keyword definitions
                foreach (var keyWord in keyWords)
                {
                    // search key word definition in the wikipedia
                    var wikiSr = Search(keyWord, wikiService);
                    // add search results in full results list (ignore errors)
                    searchResults.Results.AddRange(wikiSr.Results);
                }
            }

            // search by full search string
            var sr = Search(searchString, new StartPageService() { MaxPagesCount = 100 });
            // add search results in full results list without dublicates
            searchResults.Results.AddRange(sr.Results.Where(item => !searchResults.Results.Any(it => it.Link == item.Link)));
            // copy error if it is
            searchResults.ErrorMessage = sr.ErrorMessage;

            return searchResults;
        }

        public SearchResults Search(string searchString)
        {
            DetectLanguage(searchString);

            // 1. Google search
            //if (Search(ref searchResults, searchString, new GoogleService())) return searchResults;

            // 2. Gigablast
            //if (Search(ref searchResults, searchString, new GigablastService())) return searchResults;

            // 3. DuckDuckGo
            //if (Search(ref searchResults, searchString, new DuckDuckGoService())) return searchResults;

            // 4. StartPage
            return Search(searchString, new StartPageService());
        }

        private bool Search(ref SearchResults searchResults, string searchString, IWebSearchService searchService)
        {
            if (searchService == null) return false;

            try
            {
                //searchService.Language = Language;
                searchService.MaxPagesCount = 100;
                var sr = searchService.Search(searchString);

                if (!sr.HasErrors)
                {
                    FixLinks(ref sr);

                    foreach(var r in sr.Results)
                    {
                        searchResults.Results.Add(r);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                searchResults.ErrorMessage = ex.Message;
                return false;
            }

            return false;
        }

        private SearchResults Search(string searchString, IWebSearchService searchService)
        {
            try
            {
                searchService.Language = Language;
                //searchService.MaxPagesCount = 100;
                var searchResults = searchService.Search(searchString);

                if (!searchResults.HasErrors)
                {
                    FixLinks(ref searchResults);
                    return searchResults;
                }
            }
            catch (Exception ex)
            {
                var searchResults = new SearchResults();
                searchResults.ErrorMessage = ex.Message;
                return searchResults;
            }

            return new SearchResults();
        }

        private void FixLinks(ref SearchResults searchResults)
        {
            foreach(var sr in searchResults.Results)
            {
                if(sr.Link != null)
                {
                    if(!sr.Link.StartsWith("http"))
                        sr.Link = string.Format("https://{0}", sr.Link);

                    if (sr.Link.EndsWith("/"))
                        sr.Link = sr.Link.Substring(0, sr.Link.Length - 1);
                }
            }
        }

        private void DetectLanguage(string text)
        {
            Language = LangDetection.LangDetector.DetectLanguage(text);
            //CultureInfo.GetCultureInfo(LanguageCode).EnglishName;
        }
    }
}
