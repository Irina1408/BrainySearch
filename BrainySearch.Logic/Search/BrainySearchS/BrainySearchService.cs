using BrainySearch.Logic.Search.Base;
using BrainySearch.Logic.Search.StartPage;
using BrainySearch.Logic.Search.Wikipedia;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace BrainySearch.Logic.Search.BrainySearchS
{
    public class BrainySearchService : SearchService, IBrainySearchService
    {
        #region Public methods

        public SearchResults<ISearchResult> Search(string searchString, string[] keyWords)
        {
            // result
            var searchResults = new SearchResults<ISearchResult>();

            if(keyWords != null)
            {
                var wikiService = new WikipediaService();
                wikiService.SearchParameters.Limit = 1;
                //wikiService.SearchParameters.TextType = TextType.HtmlPagePart;

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
            var sr = Search(searchString);
            // add search results in full results list without dublicates
            searchResults.Results.AddRange(
                sr.Results.Where(item => !searchResults.Results.Any(it => it.Link == item.Link)));
            // copy error if it is
            searchResults.ErrorMessage = sr.ErrorMessage;

            return searchResults;
        }

        public override SearchResults<ISearchResult> Search(string searchString)
        {
            // StartPage search
            var startPageService = new StartPageService();
            startPageService.SearchParameters.Limit = SearchParameters.Limit;
            startPageService.SearchParameters.LoadText = false;

            return Search(searchString, startPageService);
        }

        #endregion

        #region Private methods

        private SearchResults<TResult> Search<TParameters, TResult>(string searchString, ISearchService<TParameters, TResult> searchService)
            where TParameters : ISearchParameters
            where TResult : ISearchResult
        {
            try
            {
                // fill search parameters
                searchService.SearchParameters.Language = SearchParameters.Language;
                // search
                var searchResults = searchService.Search(searchString);
                // return search result
                return searchResults;
            }
            catch (Exception ex)
            {
                // write error if it is
                var searchResults = new SearchResults<TResult>();
                searchResults.ErrorMessage = ex.Message;
                return searchResults;
            }
        }

        #endregion
    }
}
