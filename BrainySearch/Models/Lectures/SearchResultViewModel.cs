﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrainySearch.Models.Lectures
{
    public class SearchResultViewModel
    {
        public string Text { get; set; }

        public string SourceLink { get; set; }

        public bool AddToLecture { get; set; }
    }
}