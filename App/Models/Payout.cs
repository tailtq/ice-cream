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
    
    public partial class Payout
    {
        public int Id { get; set; }
        public int FlavorId { get; set; }
        public int CustomerId { get; set; }
        public int StaffId { get; set; }
        public decimal SumTotal { get; set; }
        public string Message { get; set; }
        public System.DateTime CreatedAt { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Flavor Flavor { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
