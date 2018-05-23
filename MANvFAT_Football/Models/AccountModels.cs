using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;
//using System.Web.Mvc;

namespace MANvFAT_Football.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [StringLength(20, ErrorMessage = "Minimum 6 characters required", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "at least one Capital letter and a Number required")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password Required")]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ForgottenPasswordModel
    {

        [Required(ErrorMessage = "Please enter Email Address")]
        //  [EmailAddress(ErrorMessage = "Please enter valid Email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,8}$", ErrorMessage = "Please enter Valid Email Address")]
        public string Email { get; set; }
        public string DashboardURLId { get; set; }
        public string Reason { get; set; }
        public string AlertType { get; set; }

    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class ResetPasswordModel
    {
        public long UserID { get; set; }
        public bool status { get; set; }
        public string Reason { get; set; }
        public string AlertType { get; set; }

        [StringLength(20, ErrorMessage = "Minimum 6 characters required", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "at least one Capital letter and a Number required")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "New Password Required")]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class EmailAddressAttribute : RegularExpressionAttribute
    {
        private const string pattern = @"^\w+([-+.]*[\w-]+)*@(\w+([-.]?\w+)){1,}\.\w{2,4}$";

        static EmailAddressAttribute()
        {
            // necessary to enable client side validation
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(EmailAddressAttribute), typeof(RegularExpressionAttributeAdapter));
        }

        public EmailAddressAttribute()
            : base(pattern)
        {
        }
    }
}
