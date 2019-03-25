using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CascBasic.Models
{
    [Table("AspNetGroups")]
    public class ApplicationGroup
    {

        public ApplicationGroup()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new List<ApplicationRole>();
            this.Users = new List<ApplicationUser>();
            this.MenuItems = new List<MenuItem>();
        }

        public ApplicationGroup(string name)
            : this()
        {
            this.Name = name;
        }

        public ApplicationGroup(string name, string description)
            : this(name)
        {
            this.Description = description;
        }

        [Key]
        public string Id { get; set; }

        [StringLength(64)]
        [Index(IsUnique = true)]
        public string Name { get; set; }
        public string Description { get; set; }

        [ForeignKey("Institution")]
        public Int64 InstId { get; set; }

        [ForeignKey("Parent")]
        public string ParentId { get; set; }


        public virtual Institution Institution { get; set; }
        public virtual ApplicationGroup Parent { get; set; }
        public virtual ICollection<ApplicationRole> Roles { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
        public virtual ICollection<MenuItem> MenuItems { get; set; }
        public virtual ICollection<ApplicationGroup> Children { get; set; }

        #region Helpers
        public override bool Equals(object obj)
        {
            return this.Equals(obj as ApplicationGroup);
        }

        public bool Equals(ApplicationGroup other)
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