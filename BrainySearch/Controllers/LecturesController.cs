using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}