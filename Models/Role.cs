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
        }

        public ApplicationRole(string name) : this()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Name = name;
        }

        public string Description { get; set; }
        public virtual ICollection<ApplicationGroup> Groups { get; set; }

    }

    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public ApplicationUserRole() : base() { }

        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }        
    }
}