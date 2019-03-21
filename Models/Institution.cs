using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CascBasic.Models
{
    [Table("Institutions")]
    public class Institution
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64    Id { get; set; }

        public string   Name { get; set; }
        public string   ZEmail { get; set; }
        public string   WelcomeMsg { get; set; }
        public string   Campus { get; set; }
        public string   CollegeName { get; set; }
        public string   CollegePhone { get; set; }
        public string   CollegeFax { get; set; }
        public string   CollegeVATnumber { get; set; }
        public byte[]   CollegeCrest { get; set; }  //image
        public string   CollegeCharityNumber { get; set; }
        public bool?     InformChanges { get; set; }
        public int?      TrumpSafeMinutesShortTerm { get; set; }
        public int?      TrumpSafeMinutesLongTerm { get; set; }
        public string   NoAccountMsg { get; set; }
        public string   ContactName { get; set; }
        public string   ContactEmail { get; set; }
        public bool?     AcceptInventory { get; set; }
        public string   BkAcceptInvInstructions { get; set; }
        public int?      BkAcceptInvLimitDays { get; set; }
        public int?      AccInvoiceTypeID { get; set; }
        public int?      AddrID { get; set; }
        public bool?     ShowRoomState { get; set; }
        public int?      BkEndDateEmailDays { get; set; }
        public int?      PrevTicketsOnQuickIssue { get; set; }
        public int?      PkgeReminderEmailDays { get; set; }
        public string   PkgeFromEmail { get; set; }

        public virtual ICollection<ApplicationGroup> Groups { get; set; }

        #region Helpers
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Institution);
        }

        public bool Equals(Institution other)
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