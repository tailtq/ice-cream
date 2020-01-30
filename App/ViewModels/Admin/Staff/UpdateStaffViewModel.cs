using System.ComponentModel.DataAnnotations;

namespace App.ViewModels.Admin.Staff
{
    public class UpdateStaffViewModel
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Display(Name = "User name")]
        public string Name { get; set; }
        public string Address { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public int LevelId { get; set; }
    }
}