using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CascBasic.Models
{
    [Table("Permissions")]
    public class Permission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        [StringLength(64)]
        [Index(IsUnique = true)]
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ApplicationRole> Roles { get; set; }
        //public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<ApplicationGroup> Groups { get; set; }

        #region Helpers
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Permission);
        }

        public bool Equals(Permission other)
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