using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrainySearch.Models.Lectures
{  
    public class LectureDetailsViewModel
    {        
        public LectureDetailsViewModel()
        {
            KeyWords = new List<string>();
            Links = new List<LinkInfo>();
        }

        public Guid LectureId { get; set; }

        public string LectureTheme { get; set; }

        public List<string> KeyWords { get; }

        public List<LinkInfo> Links { get; }

        public string Text { get; set; }

        public string ShortText { get { return Text.Length > 500 ? Text.Substring(0, 500) + "..." : Text; } }
        
        public string Html { get { return string.Format("<p>{0}</p>", Text.Replace("\n", "</p><p>")); } }

        public string ShortHtml { get { return string.Format("<p>{0}</p>", ShortText.Replace("\n", "</p><p>")); } }
    }
}