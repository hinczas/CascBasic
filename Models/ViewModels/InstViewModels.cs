using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CascBasic.Models.ViewModels
{
    public class InstManageViewModel : StatusViewModel
    {
        // Basic Info
        public Int64 Id { get; set; }

        public string Name { get; set; }
        public string ZEmail { get; set; }
        public string WelcomeMsg { get; set; }
        public string Campus { get; set; }
        public string CollegeName { get; set; }
        public string CollegePhone { get; set; }
        public string CollegeFax { get; set; }
        public string CollegeVATnumber { get; set; }
        public string CollegeCharityNumber { get; set; }
        public bool Crest { get; set; }

        // Additonal info
        public bool? InformChanges { get; set; }
        public int? TrumpSafeMinutesShortTerm { get; set; }
        public int? TrumpSafeMinutesLongTerm { get; set; }
        public string NoAccountMsg { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public bool? AcceptInventory { get; set; }
        public string BkAcceptInvInstructions { get; set; }
        public int? BkAcceptInvLimitDays { get; set; }
        public int? AccInvoiceTypeID { get; set; }
        public int? AddrID { get; set; }
        public bool? ShowRoomState { get; set; }
        public int? BkEndDateEmailDays { get; set; }
        public int? PrevTicketsOnQuickIssue { get; set; }
        public int? PkgeReminderEmailDays { get; set; }
        public string PkgeFromEmail { get; set; }

        public List<GroupViewModel> BasicGroups { get; set; }

    }
}