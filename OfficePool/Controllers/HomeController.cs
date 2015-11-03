using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficePool.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            // Always re-direct to the "Default.htm" page
            return new RedirectResult(Url.Content("~/Default.htm"));
            
        }
        public ActionResult About()
        {
            ViewBag.Message = "Office Pool Management tool.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "";

            return View();
        }

    }
}