using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MongoMovie.Models.AppUsersViewModels
{
    public class CreateViewModel
    {

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Telephone")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} must more than {2}, but less than {1} 。", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Roles")]
        public IList<string> Roles { get; set; }
    }
}
