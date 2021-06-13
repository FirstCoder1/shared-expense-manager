using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SharedExpenseApplicationDataAccess;

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

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserScoreCardViewModel
    {
        public string UserName { get; set; }

        public string Score { get; set; }
    }

    public class ExpenseViewModel
    {
        [Display(Name = "User Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name required")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Display(Name = "User EmailId")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name required")]
        [DataType(DataType.EmailAddress)]
        public string EmailId { get; set; }

        [Display(Name = "Expense Title")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "LastName required")]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Display(Name = "Expense Description")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "EmailID required")]
        [DataType(DataType.Text)]
        public string Description { get; set; }

        [Display(Name = "Expense Date")]
        [DataType(DataType.DateTime)]
        public DateTime ExpenseDate { get; set; }

        [Display(Name = "Expense Amount")]
        [DataType(DataType.Currency)]
        public decimal ExpenseAmount { get; set; }

        public string Expense { get; set; }

    }
}