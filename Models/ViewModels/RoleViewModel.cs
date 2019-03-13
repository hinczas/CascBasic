using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CascBasic.Models.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int UsersCount { get; set; }


        #region Helpers
        public override bool Equals(object obj)
        {
            return this.Equals(obj as RoleViewModel);
        }

        public bool Equals(RoleViewModel other)
        {
            if (other == null)
                return false;

            return this.Id.Equals(other.Id) && this.Name.Equals(other.Name);
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