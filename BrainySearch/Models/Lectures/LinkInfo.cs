using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrainySearch.Models.Lectures
{
    public class LinkInfo
    {
        public string SourceLink { get; set; }

        public string ShortLink
        {
            get
            {
                // create short link for view
                var shortLink = SourceLink.Length > 30 ? string.Format("{0}...", SourceLink.Substring(0, 30)) : SourceLink;

                if (shortLink.StartsWith("https://"))
                    shortLink = shortLink.Substring("https://".Length, shortLink.Length - "https://".Length);

                if (shortLink.StartsWith("http://"))
                    shortLink = shortLink.Substring("http://".Length, shortLink.Length - "http://".Length);

                return shortLink;
            }
        }
    }
}