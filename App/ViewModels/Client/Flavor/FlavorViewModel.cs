using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App.ViewModels.Client.Flavor
{
    public class FlavorViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string Images { get; set; }

        public Nullable<int> PreparationTime { get; set; }

        public Nullable<int> TotalTime { get; set; }

        [Required]
        public string Ingredients { get; set; }

        [Required]
        public string Recipe { get; set; }
    }
}