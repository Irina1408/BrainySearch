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
                    searchResults.Results.AddRange(
                        wikiSr.Results.Select(item => new BrainySearchSearchResult(item) { ParsePage = false }));
                }
            }

            // search by full search string
            var sr = Search(searchString);
            // add search results in full results list without dublicates
            searchResults.Results.AddRange(
                sr.Results.Where(item => !searchResults.Results.Any(it => it.Link == item.Link))
                .Select(item => new BrainySearchSearchResult(item)));
            // copy error if it is
            searchResults.ErrorMessage = sr.ErrorMessage;

            return searchResults;
        }

        public SearchResults Search(string searchString)
        {
            // 1. Google search
            //if (Search(ref searchResults, searchString, new GoogleService())) return searchResults;

            // 2. Gigablast
            //if (Search(ref searchResults, searchString, new GigablastService())) return searchResults;

            // 3. DuckDuckGo
            //if (Search(ref searchResults, searchString, new DuckDuckGoService())) return searchResults;

            // 4. StartPage
            return Search(searchString, new StartPageService() { MaxPagesCount = 100 });
        }

        private SearchResults Search(string searchString, IWebSearchService searchService)
        {
            try
            {
                searchService.Language = Language;
                var searchResults = searchService.Search(searchString);

                if (!searchResults.HasErrors)
                {
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
    }
}
