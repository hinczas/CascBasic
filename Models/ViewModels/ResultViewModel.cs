using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CascBasic.Models.ViewModels
{
    public class ResultViewModel
    {
        public string Header { get; set; }
        public string Class { get; set; }
        public string Message { get; set; }

        public ResultViewModel(string hdr, string cls, string msg)
        {
            Header = hdr;
            Class = cls;
            Message = msg;
        }
    }
}