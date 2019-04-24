using CascBasic.Classes;
using CascBasic.Classes.API.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CascBasic.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Lookup(string CRSid)
        {
            ViewBag.Message = "Your contact page.";

            PersonLookup pl = new PersonLookup();

            pl.GetPerson(CRSid, "email");
            return View();
        }

        [AllowAnonymous]
        public ActionResult Forbidden()
        {
            return View();
        }
    }
}