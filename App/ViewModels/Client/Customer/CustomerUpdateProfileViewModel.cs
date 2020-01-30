using App.Helpers;
using System.ComponentModel.DataAnnotations;

namespace App.ViewModels.Client.Customer
{
    public class CustomerUpdateProfileViewModel
    {
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(255)]
        public string Address { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(30)]
        public string Phone { get; set; }

        [MaxLength(30)]
        public string CreditCard { get; set; }
    }
}