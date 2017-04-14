using BrainySearch.Logic.Parser;
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
            // TODO: should be unlimited
            brainySearchService.SearchParameters.Limit = 10;
        }

        #endregion

        #region Public methods

        public SearchResults<BrainySearchResult> BrainySearch(string searchString, string[] keyWords)
        {
            // 1. Search
            var searchResults = Search(searchString, keyWords);
            if (searchResults.HasErrors || searchResults.Results.Count == 0) return searchResults;
            // 2. Parse html and get clear text
            Parse(searchResults);
            if (searchResults.HasErrors) return searchResults;

            return searchResults;
        }

        #endregion

        #region Private methods

        #region Search

        private SearchResults<BrainySearchResult> Search(string searchString, string[] keyWords)
        {
            // 1. Detect search string language
            DetectLanguage(searchString);
            // 2. Search links by search string
            var sr = brainySearchService.Search(searchString, keyWords);
            // 3. Create SearchResults<BrainySearchResult>
            var searchResults = new SearchResults<BrainySearchResult>();
            searchResults.ErrorMessage = sr.ErrorMessage;
            searchResults.Results.AddRange(
                sr.Results.Select(item => new BrainySearchResult()
                {
                    Title = item.Title,
                    Link = item.Link,
                    Text = item.Text,    // TODO: remove this line,
                    Html = item.Text    // TODO: remove this line
                }));
            // 4. Remove extraneous links
            RemoveExtraneousLinks(searchResults);
            // 5. Fix result links
            FixLinks(searchResults);
            
            return searchResults;
        }

        /// <summary>
        /// Detect language by text
        /// </summary>
        /// <param name="text"></param>
        private void DetectLanguage(string text)
        {
            brainySearchService.SearchParameters.Language = LangDetection.LangDetector.Detect(text);
        }

        /// <summary>
        /// Fix links for correct showing and link
        /// </summary>
        /// <param name="searchResults"></param>
        private void FixLinks(SearchResults<BrainySearchResult> searchResults)
        {
            foreach (var sr in searchResults.Results.Where(item => item.Link != null))
            {
                // add https in page link if it does not exist
                if (!sr.Link.StartsWith("http"))
                    sr.Link = string.Format("https://{0}", sr.Link);

                // remove last symbol "/"
                if (sr.Link.EndsWith("/"))
                    sr.Link = sr.Link.Substring(0, sr.Link.Length - 1);
            }
        }

        /// <summary>
        /// Remove extraneous links like forums etc
        /// </summary>
        private void RemoveExtraneousLinks(SearchResults<BrainySearchResult> searchResults)
        {
            // remove forums
            searchResults.Results.RemoveAll(item => item.Link.Contains("forum"));

            // TODO: make it generic
            // remove links to file download
            var fileExtensions = new string[] { ".doc", ".docx", ".xls", ".xlsx", ".pdf" };
            foreach(var ext in fileExtensions)
                searchResults.Results.RemoveAll(item => item.Link.EndsWith(ext));
        }

        #endregion

        #region Parsing

        private void Parse(SearchResults<BrainySearchResult> searchResults)
        {
            // 1. Get html
            ParsePageHtml(searchResults);
            // 2. Fix results
            FixParseResult(searchResults);
        }

        /// <summary>
        /// Fill result text by page parsing
        /// </summary>
        private void ParsePageHtml(SearchResults<BrainySearchResult> searchResults)
        {
            // init parsers
            var wikiParser = new WikipediaParser();
            var articleParser = new ArticleParser();

            using (var webClient = new WebClient())
            {
                // prepare web client
                webClient.Encoding = System.Text.Encoding.UTF8;

                foreach (var r in searchResults.Results)
                {
                    try
                    {
                        var pageHtml = webClient.DownloadString(r.Link);
                        // wikipedia page
                        if (r.Link.Contains("wikipedia.org"))
                        {
                            // get html of detinition from wikipedia
                            r.Html = wikiParser.Parse(pageHtml);
                        }
                        else
                        {
                            // parse unknown page html
                            r.Html = articleParser.Parse(pageHtml);
                        }
                    }
                    catch (WebException ex)
                    {
                        // TODO: remove
                        r.Html = "Web exception: " + ex.Message;
                    }
                    catch (Exception ex)
                    {
                        // do nothing
                        // TODO: remove
                        r.Html = "Error html parsing: " + ex.Message;
                    }                    
                }
            }   
        }

        private void FixParseResult(SearchResults<BrainySearchResult> searchResults)
        {
            // fix links in html
            foreach (var r in searchResults.Results.Where(item => !string.IsNullOrEmpty(item.Html) && item.Html.Contains("href=\"/")))
            {
                // get site domain
                var domain = r.Link.Substring(0, r.Link.IndexOf("/") + 1);

                // fix links in html descriptions
                r.Html = r.Html
                    .Replace("href=\"/", string.Format("href=\"{0}/", domain));
            }
        }

        #endregion

        #endregion
    }
}
