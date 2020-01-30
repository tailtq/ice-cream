using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App.ViewModels.Admin.BookCategory
{
    public class BookCategoryViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}