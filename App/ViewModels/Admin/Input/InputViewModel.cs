using App.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.ViewModels.Admin.Input
{
    public class InputViewModel
    {
        [Required]
        public int SupplierId { get; set; }
        [Required, MaxLength(10)]
        public string Code { get; set; }
        [Required]
        public System.DateTime ImportedAt { get; set; }
        [Required]
        public virtual ICollection<InputDetail> InputDetails { get; set; }
    }
}