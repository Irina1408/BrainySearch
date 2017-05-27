using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrainySearch.Models.Lectures
{
    

    public class LectureDetailsViewModel
    {        
        public Guid LectureId { get; set; }

        public string LectureTheme { get; set; }

        public string[] KeyWords { get; set; }

        public LinkInfo[] Links { get; set; }

        public string Text { get; set; }

        public string ShortText { get { return Text.Length > 500 ? Text.Substring(0, 500) + "..." : Text; } }
    }
}