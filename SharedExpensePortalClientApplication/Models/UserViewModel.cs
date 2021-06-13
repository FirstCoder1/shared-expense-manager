using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SharedExpensePortalClientApplication.Models
{
    public class UserViewModel
    {
        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name required")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "LastName required")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Display(Name = "User EmailID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "EmailID required")]
        [DataType(DataType.EmailAddress)]
        public string EmailID { get; set; }

        [Display(Name = "User PhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }

    public class UserScoreCardViewModel
    {
        public string UserName { get; set; }

        public string Score { get; set; }
    }
}