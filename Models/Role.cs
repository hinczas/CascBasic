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
        public ApplicationRole() : base() {
            this.Id = Guid.NewGuid().ToString();
            this.MenuItems = new List<MenuItem>();
        }

        public ApplicationRole(string name) : this()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Name = name;
            this.MenuItems = new List<MenuItem>();
        }

        public string Description { get; set; }
        public virtual ICollection<ApplicationGroup> Groups { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
        public virtual ICollection<MenuItem> MenuItems { get; set; }

    }

    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public ApplicationUserRole() : base() { }

        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }        
    }
}