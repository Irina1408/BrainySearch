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
using System.Threading;
using System.Web;

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
            brainySearchService.SearchParameters.Limit = -1;
        }

        #endregion

        #region Public methods

        public SearchResults<BrainySearchResult> BrainySearch(string searchString, string[] keyWords)
        {
            if (string.IsNullOrEmpty(searchString)) return new SearchResults<BrainySearchResult>();
            try
            {
                // 1. Search
                var searchResults = Search(searchString, keyWords);
                if (searchResults.HasErrors || searchResults.Results.Count == 0) return BuildBrainySearchResult(searchResults);
                // 2. Parse html and get clear text
                ParsePageHtml(searchResults.Results);
                // 3. Apply all text filters
                FilterResultsByContent(searchResults.Results, keyWords);
                // 4. Apply analysis filters and update search result sequence
                if (keyWords != null) brainySearchAnalyser.KeyWords.AddRange(keyWords);
                var analysisResult = brainySearchAnalyser.Analyse(searchResults.Results);
                UpdateAnalysisResults(analysisResult);
                // 5. Build full result
                return BuildBrainySearchResult(analysisResult);
            }
            catch(Exception ex)
            {
                // write error if it is
                var searchResults = new SearchResults<BrainySearchResult>();
                searchResults.ErrorMessage = ex.Message;
                return searchResults;
            }
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

                if(string.IsNullOrEmpty(sr.Title))
                    sr.Title = HttpUtility.HtmlDecode(sr.Title);
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
            // create tasks for requests
            var tasks = new List<Task>();

            foreach (var r in searchResults)
            {
                var sr = r;
                tasks.Add(Task.Run(() => {
                    try
                    {
                        string data = GetHtmlResult(sr.Link);
                        if (string.IsNullOrEmpty(data)) return;

                        // parse html page by wiki parser on unknown page parser
                        sr.Text = HttpUtility.HtmlDecode(sr.Link.Contains("wikipedia.org") ? wikiParser.Parse(data) : articleParser.Parse(data));
                    }
                    catch (Exception)
                    { }
                }));              
            }

            // wait while all tasks are ended
            Task.WaitAll(tasks.ToArray(), CancellationToken.None);
            // dispose all tasks
            foreach (var t in tasks)
                t.Dispose();
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
        /// Updates analysis results
        /// </summary>
        private void UpdateAnalysisResults(List<AnalysisResult> searchResults)
        {
            foreach(var sr in searchResults.Where(item => item.SearchResult.Link.Contains("wikipedia.org")))
            {
                sr.IsSuitable = true;
                sr.Index = -1;
            }
        }

        /// <summary>
        /// Returns brainy search result
        /// </summary>
        private SearchResults<BrainySearchResult> BuildBrainySearchResult(List<AnalysisResult> searchResults)
        {
            // init brainy search result
            var sr = new SearchResults<BrainySearchResult>();
            sr.Results.AddRange(searchResults.Where(item => item.IsSuitable).Select(item => new BrainySearchResult()
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
                while (r.Text.Contains("\r")) r.Text = r.Text.Replace("\r", " ");
                while (r.Text.Contains("  ")) r.Text = r.Text.Replace("  ", " ");
                while (r.Text.Contains("\n \n")) r.Text = r.Text.Replace("\n \n", "\n");
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

            // 1. Remove not persed pages
            foreach (var r in searchResults.Where(item => string.IsNullOrEmpty(item.Text)))
                resultsToRemove.Add(r);

            // 2. Apply key words filter
            // set key words
            keyWordsFilter.KeyWords.Clear();
            if (keyWords != null) keyWordsFilter.KeyWords.AddRange(keyWords);
            keyWordsFilter.NormalizeKeyWords();

            // key: search result
            // value: key words count that search result contains
            var searchResultKeyWords = new Dictionary<ISearchResult, int>();
            // tasks for getting key words count
            var tasks = new List<Task>();

            // get key words for every result
            foreach (var r in searchResults.Where(item => !resultsToRemove.Contains(item) && !item.Link.Contains("wikipedia.org")))
            {
                var tmp = r;
                tasks.Add(Task.Run(() => searchResultKeyWords.Add(tmp, keyWordsFilter.GetContainedKeyWordsCount(tmp.Text))));
            }

            // wait all tasks are ended
            Task.WaitAll(tasks.ToArray(), CancellationToken.None);
            // dispose all tasks
            foreach (var t in tasks)
                t.Dispose();

            // get max counted key words in results
            int maxKeyWordsCount = searchResultKeyWords.Max(item => item.Value);

            // remove results that do not contain all key words
            foreach(var r in searchResultKeyWords.Where(item => item.Value != maxKeyWordsCount))
                resultsToRemove.Add(r.Key);

            // 3. Remove found external results
            foreach (var r in resultsToRemove)
            {
                if (searchResults.Contains(r))
                    searchResults.Remove(r);
            }
        }

        #endregion
    }
}
