using CascBasic.Classes;
using CascBasic.Context;
using CascBasic.Models;
using CascBasic.Models.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CascBasic.Controllers
{
    [AuthorizeRoles(Roles = "Admin")]
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
    }
}