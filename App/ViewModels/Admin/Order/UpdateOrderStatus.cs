using System.ComponentModel.DataAnnotations;

namespace App.ViewModels.Admin.Order
{
    public class UpdateOrderStatus
    {
        [Required]
        [EnumDataType(typeof(ValidStatuses))]
        public byte Status { get; set; }

        [MaxLength(255)]
        public string Message { get; set; }

        public enum ValidStatuses
        {
            DELIVERING = Models.Order.DELIVERING,
            RECEIVED = Models.Order.RECEIVED,
            CANCELED = Models.Order.CANCELED,
        }
    }
}