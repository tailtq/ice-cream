using System.ComponentModel.DataAnnotations;

namespace App.ViewModels.Admin.Payout
{
    public class ApproveFlavorViewModel
    {
        [Required]
        [Range(0.01, 10000, ErrorMessage = "Please enter a value bigger than {1}")]
        public decimal SumTotal { get; set; }
    }
}