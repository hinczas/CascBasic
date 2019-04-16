using CascBasic.Classes;
using CascBasic.Context;
using CascBasic.Models;
using CascBasic.Models.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CascBasic.Controllers
{
    
    public class DashboardController : Controller
    {
        ApplicationDbContext _db;

        public DashboardController()
        {
            _db = new ApplicationDbContext();
        }

        public ActionResult Index(string sub)
        {
            string cod = "";
            string hed = "";
            string msg = "";
            if (TempData["Messages"] != null)
            {
                cod = (string)TempData["Code"];
                hed = (string)TempData["Head"];
                msg = String.Join("\n", ((List<string>)TempData["Messages"]).ToArray());
            }

            sub = string.IsNullOrEmpty(sub) ? "Home" : sub;

            var model = new DashboardViewModel()
            {
                Code = cod,
                Head = hed,
                Message = msg,

                PartialView = sub
            };

            ViewBag.Title = "Admin";

            return View(model);
        }        

        [HttpGet]
        public ActionResult Home()
        {

            ViewBag.Institutions = new SelectList(_db.Institutions, "Id", "Name");

            return PartialView();
        }

        [HttpGet]
        public JsonResult GetTree()
        {
            MenuService _ms = new MenuService(_db);

            var smh = _ms.GetMenu((string)Session["roleId"]);

            return Json(smh, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Users()
        {

            var users = _db.Users.Select(a => new UsersViewModel()
            {
                Id = a.Id,
                FirstName = a.FirstName,
                MiddleName = a.MiddleName,
                LastName = a.LastName,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                UserName = a.UserName,
                Groups = a.Groups.Count,
                Roles = a.Roles.Count,
                ExternalLogins = a.Logins.Count
            })
            .OrderBy(b=> b.UserName);
            return PartialView(users);
        }

        [HttpGet]
        public ActionResult Groups()
        {
            var groups = _db.Groups.Select(a => new GroupViewModel()
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                UsersCount = a.Users.Count,
                PermCount = a.Permissions.Count,
                InstId = a.InstId,
                Institution = a.Institution.Name,
                ParentId = a.ParentId,
                Parent = a.ParentId == null ? "" : a.Parent.Name

            })
            .OrderBy(b => b.Name)
            .ToList();

            ViewBag.Groups = new SelectList(_db.Groups, "Id", "Name");
            ViewBag.Institutions = new SelectList(_db.Institutions, "Id", "Name");

            return PartialView(groups);
        }

        [HttpGet]
        public ActionResult Roles()
        {

            var roles = _db.Roles.Select(a => new RoleViewModel()
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                UsersCount = a.Users.Count,
                PermCount = a.Permissions.Count
            })
            .OrderBy(b => b.Name)
            .ToList();

            return PartialView(roles);
        }

        [HttpGet]
        public ActionResult Inst()
        {

            var insts = _db.Institutions.Select(a => new InstViewModel()
            {
                Id = a.Id,
                Name = a.Name,
                Campus = a.Campus,
                Crest = a.CollegeCrest == null ? false : true
            })
            .OrderBy(b => b.Campus)
            .ToList();

            return PartialView(insts);
        }


        [HttpGet]
        public ActionResult Perm()
        {

            var perms = _db.Permissions.Select(a => new PermsViewModel()
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Groups = a.Groups.Count,
                Roles = a.Roles.Count
            })
            .OrderBy(b => b.Name)
            .ToList();

            return PartialView(perms);
        }

        [HttpPost]
        public async Task<ActionResult> SeedUsers(int from, int count)
        {
            if (count < 1)
            {
                ViewBag.Code = "danger";
                ViewBag.Head = "Error";
                ViewBag.Messages = new List<string>() { "Cannot generate empty list" };
                return RedirectToAction("Index");
            }

            int errors = 0;
            int success = 0;
            for (int i = 0; i < count; i++)
            {
                string numb = from.ToString().PadLeft(4, '0');
                var user = new ApplicationUser
                {
                    FirstName = "First" + numb,
                    MiddleName = "Test" + numb,
                    LastName = "Last" + numb,
                    PhoneNumber = "1234567890",
                    UserName = "test" + numb,
                    Email = "test" + numb + "@cam.ac.uk"
                };

                var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var result = await UserManager.CreateAsync(user, "Password1!");
                if (result.Succeeded)
                    success++;
                errors = +result.Errors.Count();
                from++;
            }
            // Create User

                ViewBag.Code = "info";
                ViewBag.Head = "Info";
                ViewBag.Messages = new List<string>() { "Created "+success+" users. Returned "+errors+" errors." };

            return RedirectToAction("Index");
        }

        

    }
}