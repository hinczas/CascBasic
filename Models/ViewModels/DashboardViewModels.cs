using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CascBasic.Models.ViewModels
{
    public class DashboardViewModel
    {
        public string PartialView { get; set; }
    }

    public class GroupViewModel
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public int UsersCount { get; set; }
        public string Checked { get; set; }

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

    public class RoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int UsersCount { get; set; }
        public string Checked { get; set; }


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


    public class UsersViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public int Groups { get; set; }
        public int Roles { get; set; }
        public int ExternalLogins { get; set; }
    }


}