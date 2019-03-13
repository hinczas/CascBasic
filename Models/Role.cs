using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CascBasic.Models
{
    [Table("AspNetRoles")]
    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string name) : this()
        {
            this.Name = name;
        }

        public virtual ICollection<Group> Groups { get; set; }
    }

    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public ApplicationUserRole() : base() { }

        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
}