using BrainySearch.Logic.Search.Base;
using BrainySearch.Logic.Search.DuckDuckGo;
using BrainySearch.Logic.Search.Gigablast;
using BrainySearch.Logic.Search.StartPage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Search.BrainySearchS
{
    public class BrainySearchService : IBrainySearchService
    {
        public SearchResults Search(string searchString)
        {
            var searchResults = new SearchResults();

            // 1. Google search
            //if (Search(ref searchResults, searchString, new GoogleService())) return searchResults;

            // 2. Gigablast
            //if (Search(ref searchResults, searchString, new GigablastService())) return searchResults;

            // 3. DuckDuckGo
            //if (Search(ref searchResults, searchString, new DuckDuckGoService())) return searchResults;

            // 4. StartPage
            if (Search(ref searchResults, searchString, new StartPageService())) return searchResults;
            
            return searchResults;
        }

        private bool Search(ref SearchResults searchResults, string searchString, IWebSearchService searchService)
        {
            if (searchService == null) return false;

            try
            {
                searchService.LanguageCode = LangDetection.LangDetector.DetectLanguage(searchString);
                searchService.Language = CultureInfo.GetCultureInfo(searchService.LanguageCode).EnglishName;
                searchService.MaxPagesCount = 100;
                searchResults = searchService.Search(searchString);

                if (!searchResults.HasErrors)
                {
                    FixLinks(ref searchResults);
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

        private void FixLinks(ref SearchResults searchResults)
        {
            foreach(var sr in searchResults.Results)
            {
                if(!sr.Link.StartsWith("http"))
                {
                    sr.Link = string.Format("https://{0}", sr.Link);
                }
            }
        }
    }
}
