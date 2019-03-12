using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CascBasic.Models
{
    [Table("Groups")]
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public string Name { get; set; }
        
        public virtual ICollection<ApplicationRole> Roles { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        #region Helpers
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Group);
        }

        public bool Equals(Group other)
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