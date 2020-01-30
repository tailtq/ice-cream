using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Xunit;

namespace App.ViewModels.Admin.Staff
{
    public class StaffViewModel
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Display(Name="User name")]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Incorrect entry")]
        public string ConfirmPassword { get; set; }

        public string Address { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public int LevelId { get; set; }
    }
}