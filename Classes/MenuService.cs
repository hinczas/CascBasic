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

            var userMenus = user.Groups.SelectMany(a => a.MenuItems).Distinct().ToList();

            if (userMenus == null || userMenus.Count < 1)
                return null;

            var menuParents = userMenus.Select(a => a.Parent).Where(b => b!=null).Distinct().ToList();

            var allMenus = userMenus.Union(menuParents).Distinct().ToList();

            List<MenuViewModel> menu = new List<MenuViewModel>();

            foreach (var item in menuParents)
            {
                var parentMenu = new MenuEntryVM()
                {
                    Label = item.Label,
                    Url = item.Url
                };
                var submenus = allMenus.Where(a => a.ParentId == item.Id).Select(b => new MenuEntryVM()
                {
                    Label = b.Label,
                    Url = b.Url
                }).ToList();

                menu.Add(new MenuViewModel(parentMenu, submenus));
            }

            return menu;            
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