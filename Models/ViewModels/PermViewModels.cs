using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CascBasic.Models.ViewModels
{
    public class PermViewModels
    {
        public class BasicListPermVM
        {
            public string Id { get; set; }
            public string Name { get; set; }

            #region Helpers
            public override bool Equals(object obj)
            {
                return this.Equals(obj as BasicListPermVM);
            }

            public bool Equals(BasicListPermVM other)
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

        public class GroupDetViewModel : StatusViewModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string InstName { get; set; }
            public long InstId { get; set; }
            public bool InstCrest { get; set; }
            public List<UserDetViewModel> Users { get; set; }
            public List<RoleViewModel> Roles { get; set; }
            public List<PermsViewModel> Perms { get; set; }
            public SelectList Insts { get; set; }
            public SelectList Groups { get; set; }
        }

        public class RoleDetViewModel : StatusViewModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public List<UserDetViewModel> Users { get; set; }
            public List<GroupViewModel> Groups { get; set; }
            public List<PermsViewModel> Perms { get; set; }
        }

        public class CreatePermViewModel
        {
            [Display(Name = "Institution")]
            public long InstId { get; set; }

            [Display(Name = "Parent Group")]
            public string ParentId { get; set; }

            [Required]
            [Display(Name = "Short name")]
            public string Name { get; set; }

            [Display(Name = "Description")]
            public string Description { get; set; }
        }

        public class UserDetViewModel
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Checked { get; set; }

            #region Helpers
            public override bool Equals(object obj)
            {
                return this.Equals(obj as UserDetViewModel);
            }

            public bool Equals(UserDetViewModel other)
            {
                if (other == null)
                    return false;

                return this.Id == other.Id && this.Email.Equals(other.Email);
            }

            public override int GetHashCode()
            {

                int email = Email.GetHashCode();
                int id = Id.GetHashCode();

                return email ^ id;
            }
            #endregion
        }
    }
}