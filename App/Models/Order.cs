//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace App.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
        }

        public const int PROCESSING = 1;
        public const int DELIVERING = 2;
        public const int RECEIVED = 3;
        public const int CANCELED = 4;
        public const int WANT_TO_CANCEL = 5;

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int? StaffId { get; set; }
        public string Code { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Tax { get; set; }
        public byte Status { get; set; }
        public string Message { get; set; }
        public System.DateTime ArrivalDate { get; set; }
        public decimal PaymentSum { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime UpdatedAt { get; set; }

        public decimal GrandTotal
        {
            get
            {
                return PaymentSum + Tax + ShippingFee;
            }
        }
    
        public virtual Customer Customer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
