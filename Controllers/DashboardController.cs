using CascBasic.Classes;
using CascBasic.Context;
using CascBasic.Models;
using CascBasic.Models.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
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
            ViewBag.Title = "Dashboard";
            string cod = "";
            string hed = "";
            string msg = "";
            if (TempData["Messages"] != null)
            {
                cod = (string)TempData["Code"];
                hed = (string)TempData["Head"];
                msg = String.Join("\n", ((List<string>)TempData["Messages"]).ToArray());
            }

            var model = new DashboardViewModel()
            {
                Code = cod,
                Head = hed,
                Message = msg,

                PartialView = string.IsNullOrEmpty(sub) ? "Home" : sub
            };

            return View(model);
        }        

        [HttpGet]
        public ActionResult Home()
        {
            ViewBag.Title = "Dashboard";

            return PartialView();
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
            });
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
                RolesCount = a.Roles.Count
            }).ToList();

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
                GroupsCount = a.Groups.Count
            }).ToList();

            return PartialView(roles);
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