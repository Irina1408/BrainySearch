using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrainySearch.Models.Lectures
{
    public class LecturesListViewModel
    {
        public LecturesListViewModel()
        {
            Lectures = new List<LectureDetailsViewModel>();
        }

        public List<LectureDetailsViewModel> Lectures { get; set; }
    }
}