using BrainySearch.Models;
using BrainySearch.Models.Lectures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace BrainySearch.Controllers
{
    public class LecturesController : Controller
    {
        // GET: Lectures
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateLecture(int searchNumber, int[] searchResultIds)
        {
            // get data
            SearchResultViewModel[] searchResults = Session[SharedData.SearchResultsKeyName + searchNumber] as SearchResultViewModel[];
            string lectureTheme = Session[SharedData.LectureThemeKeyName + searchNumber] as string;
            string[] keyWords = Session[SharedData.KeyWordsKeyName + searchNumber] as string[];
            Session[SharedData.LectureNumInEditingKeyName] = searchNumber;
            Session[SharedData.IsNewLectureKeyName + searchNumber] = true;

            var lectureText = new StringBuilder();

            if (searchResults != null)
            {
                foreach (var sr in searchResults)
                {
                    if (lectureText.Length > 0)
                        lectureText.AppendLine();
                    lectureText.AppendLine(sr.Title);
                    lectureText.AppendLine(sr.Text);
                }
            }

            // build view
            var lectureDetails = new LectureDetailsViewModel()
            {
                LectureTheme = lectureTheme,
                Results = searchResults,
                Text = lectureText.ToString(),
                KeyWords = keyWords
            };

            return View("LectureEditing", lectureDetails);
        }

        [HttpGet]
        public ActionResult LectureEditing(bool isNew)
        {
            //string lectureTheme = TempData["lectureTheme"] as string;
            //string[] keyWords = TempData["keyWords"] as string[];
            //SearchResultViewModel[] searchResults = TempData["searchResults"] as SearchResultViewModel[];

            // get data
            int searchNumber = (int)TempData["searchNumber"];
            SearchResultViewModel[] searchResults = Session[SharedData.SearchResultsKeyName + searchNumber] as SearchResultViewModel[];
            string lectureTheme = Session[SharedData.LectureThemeKeyName + searchNumber] as string;
            string[] keyWords = Session[SharedData.KeyWordsKeyName + searchNumber] as string[];
            Session[SharedData.LectureNumInEditingKeyName] = searchNumber;
            Session[SharedData.IsNewLectureKeyName + searchNumber] = isNew;

            var lectureText = new StringBuilder();

            if (searchResults != null)
            {
                foreach (var sr in searchResults)
                {
                    if (lectureText.Length > 0)
                        lectureText.AppendLine();
                    lectureText.AppendLine(sr.Title);
                    lectureText.AppendLine(sr.Text);
                }
            }

            // build view
            var lectureDetails = new LectureDetailsViewModel()
            {
                LectureTheme = lectureTheme,
                Results = searchResults,
                Text = lectureText.ToString(),
                KeyWords = keyWords
            };

            return View(lectureDetails);
        }

        [HttpPost]
        public ActionResult SaveLecture(LectureDetailsViewModel model)
        {
            // update model (to be updated this implementation)
            int searchNumber = (int)Session[SharedData.LectureNumInEditingKeyName];
            bool isNew = (bool)Session[SharedData.IsNewLectureKeyName + searchNumber];
            if (isNew)
            {
                // get data to save
                model.LectureTheme = Session[SharedData.LectureThemeKeyName + searchNumber] as string;
                model.KeyWords = Session[SharedData.KeyWordsKeyName + searchNumber] as string[];
                model.Results = Session[SharedData.SearchResultsKeyName + searchNumber] as SearchResultViewModel[];
                // clean data
                Session.Remove(SharedData.LectureThemeKeyName + searchNumber);
                Session.Remove(SharedData.KeyWordsKeyName + searchNumber);
                Session.Remove(SharedData.IsNewLectureKeyName + searchNumber);
                // save lecture to database
                // TODO: save
            }
            else
            {
                // get lecture id
                int lectureId = (int)Session[SharedData.LectureIdInEditingKeyName + searchNumber];
                // clean data
                Session.Remove(SharedData.LectureIdInEditingKeyName + searchNumber);
                // update lecture in database
                // TODO: save
            }

            // clean data
            Session.Remove(SharedData.LectureNumInEditingKeyName);
            Session.Remove(SharedData.SearchResultsKeyName + searchNumber);
            
            // redirect to list of lectures
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult LectureDetails()
        {
            LectureDetailsViewModel model = TempData[nameof(LectureDetailsViewModel)] as LectureDetailsViewModel;
            return View(model);
        }
    }
}