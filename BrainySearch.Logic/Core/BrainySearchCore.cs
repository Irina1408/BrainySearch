using BrainySearch.Logic.Filters;
using BrainySearch.Logic.Parser;
using BrainySearch.Logic.Search.Base;
using BrainySearch.Logic.Search.BrainySearchS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using BrainySearch.Logic.Analysis;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Core
{
    public class BrainySearchCore
    {
        #region Private fields
        
        private IBrainySearchService brainySearchService;
        private SearchResultsFilter searchResultsFilter;
        private KeyWordsFilter keyWordsFilter;
        private BrainySearchAnalyser brainySearchAnalyser;

        #endregion

        #region Initialization

        public BrainySearchCore()
        {
            brainySearchService = new BrainySearchService();
            searchResultsFilter = new SearchResultsFilter();
            keyWordsFilter = new KeyWordsFilter();
            brainySearchAnalyser = new BrainySearchAnalyser();
            // TODO: should be unlimited
            brainySearchService.SearchParameters.Limit = 80;
        }

        #endregion

        #region Public methods

        public SearchResults<BrainySearchResult> BrainySearch(string searchString, string[] keyWords)
        {
            // 1. Search
            var searchResults = Search(searchString, keyWords);
            if (searchResults.HasErrors || searchResults.Results.Count == 0) return BuildBrainySearchResult(searchResults);
            // 2. Parse html and get clear text
            ParsePageHtml(searchResults.Results);
            // 3. Apply all text filters
            FilterResultsByContent(searchResults.Results, keyWords);
            // 4. Apply analysis filters and update search result sequence
            var analysisResult = brainySearchAnalyser.Analyse(searchResults.Results);

            // 5. Build full result
            return BuildBrainySearchResult(analysisResult);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Search by text and key words list of correspond links
        /// </summary>
        private SearchResults<ISearchResult> Search(string searchString, string[] keyWords)
        {
            // 1. Detect search string language
            brainySearchService.SearchParameters.Language = TextProcessing.LangDetector.Detect(searchString);
            // 2. Search links by search string and key words
            var searchResults = brainySearchService.Search(searchString, keyWords);
            // 3. Filter search results (by links)
            searchResultsFilter.Filter(searchResults.Results);
            // 4. Fix result links
            FixLinks(searchResults.Results);
            // 5. Clean found text if it exists
            foreach (var r in searchResults.Results.Where(item => !string.IsNullOrEmpty(item.Text)))
                r.Text = null;

            return searchResults;
        }

        /// <summary>
        /// Fix links for correct showing and opening
        /// </summary>
        private void FixLinks(List<ISearchResult> searchResults)
        {
            foreach (var sr in searchResults.Where(item => item.Link != null))
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
        /// Fill result text by page parsing
        /// </summary>
        private void ParsePageHtml(List<ISearchResult> searchResults)
        {
            // init parsers
            var wikiParser = new WikipediaParser();
            var articleParser = new ArticleParser();

            foreach (var r in searchResults)
            {
                try
                {
                    string data = GetHtmlResult(r.Link);
                    if (string.IsNullOrEmpty(data)) continue;

                    // parse html page by wiki parser on unknown page parser
                    r.Text = r.Link.Contains("wikipedia.org") ? wikiParser.Parse(data) : articleParser.Parse(data);
                }
                catch (Exception)
                { }
            }
        }

        /// <summary>
        /// Returns html page with correct encoding
        /// </summary>
        private string GetHtmlResult(string link)
        {
            // result
            string data = null;
            // variables to dispose
            HttpWebResponse response = null;
            Stream receiveStream = null;
            MemoryStream mReceiveStream = new MemoryStream();
            StreamReader readStream = null;

            try
            {
                // get data
                response = (HttpWebResponse)((HttpWebRequest)WebRequest.Create(link)).GetResponse();

                // check response status and result type
                if (response.StatusCode == HttpStatusCode.OK && response.ContentType.Contains("text/html"))
                {
                    // get response
                    receiveStream = response.GetResponseStream();
                    receiveStream.CopyTo(mReceiveStream);
                    mReceiveStream.Position = 0;
                    // get encoding if correspond exists
                    Encoding enc = Encoding.GetEncodings()
                        .Where(item => item.Name == response.CharacterSet)?.FirstOrDefault()?.GetEncoding();
                    if (enc == null && response.CharacterSet.Contains("1251")) enc = Encoding.GetEncoding(1251);
                    // get stream
                    readStream = enc == null ? new StreamReader(mReceiveStream) : new StreamReader(mReceiveStream, enc);
                    // read html
                    data = readStream.ReadToEnd();

                    // recheck encoding
                    if (data.Contains("charset="))
                    {
                        // get encoding start index and end index in the html page
                        int encStartIndex = data.IndexOf("charset=") + "charset=".Length;
                        int encEndEncodingIndex = data.IndexOfAny(new[] { ' ', '\"', ';', '\'' }, encStartIndex);
                        // get encoding name
                        string encoding = data.Substring(encStartIndex, encEndEncodingIndex - encStartIndex);
                        // get encoding object
                        Encoding newEnc = Encoding.GetEncodings()
                            .Where(item => item.Name == encoding)?.FirstOrDefault()?.GetEncoding();
                        if (newEnc == null && encoding.Contains("1251")) newEnc = Encoding.GetEncoding(1251);

                        // check encoding is not the same
                        if (newEnc != null && (enc == null || !enc.Equals(newEnc)))
                        {
                            // read html with new encoding
                            mReceiveStream.Position = 0;
                            var sr = new StreamReader(mReceiveStream, newEnc);
                            data = sr.ReadToEnd();
                            sr.Close(); sr.Dispose();
                        }
                    }
                }
            }
            finally
            {
                // clean up
                if (response != null) { response.Close(); response.Dispose(); }
                if (receiveStream != null) { receiveStream.Close(); receiveStream.Dispose(); }
                if (mReceiveStream != null) { mReceiveStream.Close(); mReceiveStream.Dispose(); }
                if (readStream != null) { readStream.Close(); readStream.Dispose(); }
            }

            return data;
        }

        /// <summary>
        /// Returns brainy search result
        /// </summary>
        private SearchResults<BrainySearchResult> BuildBrainySearchResult(SearchResults<ISearchResult> searchResults)
        {
            // init brainy search result
            var sr = new SearchResults<BrainySearchResult>() { ErrorMessage = searchResults.ErrorMessage };
            sr.Results.AddRange(searchResults.Results.Select(item => new BrainySearchResult()
                { Title = item.Title, Link = item.Link, Text = item.Text }));
            // build html for correct text showing
            BuildResultHtml(sr);

            return sr;
        }

        /// <summary>
        /// Returns brainy search result
        /// </summary>
        private SearchResults<BrainySearchResult> BuildBrainySearchResult(List<AnalysisResult> searchResults)
        {
            // init brainy search result
            var sr = new SearchResults<BrainySearchResult>();
            sr.Results.AddRange(searchResults.Select(item => new BrainySearchResult()
            {
                Title = item.SearchResult.Title,
                Link = item.SearchResult.Link,
                Text = item.SearchResult.Text,
                Index = item.Index
            }));
            // build html for correct text showing
            BuildResultHtml(sr);

            return sr;
        }

        /// <summary>
        /// Build html from simple text in search results for correct showing
        /// </summary>
        private void BuildResultHtml(SearchResults<BrainySearchResult> searchResults)
        {
            foreach (var r in searchResults.Results.Where(item => !string.IsNullOrEmpty(item.Text)))
            {
                while (r.Text.Contains("\n\n")) r.Text = r.Text.Replace("\n\n", "\n");
                r.Html = string.Format("<p>{0}</p>", r.Text.Replace("\n", "</p><p>"));
            }
        }

        /// <summary>
        /// Filters search results by text
        /// </summary>
        private void FilterResultsByContent(List<ISearchResult> searchResults, string[] keyWords)
        {
            var resultsToRemove = new List<ISearchResult>();

            // remove not persed pages
            foreach (var r in searchResults.Where(item => string.IsNullOrEmpty(item.Text)))
                resultsToRemove.Add(r);

            // apply key words filter
            keyWordsFilter.KeyWords = keyWords?.ToList();
            foreach (var r in searchResults.Where(item => !resultsToRemove.Contains(item)))
            {
                if (!keyWordsFilter.IsSuitableText(r.Text))
                    resultsToRemove.Add(r);
            }

            // remove external results
            foreach(var r in resultsToRemove)
            {
                if (searchResults.Contains(r))
                    searchResults.Remove(r);
            }
        }

        #endregion
    }
}
