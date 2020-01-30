using App.Helpers;
using System.ComponentModel.DataAnnotations;

namespace App.ViewModels.Client.Customer
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(30)]
        public string Password { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(255)]
        public string Address { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(30)]
        public string Phone { get; set; }
    }
}