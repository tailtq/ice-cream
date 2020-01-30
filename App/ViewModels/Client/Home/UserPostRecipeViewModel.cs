using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.ViewModels.Client.Home
{
    public class UserPostRecipeViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public DateTime FlavorCreateAt { get; set; }

        public string[] DecodedImages()
        {
            try
            {
                return JsonConvert.DeserializeObject<string[]>(this.Avatar);
            }
            catch
            {
                return new string[] { "/Content/Client/img/others/blank-avatar.png" };
            }
        }
    }
}