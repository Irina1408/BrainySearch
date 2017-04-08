using BrainySearch.Logic.Search;
using BrainySearch.Models.Lectures;
using Microsoft.Web.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BrainySearch.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "It is a good decision for resolving your problems. Brainy search can halp you to find only necessary information.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult Search(string lectureTheme, string[] keyWords)
        {
            var res = new List<SearchResultViewModel>();
            var searchService = new BrainySearchService();
            var searchResult = searchService.Search(lectureTheme);

            if(!searchResult.HasErrors)
            {
                foreach (var sr in searchResult.Results)
                {
                    res.Add(new SearchResultViewModel
                    {
                        Text = string.Format("{0}/n{1}", sr.Title, sr.Description),
                        AddToLecture = true,
                        SourceLink = sr.Link.EndsWith("/") ? sr.Link.Substring(0, sr.Link.Length - 1) : sr.Link,
                        ShortLink = sr.Link.Length > 30 ? string.Format("{0}...", sr.Link.Substring(0, 30)) : sr.Link
                    });
                }
            }
            // for tests
            else
            {
                res.Add(new SearchResultViewModel { Text = searchResult.ErrorMessage });
            }          

            return Content(JsonConvert.SerializeObject(res));
        }
    }
}