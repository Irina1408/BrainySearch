using BrainySearch.Logic.Search.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Filters
{
    /// <summary>
    /// Filter search results of any searcher
    /// </summary>
    public class SearchResultsFilter
    {
        // forums and youtube videos
        private readonly string[] linkToRemove = new[] { "forum", "youtube.com" };
        private readonly string[] fileExtensions = new string[] { ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".ppt" };

        public void Filter(List<ISearchResult> searchResults)
        {
            // remove external links
            searchResults.RemoveAll(item => linkToRemove.Any(l => item.Link.Contains(l)));

            // remove links to file download
            foreach (var ext in fileExtensions)
                searchResults.RemoveAll(item => item.Link.EndsWith(ext));
        }
    }
}
