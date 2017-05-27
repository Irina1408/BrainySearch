using BrainySearch.Logic.Core;
using BrainySearch.Logic.Search.Base;
using BrainySearch.Models;
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
using System.Web.Routing;

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
            // local variables
            var res = new SearchResultsViewModel();
            var searchService = new BrainySearchCore();
            //var searchResult = searchService.BrainySearch(lectureTheme, keyWords);

            // --- TESTS
            var searchResult = new SearchResults<BrainySearchResult>();
            searchResult.Results.Add(new BrainySearchResult()
            {
                Link = "https://vk.com/feed",
                Html = "Some html",
                Title = "VK",
                Text = "Some interesting text."
            });
            for (int i = 0; i < 100; i++)
            {
                searchResult.Results[0].Html += " My so long text.";
                searchResult.Results[0].Text += " My so long text.";
            }
            // --- TESTS

            if (!searchResult.HasErrors)
            {
                var index = 1;

                foreach (var sr in searchResult.Results.OrderBy(item => item.Index))
                {
                    res.Results.Add(new SearchResultViewModel
                    {
                        Id = index++,
                        Title = sr.Title,
                        Text = sr.Text,
                        Html = sr.Html,
                        LinkInfo = new LinkInfo() { SourceLink = sr.Link }
                    });
                }
            }
            else
                res.ErrorMessage = searchResult.ErrorMessage;
            
            // keep found results
            Session[SharedData.SearchResultsKeyName] = res.Results.ToArray();
            Session[SharedData.LectureThemeKeyName] = lectureTheme;
            Session[SharedData.KeyWordsKeyName] = keyWords;

            return Content(JsonConvert.SerializeObject(res));
        }
    }
}