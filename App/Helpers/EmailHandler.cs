using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace App.Helpers
{
    public class EmailHandler
    {
        const string EMAIL = "icecream.aptech1@gmail.com";

        const string PASSWORD = "Abcd@1234";

        [Obsolete]
        public static void Handle<T>(T model, string toEmail, string title, string filePath)
        {
            string[] paths = GetPaths(filePath);
            var templateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, paths[0]);
            var welcomeEmailTemplatePath = Path.Combine(templateFolderPath, paths[1]);

            TemplateService templateService = new TemplateService();
            var emailHtmlBody = templateService.Parse(System.IO.File.ReadAllText(welcomeEmailTemplatePath), model, null, null);
            Debug.Print(emailHtmlBody);
            using (var message = new MailMessage(EMAIL, toEmail))
            {
                message.Body = emailHtmlBody;
                message.IsBodyHtml = true;
                message.Subject = title;
                GetStmpClient().Send(message);
            }
        }

        private static SmtpClient GetStmpClient()
        {
            return new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(EMAIL, PASSWORD)
            };
        }

        /**
         * Index:
         * - 0: Directory path
         * - 1: File name
         */
        private static string[] GetPaths(string path)
        {
            string[] paths = path.Split('/');
            string directoryPath = "";
            for (int i = 0; i < paths.Length - 1; i++)
            {
                directoryPath += paths[i] + "/";
            }
            directoryPath = directoryPath.Remove(directoryPath.Length - 1);

            return new string[]
            {
                directoryPath,
                paths[paths.Length - 1]
            };
        }
    }
}