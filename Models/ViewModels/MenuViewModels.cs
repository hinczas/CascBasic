using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CascBasic.Models.ViewModels
{
    [Serializable()]
    public class MenuVM
    {
        //public MenuEntryVM MenuItem { get; set; }
        public string href { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public List<MenuVM> children { get; set; }

        public MenuVM(MenuEntryVM parent, List<MenuVM> subs)
        {
            href = parent.Href;
            text = parent.Label;
            icon = parent.Icon;
            children = subs;
        }
        public MenuVM(MenuEntryVM parent)
        {
            href = parent.Href;
            text = parent.Label;
            icon = parent.Icon;
        }
    }

    public class MenuEntryVM
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Href { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }
        public long SortOrder { get; set; }

        #region Helpers
        public override bool Equals(object obj)
        {
            return this.Equals(obj as MenuEntryVM);
        }

        public bool Equals(MenuEntryVM other)
        {
            if (other == null)
                return false;

            return this.Id == other.Id && this.Label.Equals(other.Label);
        }

        public override int GetHashCode()
        {

            int label = Label.GetHashCode();
            int id = Id.GetHashCode();

            return label ^ id;
        }
        #endregion
    }

    //public class MenuItemVM
    //{
    //    public long Id { get; set; }
    //    public long ParentId { get; set; }
    //    public string Href { get; set; }
    //    public string Label { get; set; }
    //}

    //public class MenuVM
    //{
    //    public MenuItemVM MenuItem { get; set; }
    //    public List<MenuVM> Children { get; set; }
    //}
}