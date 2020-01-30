using App.Helpers;
using System.ComponentModel.DataAnnotations;

namespace App.ViewModels.Client.Customer
{
    public class CustomerForgotPasswordViewModel
    {
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public string Email { get; set; }

        public string Url { get; set; }
    }
}