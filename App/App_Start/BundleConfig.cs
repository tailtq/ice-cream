using System.Web;
using System.Web.Optimization;

namespace App
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Admin Bundles
            bundles.Add(new StyleBundle("~/Content/Admin/css").Include(
                      "~/Content/Admin/css/adminlte.min.css",
                      "~/Content/Admin/vendors/fontawesome-free/css/all.min.css",
                      "~/Content/Admin/vendors/sweetalert/sweetalert2.min.css",
                      "~/Content/Admin/css/custom.css"));

            bundles.Add(new ScriptBundle("~/Scripts/Admin/js").Include(
                      "~/Scripts/Admin/vendors/jquery/jquery.min.js",
                      "~/Scripts/Admin/vendors/bootstrap/js/bootstrap.bundle.min.js",
                      "~/Scripts/Admin/vendors/fastclick/fastclick.js",
                      "~/Scripts/Admin/js/adminlte.min.js",
                      "~/Content/Admin/vendors/sweetalert/sweetalert2.min.js",
                      "~/Scripts/Admin/js/custom.js"));

            // Client Bundles
            bundles.Add(new StyleBundle("~/Content/Client/css").Include(
                      "~/Content/Client/css/plugins.css",
                      "~/Content/Client/css/main.css",
                      "~/Content/Admin/vendors/sweetalert/sweetalert2.min.css",
                      "~/Content/Client/css/custom.css"));

            bundles.Add(new ScriptBundle("~/Scripts/Client/js").Include(
                      "~/Scripts/Client/plugins.min.js",
                      "~/Scripts/Client/ajax-mail.js",
                      "~/Content/Admin/vendors/sweetalert/sweetalert2.min.js",
                      "~/Scripts/Client/custom.js",
                      "~/Scripts/Client/app.js"));
        }
    }
}
