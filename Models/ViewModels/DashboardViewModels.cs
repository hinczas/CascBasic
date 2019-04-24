using System.Collections.Generic;

namespace CascBasic.Models.ViewModels
{
    public class DashboardViewModel : StatusViewModel
    {
        public string PartialView { get; set; }
        public List<DashLink> DashLinks { get; set; }
    }

    public class DashLink
    {
        public string Action { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }
    }

    public class GroupViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long InstId { get; set; }
        public string Institution { get; set; }
        public string ParentId { get; set; }
        public string Parent { get; set; }
        public int UsersCount { get; set; }
        public int RolesCount { get; set; }
        public int PermCount { get; set; }
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
        public string Description { get; set; }
        public int UsersCount { get; set; }
        public int GroupsCount { get; set; }
        public int PermCount { get; set; }
        public string Checked { get; set; }
        public bool CanCreate { get; set; }


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

    public class PermsViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Groups { get; set; }
        public int Roles { get; set; }
        public int Users { get; set; }
        public string Checked { get; set; }

        #region Helpers
        public override bool Equals(object obj)
        {
            return this.Equals(obj as PermsViewModel);
        }

        public bool Equals(PermsViewModel other)
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

    public class InstViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Campus { get; set; }
        public bool Crest { get; set; }
    }


    public class MenusViewModel
    {
        public List<MenusRoleVM> Roles { get; set; }
        public List<MenusMenuItemVM> Available { get; set; }
    }

    public class MenusRoleVM
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
    public class MenusMenuItemVM
    {
        public long MenuItemId { get; set; }
        public string MenuItemName { get; set; }
    }

}