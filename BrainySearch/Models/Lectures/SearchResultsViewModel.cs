using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrainySearch.Models.Lectures
{
    public class SearchResultsViewModel
    {
        public SearchResultsViewModel()
        {
            Results = new List<SearchResultViewModel>();
        }

        /// <summary>
        /// Search results list
        /// </summary>
        public List<SearchResultViewModel> Results { get; }

        /// <summary>
        /// Error message if search was failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Returns true if search was failed
        /// </summary>
        public bool HasErrors { get { return !string.IsNullOrEmpty(ErrorMessage); } }
    }
}