using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App.ViewModels.Client.Order
{
    public class CancelOrderViewModel
    {
        [Required]
        public string Message { get; set; }
    }
}