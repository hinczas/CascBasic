using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CascBasic.Models
{
    [Table("MenuItems")]
    public class MenuItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        public string Label { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Url { get; set; }
        public string FlatIconName { get; set; }

        public Int64? ParentId { get; set; }
        public virtual MenuItem Parent { get; set; }

        public virtual ICollection<ApplicationGroup> Groups { get; set; }

        #region Helpers
        public override bool Equals(object obj)
        {
            return this.Equals(obj as MenuItem);
        }

        public bool Equals(MenuItem other)
        {
            if (other == null)
                return false;

            return this.Id == other.Id && this.Label.Equals(other.Label);
        }

        public override int GetHashCode()
        {

            int label = Label.GetHashCode();
            int id = Id.GetHashCode();

            return label ^ id;
        }
        #endregion
    }

    public class NavButtons
    {
        public string ListName { get; set; }
        public string ListUrl { get; set; }

        public List<DropItem> DropItems { get; set; }

    }

    public class DropItem
    {
        public string DropName { get; set; }
        public string DropUrl { get; set; }
    }
}