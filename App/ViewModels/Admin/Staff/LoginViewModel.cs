using System.ComponentModel.DataAnnotations;

namespace App.ViewModels.Admin.Staff
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}