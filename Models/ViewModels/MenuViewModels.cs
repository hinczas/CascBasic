using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CascBasic.Models.ViewModels
{
    public class MenuViewModel
    {
        public MenuEntryVM ParentMenu { get; set; }
        public List<MenuEntryVM> SubMenus { get; set; }

        public MenuViewModel(MenuEntryVM parent, List<MenuEntryVM> subs)
        {
            ParentMenu = parent;
            SubMenus = subs;
        }
    }

    public class MenuEntryVM
    {
        public string Label { get; set; }
        public string Url { get; set; }
    }
}