using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CascBasic.Models.ViewModels.ManageViewModels
{
    public class ManagePersonalVM : StatusViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
}