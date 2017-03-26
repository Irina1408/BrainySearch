using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BrainySearch.Models.Lectures
{
    public class SearchPropertiesViewModel
    {
        public SearchPropertiesViewModel()
        {
            SearchResultViewModels = new List<SearchResultViewModel>();
        }

        [Display(Name = "Lecture theme")]
        public string LectureTheme { get; set; }

        public List<SearchResultViewModel> SearchResultViewModels { get; set; }
    }
}