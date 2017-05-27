using BrainySearch.Data.Models;
using BrainySearch.Models;
using BrainySearch.Models.Lectures;
using BrainySearch.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BrainySearch.Data;

namespace BrainySearch.Controllers
{
    public class LecturesController : Controller
    {
        #region Private fields

        private readonly ILectureService lecturesService;
        private readonly IInitialInfoService initialInfoService;
        private readonly IKeyWordService keyWordService;

        #endregion

        #region Initialization

        public LecturesController()
        {
            var dbContext = ApplicationDbContext.Create();
            this.lecturesService = new LectureService(dbContext);
            this.initialInfoService = new InitialInfoService(dbContext);
            this.keyWordService = new KeyWordService(dbContext);
        }

        #endregion

        #region Public methods

        // GET: Lectures
        public ActionResult Index()
        {
            // load data from database
            var lectures = lecturesService.GetAll();

            // result view
            var lecturesListViewModel = new LecturesListViewModel();
            foreach(var lecture in lectures)
            {
                lecturesListViewModel.Lectures.Add(new LectureDetailsViewModel()
                {
                    LectureId = lecture.Id,
                    LectureTheme = lecture.Theme,
                    Text = lecture.Text
                });
            }
            return View(lecturesListViewModel);
        }

        [HttpGet]
        public ActionResult CreateLecture(int[] searchResultIds)
        {
            // get data
            SearchResultViewModel[] searchResults = Session[SharedData.SearchResultsKeyName] as SearchResultViewModel[];
            string lectureTheme = Session[SharedData.LectureThemeKeyName] as string;
            string[] keyWords = Session[SharedData.KeyWordsKeyName] as string[];
            Session[SharedData.IsNewLectureKeyName] = true;

            var lectureText = new StringBuilder();

            if (searchResults != null)
            {
                foreach (var sr in searchResults.Where(item => searchResultIds.Contains(item.Id)))
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
                Text = lectureText.ToString(),
                KeyWords = keyWords,
                Links = searchResults.Where(item => searchResultIds.Contains(item.Id)).Select(item => item.LinkInfo).ToArray()
            };

            return View("LectureEditing", lectureDetails);
        }

        [HttpGet]
        public ActionResult LectureEditing(Guid lectureId)
        {
            var lecture = lecturesService.GetById(lectureId);
            LectureDetailsViewModel model = new LectureDetailsViewModel()
            {
                LectureId = lectureId,
                LectureTheme = lecture.Theme,
                Text = lecture.Text,
                KeyWords = keyWordService.GetAllByLectureId(lectureId).Select(item => item.KeyWordText).ToArray(),
                Links = initialInfoService.GetAllByLectureId(lectureId).Select(item => new LinkInfo() { SourceLink = item.SourceLink }).ToArray()
            };

            Session[SharedData.IsNewLectureKeyName] = false;
            Session[SharedData.LectureIdInEditingKeyName] = lectureId;

            return View(model);
        }

        //[HttpGet]
        //public ActionResult LectureEditing()
        //{
        //    // get data
        //    SearchResultViewModel[] searchResults = Session[SharedData.SearchResultsKeyName] as SearchResultViewModel[];
        //    string lectureTheme = Session[SharedData.LectureThemeKeyName] as string;
        //    string[] keyWords = Session[SharedData.KeyWordsKeyName] as string[];

        //    var lectureText = new StringBuilder();

        //    if (searchResults != null)
        //    {
        //        foreach (var sr in searchResults)
        //        {
        //            if (lectureText.Length > 0)
        //                lectureText.AppendLine();
        //            lectureText.AppendLine(sr.Title);
        //            lectureText.AppendLine(sr.Text);
        //        }
        //    }

        //    // build view
        //    var lectureDetails = new LectureDetailsViewModel()
        //    {
        //        LectureTheme = lectureTheme,
        //        Text = lectureText.ToString(),
        //        KeyWords = keyWords
        //    };

        //    return View(lectureDetails);
        //}

        [HttpPost]
        public ActionResult SaveLecture(LectureDetailsViewModel model)
        {
            // update model (to be updated this implementation)
            bool isNew = (bool)Session[SharedData.IsNewLectureKeyName];
            if (isNew)
            {
                // get data to save
                model.LectureTheme = Session[SharedData.LectureThemeKeyName] as string;
                model.KeyWords = Session[SharedData.KeyWordsKeyName] as string[];
                SearchResultViewModel[] searchResults = Session[SharedData.SearchResultsKeyName] as SearchResultViewModel[];
                // clean data
                Session.Remove(SharedData.LectureThemeKeyName);
                Session.Remove(SharedData.KeyWordsKeyName);
                Session.Remove(SharedData.SearchResultsKeyName);
                // save lecture to database
                // TODO: save
                var lecture = new Lecture();
                lecture.Theme = model.LectureTheme;
                lecture.Text = model.Text;
                lecture.UserId = User.Identity.GetUserId();
                foreach (var res in searchResults)
                {
                    var initialInfo = new InitialInfo()
                    {
                        Title = res.Title,
                        SourceLink = res.LinkInfo.SourceLink,
                        Lecture = lecture,
                        Text = res.Text
                    };
                    lecture.InitialInfos.Add(initialInfo);
                }
                foreach(var kw in model.KeyWords)
                {
                    var keyWord = new KeyWord()
                    {
                        KeyWordText = kw
                    };
                    lecture.KeyWords.Add(keyWord);
                }
                lecturesService.Add(lecture);
                lecturesService.Save();
            }
            else
            {
                // get lecture id
                Guid lectureId = (Guid)Session[SharedData.LectureIdInEditingKeyName];
                // clean data
                Session.Remove(SharedData.LectureIdInEditingKeyName);
                // update lecture in database
                var lecture = lecturesService.GetById(lectureId);
                lecture.Text = model.Text;
                lecturesService.Save();
            }

            // clean data
            Session.Remove(SharedData.IsNewLectureKeyName);

            // redirect to list of lectures
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult LectureDetails(Guid lectureId)
        {
            var lecture = lecturesService.GetById(lectureId);
            LectureDetailsViewModel model = new LectureDetailsViewModel()
            {
                LectureId = lectureId,
                LectureTheme = lecture.Theme,
                Text = lecture.Text,
                KeyWords = keyWordService.GetAllByLectureId(lectureId).Select(item => item.KeyWordText).ToArray(),
                Links = initialInfoService.GetAllByLectureId(lectureId).Select(item => new LinkInfo() { SourceLink = item.SourceLink }).ToArray()
            };
            return View(model);
        }

        #endregion
    }
}