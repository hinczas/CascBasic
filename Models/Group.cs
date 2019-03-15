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

        public virtual ICollection<ApplicationRole> Roles { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

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