using App.Models;
using System.Collections.Generic;

namespace App.ViewModels.Admin.Order
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal ShippingFee { get; set; }
        public string StaffName { get; set; }
        public string CustomerName { get; set; }
        public decimal Tax { get; set; }
        public byte Status { get; set; }
        public decimal PaymentSum { get; set; }

        public string Message { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}