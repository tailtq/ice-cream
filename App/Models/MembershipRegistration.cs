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
    
    public partial class MembershipRegistration
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal PaymentSum { get; set; }
        public byte MembershipType { get; set; }
        public System.DateTime CreatedAt { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}
