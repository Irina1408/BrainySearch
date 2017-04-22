using BrainySearch.Logic.Parser;
using BrainySearch.Logic.Search.Base;
using BrainySearch.Logic.Search.BrainySearchS;
using System;
using System.Collections.Generic;
using System.IO;
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
            // 1. Get text
            ParsePageHtml(searchResults);
            // 2. Build html
            BuildResultHtml(searchResults);
        }

        /// <summary>
        /// Fill result text by page parsing
        /// </summary>
        private void ParsePageHtml(SearchResults<BrainySearchResult> searchResults)
        {
            // init parsers
            var wikiParser = new WikipediaParser();
            var articleParser = new ArticleParser();
            
            foreach (var r in searchResults.Results)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(r.Link);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode != HttpStatusCode.OK)
                        continue;

                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    }

                    string data = readStream.ReadToEnd();

                    response.Close();
                    readStream.Close();
                    
                    // wikipedia page
                    if (r.Link.Contains("wikipedia.org"))
                    {
                        // get text of detinition from wikipedia
                        r.Text = wikiParser.Parse(data);
                    }
                    else
                    {
                        // parse unknown page html
                        r.Text = articleParser.Parse(data);
                    }
                }
                catch (WebException ex)
                {
                    // TODO: remove
                    r.Text = "Web exception: " + ex.Message;
                }
                catch (Exception ex)
                {
                    // do nothing
                    // TODO: remove
                    r.Text = "Error html parsing: " + ex.Message;
                }
            }  
        }

        private void BuildResultHtml(SearchResults<BrainySearchResult> searchResults)
        {
            foreach (var r in searchResults.Results.Where(item => !string.IsNullOrEmpty(item.Text)))
            {
                r.Html = string.Format("<p>{0}</p>", r.Text.Replace("\n", "</p><p>"));
            }
        }

        #endregion

        #endregion
    }
}
