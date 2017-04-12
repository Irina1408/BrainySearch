﻿using BrainySearch.Logic.Parser;
using BrainySearch.Logic.Search.Base;
using BrainySearch.Logic.Search.BrainySearchS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Core
{
    public class BrainySearchCore
    {
        #region Private fields

        private IBrainySearchService brainySearchService;

        #endregion

        #region Initialization

        public BrainySearchCore()
        {
            brainySearchService = new BrainySearchService();
            brainySearchService.SearchParameters.Limit = 100;
        }

        #endregion

        #region Public methods

        public SearchResults<ISearchResult> BrainySearch(string searchString, string[] keyWords)
        {
            // detect search string language
            DetectLanguage(searchString);
            // get search result
            var searchResults = brainySearchService.Search(searchString, keyWords);
            // fix result links
            FixLinks(searchResults);
            // fill full descriptions
            FillDescriptions(searchResults);

            return searchResults;
        }

        #endregion

        #region Private methods

        private void DetectLanguage(string text)
        {
            brainySearchService.SearchParameters.Language = LangDetection.LangDetector.Detect(text);
            //CultureInfo.GetCultureInfo(LanguageCode).EnglishName;
        }

        private void FixLinks(SearchResults<ISearchResult> searchResults)
        {
            foreach (var sr in searchResults.Results)
            {
                if (sr.Link != null)
                {
                    if (!sr.Link.StartsWith("http"))
                        sr.Link = string.Format("https://{0}", sr.Link);

                    if (sr.Link.EndsWith("/"))
                        sr.Link = sr.Link.Substring(0, sr.Link.Length - 1);
                }
            }
        }

        private void FillDescriptions(SearchResults<ISearchResult> searchResults)
        {
            // init parsers
            var wikiParser = new WikipediaParser();
            using (var webClient = new WebClient())
            {
                // prepare web client
                webClient.Encoding = System.Text.Encoding.UTF8;

                //foreach (var r in searchResults.Results.Where(item => item.Description == null))
                // TODO: to be removed. only for tests. previous row uncomment
                foreach (var r in searchResults.Results.Where(item => item.Link.Contains("wikipedia.org")))
                {
                    // wikipedia page
                    if (r.Link.Contains("wikipedia.org"))
                    {
                        r.Description = wikiParser.GetDefinitionHtml(webClient.DownloadString(r.Link));
                    }
                }
            }
        }

        #endregion
    }
}
