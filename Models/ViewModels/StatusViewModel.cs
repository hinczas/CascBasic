using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CascBasic.Models.ViewModels
{
    public class StatusViewModel
    {
        public string Code { get; set; }
        public string Head { get; set; }
        public string Message { get; set; }
        public string Caller { get; set; }
    }
}