using App.Helpers;
using System.ComponentModel.DataAnnotations;

namespace App.ViewModels.Client.Customer
{
    public class CustomerResetPasswordViewModel
    {
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = ErrorMessages.COMPARE)]
        public string ConfirmNewPassword { get; set; }
    }
}