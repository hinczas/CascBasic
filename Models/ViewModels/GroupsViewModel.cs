using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CascBasic.Models.ViewModels
{
    public class GroupsViewModel
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public int UsersCount { get; set; }
    }
}