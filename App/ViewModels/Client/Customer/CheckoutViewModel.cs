using System.ComponentModel.DataAnnotations;

namespace App.ViewModels.Client.Customer
{
    public class CheckoutViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        [Required]
        [MaxLength(255)]
        public string Address { get; set; }
        [Required]
        [MaxLength(30)]
        public string Phone { get; set; }
        [MaxLength(255)]
        public string Message { get; set; }
    }
}