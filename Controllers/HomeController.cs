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
            if (TempData["Messages"] != null)
            {
                ViewBag.Code = (string)TempData["Code"];
                ViewBag.Head = (string)TempData["Head"];
                ViewBag.Message = String.Join("\n", ((List<string>)TempData["Messages"]).ToArray());
            }

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
        public ActionResult Forbidden(StatusCode code, string returnUrl)
        {
            ParseResults(code);
            return RedirectToAction("Index");
        }

        private void ParseResults(StatusCode status)
        {
            switch (status)
            {
                case StatusCode.PermissionUnauthorized:
                    TempData["Code"] = "danger";
                    TempData["Head"] = "Unauthorized";
                    TempData["Messages"] = new List<string>() { "You do not have permissions to access this page" };
                    break;

                case StatusCode.RoleUnauthorized:
                    TempData["Code"] = "danger";
                    TempData["Head"] = "Unauthorized";
                    TempData["Messages"] = new List<string>() { "Your Role is not authorized to acces this page" };
                    break;

                default:
                    TempData["Code"] = "warning";
                    TempData["Head"] = "Info";
                    TempData["Messages"] = new List<string>() { "Unknown error eccured." };
                    break;
            }
        }
    }
}