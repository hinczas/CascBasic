using CascBasic.Context;
using CascBasic.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CascBasic.Controllers
{
    [Authorize(Roles = "admin")]
    public class DashboardController : Controller
    {
        ApplicationDbContext _db;

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
            _db = new ApplicationDbContext();
            var users = _db.Users.ToList();
            return PartialView(users);
        }

        [HttpGet]
        public ActionResult Groups()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult Roles()
        {
            return PartialView();
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