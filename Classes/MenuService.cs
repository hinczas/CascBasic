﻿using CascBasic.Context;
using CascBasic.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CascBasic.Classes
{
    public class MenuService
    {
        private ApplicationDbContext _db;
        private List<MenuEntryVM> _masterList;
        private List<MenuEntryVM> _parentItems;
        private List<MenuEntryVM> _childItems;


        public MenuService(ApplicationDbContext context)
        {
            _db = context;
            _masterList = new List<MenuEntryVM>();
            _parentItems = new List<MenuEntryVM>();
            _childItems = new List<MenuEntryVM>();
        }
        
        public List<MenuVM> GetMenu(string roleId)
        {
            var role = _db.Roles.Find(roleId);
            if (role == null)
                return null;

            var menuItems = role.MenuItems
                .Select(a => new MenuEntryVM()
            {
                Id = a.Id,
                Label = a.Label,
                Href = a.Url,
                ParentId = a.ParentId,
                SortOrder = a.SortOrder,
                Icon = a.FlatIconName
            }).ToList();

            // Get all parents (may include children duplicates)
            List<MenuEntryVM> parentItems = GetParents(menuItems.Select(a => a.Id).ToArray());

            // Clean up duplicates
            List<MenuEntryVM> parentsClean = parentItems.Except(menuItems).ToList();

            // Join all into single list
            _masterList = menuItems.Union(parentsClean).ToList();
            _parentItems = _masterList.Where(a => a.ParentId == null)
                .OrderBy(o => o.SortOrder)
                .ThenBy(t => t.Label).ToList();
            _childItems = _masterList.Where(a => a.ParentId != null)
                .OrderBy(o => o.SortOrder)
                .ThenBy(t => t.Label).ToList();



            return GetMenu(); ;            
        }

        public async Task<StatusCode> ProcessRoleMenusAsync(string roleId, long[] menus)
        {
            try
            {
                var store = new ApplicationRoleStore(_db);
                var role = await store.FindByIdAsync(roleId);

                var currMenus = role.MenuItems.Select(a => a.Id).ToArray();

                var addMenus = menus.Except(currMenus).ToArray();
                var remMenus = currMenus.Except(menus).ToArray();

                if (remMenus.Count() > 0)
                {
                    foreach (var id in remMenus)
                    {
                        var mi = _db.MenuItems.Find(id);
                        role.MenuItems.Remove(mi);
                    }
                    await store.UpdateAsync(role);
                }

                if (addMenus.Count() > 0)
                {
                    foreach (var id in addMenus)
                    {
                        var mi = _db.MenuItems.Find(id);
                        role.MenuItems.Add(mi);
                    }
                    await store.UpdateAsync(role);
                }

                return StatusCode.UpdateSuccess;

            } catch
            {
                return StatusCode.ExceptionThrown;
            }            
        }

        private void RemoveMenus(string roleId, long[] menus)
        {

        }
        private List<MenuEntryVM> GetParents(long[] ids)
        {
            List<MenuEntryVM> result = new List<MenuEntryVM>();

            foreach(long id in ids)
            {
                result.AddRange(GetParents(id));
            }

            return result.Distinct().ToList();
        }

        private List<MenuEntryVM> GetParents(long id)
        {
            List<MenuEntryVM> list = new List<MenuEntryVM>();

            var item = _db.MenuItems.Find(id);
            if (item.Parent == null)
                return list;

            list.AddRange(GetParents((long)item.ParentId));
            var mi = new MenuEntryVM()
            {
                Id = item.Parent.Id,
                Label = item.Parent.Label,
                Href = "#", // Parents are not clickable
                Icon = item.Parent.FlatIconName,
                ParentId = item.Parent.ParentId
            };

            list.Add(mi);

            return list;
        }

        private List<MenuVM> GetMenu()
        {
            var result = new List<MenuVM>();
            
            foreach (var item in _parentItems)
            {
                var ch = GetChildrenFromSet(item.Id);
                var i = new MenuVM(item, ch);

                result.Add(i);
            }

            return result;
        }

        private List<MenuVM> GetChildrenFromSet(long id)
        {
            List<MenuVM> tmp = new List<MenuVM>();

            if (_childItems == null || _childItems.Count == 0)
                return tmp;

            var _sub = _childItems.Where(a => a.ParentId == id).ToList();

            foreach (var child in _sub)
            {
                var ch = GetChildrenFromSet(child.Id);
                var i = new MenuVM(child, ch);

                tmp.Add(i);
            }

            return tmp;
        }

    }
}