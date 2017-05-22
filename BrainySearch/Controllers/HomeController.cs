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
        private static int SearchCount = 0;

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
            // update search count
            SearchCount++;
            // local variables
            var res = new SearchResultsViewModel() { SearchNumber = SearchCount };
            var searchService = new BrainySearchCore();
            var searchResult = searchService.BrainySearch(lectureTheme, keyWords);

            // --- TESTS
            //var searchResult = new SearchResults<BrainySearchResult>();
            //searchResult.Results.Add(new BrainySearchResult()
            //{
            //    Link = "https://vk.com/feed",
            //    Html = "Some html",
            //    Title = "VK",
            //    Text = "Some interesting text."
            //});
            //for (int i = 0; i < 100; i++)
            //{
            //    searchResult.Results[0].Html += " My so long text.";
            //    searchResult.Results[0].Text += " My so long text.";
            //}
            // --- TESTS

            if (!searchResult.HasErrors)
            {
                var index = 1;

                foreach (var sr in searchResult.Results.OrderBy(item => item.Index))
                {
                    // create short link for view
                    var shortLink = sr.Link.Length > 30 ? string.Format("{0}...", sr.Link.Substring(0, 30)) : sr.Link;

                    if (shortLink.StartsWith("https://"))
                        shortLink = shortLink.Substring("https://".Length, shortLink.Length - "https://".Length);

                    if (shortLink.StartsWith("http://"))
                        shortLink = shortLink.Substring("http://".Length, shortLink.Length - "http://".Length);

                    res.Results.Add(new SearchResultViewModel
                    {
                        Id = index++,
                        Title = sr.Title,
                        Text = sr.Text,
                        Html = sr.Html,
                        SourceLink = sr.Link,
                        ShortLink = shortLink
                    });
                }
            }
            else
                res.ErrorMessage = searchResult.ErrorMessage;
            
            // keep found results
            Session[SharedData.SearchResultsKeyName + res.SearchNumber] = res.Results.ToArray();
            Session[SharedData.LectureThemeKeyName + res.SearchNumber] = lectureTheme;
            Session[SharedData.KeyWordsKeyName + res.SearchNumber] = keyWords;

            return Content(JsonConvert.SerializeObject(res));
        }

        //[HttpGet]
        //public ActionResult CreateLecture(int searchNumber, int[] searchResultIds)
        //{
        //    // get data
        //    //SearchResultViewModel[] searchResults = Session[SharedData.SearchResultsKeyName + searchNumber] as SearchResultViewModel[];
        //    //string lectureTheme = Session[SharedData.LectureThemeKeyName + searchNumber] as string;
        //    //string[] keyWords = Session[SharedData.KeyWordsKeyName + searchNumber] as string[];

        //    // clean data
        //    //Session.Remove(SearchResultsKeyName + searchNumber);
        //    //Session.Remove(LectureThemeKeyName + searchNumber);
        //    //Session.Remove(KeyWordsKeyName + searchNumber);

        //    //TempData["lectureTheme"] = lectureTheme;
        //    //TempData["keyWords"] = keyWords;
        //    //TempData["searchResults"] = searchResults.Where(item => searchResultIds.Any(it => it == item.Id)).ToArray();
        //    TempData["searchNumber"] = searchNumber;

        //    return RedirectToAction("LectureEditing", "Lectures", true);
        //}
    }
}