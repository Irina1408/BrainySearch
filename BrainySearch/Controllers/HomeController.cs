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
            // TODO: REMOVE only for tests
            var res = new List<SearchResultViewModel>();
            res.Add(new SearchResultViewModel { Text = "Test result 1", AddToLecture = true, SourceLink = "http://www.mysamplecode.com/2012/04/generate-html-table-using-javascript.html" });
            res.Add(new SearchResultViewModel { Text = "Test result 2", AddToLecture = true, SourceLink = "https://developer.mozilla.org/en-US/docs/Web/JavaScript?redirectlocale=en-US&redirectslug=JavaScript" });

            return Content(JsonConvert.SerializeObject(res));
        }
    }
}