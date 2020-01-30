using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App.ViewModels.Admin.Book
{
    public class BookViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please select option?")]
        [DisplayName("Categorys")]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string Images { get; set; }
        [Required]
        public decimal Price { get; set; }
        public double Discount { get; set; }
    }
}