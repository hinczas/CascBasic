using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CascBasic.Models.ViewModels
{
    public class PermViewModels
    {
        public class CreatePermViewModel
        {
            [Required]
            [Display(Name = "Short name")]
            public string Name { get; set; }

            [Display(Name = "Description")]
            public string Description { get; set; }
        }
    }
}