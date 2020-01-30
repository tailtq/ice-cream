using System.ComponentModel.DataAnnotations;

namespace App.ViewModels.Admin.Staff {
    public class UpdateProfileViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Address { get; set; }

        [Required]
        [MaxLength(30)]
        public string Phone { get; set; }

        public string Avatar { get; set; }

        public System.DateTime CreatedAt { get; set; }
    }
}