using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.ViewModels.Client.Home
{
    public class FlavorRakingViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Images { get; set; }
        public Nullable<int> PreparationTime { get; set; }
        public Nullable<int> TotalTime { get; set; }
        public string Ingredients { get; set; }
        public string Recipe { get; set; }
        public string Slug { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime UpdatedAt { get; set; }
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