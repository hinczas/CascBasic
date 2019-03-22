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

        public string ContactName { get; set; }
        public string ContactEmail { get; set; }

        // Additonal info
        public List<GroupViewModel> BasicGroups { get; set; }

    }

    public class InstBasicViewModel : StatusViewModel
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
    }
}