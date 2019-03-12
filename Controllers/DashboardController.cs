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
    //[Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        ApplicationDbContext _db;

        public DashboardController()
        {
            _db = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
                return View();
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
                Roles = a.Roles.Count
            });
            return PartialView(users);
        }

        [HttpGet]
        public ActionResult Groups()
        {
            var groups = _db.Groups.Select(a => new GroupsViewModel()
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
            var roles = _db.Roles.Select(a => new RolesViewModel()
            {
                Id = a.Id,
                Name = a.Name,
                UsersCount = a.Users.Count
            }).ToList();

            return PartialView(roles);
        }



        #region CreateTestUsers
        public async Task CreateTestUsers()
        {
            var db = new ApplicationDbContext();
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new ApplicationUserManager(userStore);

            for (var i = 6; i <= 200; i++)
            {
                var username = "test" + i.ToString().PadLeft(4, '0');

                var user = db.Users.FirstOrDefault(u => u.UserName == username);
                if (user == null)
                {
                    user = new ApplicationUser()
                    {
                        UserName = username,
                        Email = username + "@test.test",
                        EmailConfirmed = true,
                        LockoutEnabled = false
                    };

                    var password = "Test12345*";
                    var result = await userManager.CreateAsync(user, password);
                }
            }
        }
        #endregion
    }
}