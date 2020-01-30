using Newtonsoft.Json;
using System.Configuration;
using System.Linq;
using System.Web;

namespace App.Helpers
{
    public class SaveImages
    {
        public static string SaveAvatarFile(HttpPostedFileBase avatarFile, string email)
        {
            string dirPath = HttpContext.Current.Server.MapPath("\\") + ConfigurationManager.AppSettings["CusAvatar"].Replace("/", "\\");
            string fileName = email.Split('.').First() + "." + avatarFile.FileName.Split('.').Last();
            avatarFile.SaveAs(dirPath + fileName);
            string[] images = new string[] { "/Content/Images/Avatar/" + fileName };

            return JsonConvert.SerializeObject(images);
        }

        public static string SaveImagesFile(HttpPostedFileBase avatarFile, string name)
        {
            string dirPath = HttpContext.Current.Server.MapPath("\\") + ConfigurationManager.AppSettings["CusImages"].Replace("/", "\\");
            string fileName = name.Split('.').First() + "." + avatarFile.FileName.Split('.').Last();
            avatarFile.SaveAs(dirPath + fileName);
            string[] images = new string[] { "/Content/Images/Product/" + fileName };

            return JsonConvert.SerializeObject(images);
        }
    }
}