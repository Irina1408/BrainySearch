using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrainySearch.Models.Lectures
{
    public class SearchResultViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Html { get { return string.Format("<p>{0}</p>", Text.Replace("\n", "</p><p>")); } }

        public string Text { get; set; }

        public LinkInfo LinkInfo { get; set; }
    }
}