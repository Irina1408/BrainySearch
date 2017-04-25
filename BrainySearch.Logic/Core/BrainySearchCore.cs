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
            brainySearchService.SearchParameters.Limit = 100;
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
            var searchResults = new SearchResults<BrainySearchResult>() { ErrorMessage = sr.ErrorMessage };
            searchResults.Results.AddRange(
                sr.Results.Select(item => new BrainySearchResult()
                {
                    Title = item.Title,
                    Link = item.Link
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
            // remove forums and youtube videos
            var linkToRemove = new[] { "forum", "youtube.com" };
            searchResults.Results.RemoveAll(item => linkToRemove.Any(l => item.Link.Contains(l)));

            // TODO: make it generic
            // remove links to file download
            var fileExtensions = new string[] { ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".ppt" };
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
                    string data = GetHtmlResult(r.Link);
                    if (string.IsNullOrEmpty(data)) continue;

                    // parse html page by wiki parser on unknown page parser
                    r.Text = r.Link.Contains("wikipedia.org") ? wikiParser.Parse(data) : articleParser.Parse(data);
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
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // get response
                    receiveStream = response.GetResponseStream();
                    receiveStream.CopyTo(mReceiveStream);
                    mReceiveStream.Position = 0;
                    // get encoding if correspond exists
                    Encoding enc = Encoding.GetEncodings()
                        .Where(item => item.Name == response.CharacterSet)?.FirstOrDefault()?.GetEncoding();
                    // get stream
                    readStream = enc == null ? new StreamReader(mReceiveStream) : new StreamReader(mReceiveStream, enc);
                    // read html
                    data = readStream.ReadToEnd();

                    // recheck encoding
                    if (data.Contains("charset="))
                    {
                        int encStartIndex = data.IndexOf("charset=") + "charset=".Length;
                        int encEndEncodingIndex = data.IndexOfAny(new[] { ' ', '\"', ';', '\'' }, encStartIndex);
                        string encoding = data.Substring(encStartIndex, encEndEncodingIndex - encStartIndex);
                        Encoding newEnc = Encoding.GetEncodings()
                            .Where(item => item.Name == encoding)?.FirstOrDefault()?.GetEncoding();
                        
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

        #endregion

        #endregion
    }
}
