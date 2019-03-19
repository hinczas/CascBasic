using CascBasic.Context;
using CascBasic.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CascBasic.Classes
{
    public class MenuService
    {
        private ApplicationDbContext _db;

        public MenuService(ApplicationDbContext context)
        {
            _db = context;
        }


        public List<MenuViewModel> GetMenu(string userId)
        {
            var user = _db.Users.Find(userId);
            if (user == null)
                return null;

            var userMenus = user.Groups.SelectMany(a => a.MenuItems).ToList();

            if (userMenus == null || userMenus.Count < 1)
                return null;


            return null;
        }

        public List<MenuViewModel> GetMenu()
        {

            return null;
        }

        private void BottomUpMenus()
        {

        }
    }
}