using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Helpers
{
    public class CommonHelper
    {
        public static string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl.Substring(0, baseUrl.Length - 1);
        }

        public static string GenerateToken(int length)
        {
            Random random = new Random();
            const string chars = "0123456789";

            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomString(int length, string type = "digitText")
        {
            string chars;
            Random random = new Random();

            if (type == "digit")
            {
                chars = "0123456789";
            }
            else
            {
                chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            }

            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}