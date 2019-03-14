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
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        ApplicationDbContext _db;

        public DashboardController()
        {
            _db = new ApplicationDbContext();
        }

        public ActionResult Index(string sub)
        {
            var model = new DashboardViewModel()
            {
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
                UsersCount = a.Users.Count
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
                UsersCount = a.Users.Count
            }).ToList();

            return PartialView(roles);
        }
    }
}