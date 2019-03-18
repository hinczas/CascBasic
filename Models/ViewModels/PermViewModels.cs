using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CascBasic.Models.ViewModels
{
    public class PermViewModels
    {
        public class GroupDetViewModel : StatusViewModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public List<UserDetViewModel> Users { get; set; }
            public List<RoleViewModel> Roles { get; set; }
        }

        public class CreatePermViewModel
        {
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