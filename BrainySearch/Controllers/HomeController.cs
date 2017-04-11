using BrainySearch.Logic.Core;
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
            var searchService = new BrainySearchCore();
            var searchResult = searchService.BrainySearch(lectureTheme, keyWords);

            if(!searchResult.HasErrors)
            {
                foreach (var sr in searchResult.Results)
                {
                    // create short link for view
                    var shortLink = sr.Link.Length > 30 ? string.Format("{0}...", sr.Link.Substring(0, 30)) : sr.Link;

                    if (shortLink.StartsWith("https://"))
                        shortLink = shortLink.Substring("https://".Length, shortLink.Length - "https://".Length);

                    if (shortLink.StartsWith("http://"))
                        shortLink = shortLink.Substring("http://".Length, shortLink.Length - "http://".Length);

                    res.Add(new SearchResultViewModel
                    {
                        Title = sr.Title,
                        Text = sr.Description,
                        AddToLecture = true,
                        SourceLink = sr.Link,
                        ShortLink = shortLink
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