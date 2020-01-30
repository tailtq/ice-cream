using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.ViewModels.Client.Home
{
    public class BookRankingViewModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Images { get; set; }
        public decimal Price { get; set; }
        public double Discount { get; set; }
        public int Count { get; set; }

        public string[] DecodedImages()
        {
            try
            {
                return JsonConvert.DeserializeObject<string[]>(this.Images);
            }
            catch
            {
                return new string[] { "" };
            }
        }
    }
}