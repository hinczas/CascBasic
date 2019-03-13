using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CascBasic.Models.ViewModels
{
    public class GroupViewModel
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public int UsersCount { get; set; }

        #region Helpers
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GroupViewModel);
        }

        public bool Equals(GroupViewModel other)
        {
            if (other == null)
                return false;

            return this.Id == other.Id && this.Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {

            int name = Name.GetHashCode();
            int id = Id.GetHashCode();

            return name ^ id;
        }
        #endregion
    }
}