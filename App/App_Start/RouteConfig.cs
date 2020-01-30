using System.Web.Mvc;
using System.Web.Routing;

namespace App
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "BookCategory",
                url: "Books/{metatitle}-{catBook}",
                defaults: new { controller = "Books", action = "Index", id = UrlParameter.Optional },
                new[] { "App.Controllers" }
            );

            routes.MapRoute(
                name: "SearchBooks",
                url: "Search",
                defaults: new { controller = "Books", action = "Search", id = UrlParameter.Optional },
                new[] { "App.Controllers" }
            );

            routes.MapRoute(
                name: "Books",
                url: "Books",
                defaults: new { controller = "Books", action = "Index" },
                new[] { "App.Controllers" }
            );

            routes.MapRoute(
                name: "BookDetail",
                url: "Books/Detail/{metatitle}-{id}",
                defaults: new { controller = "Books", action = "Detail" },
                new[] { "App.Controllers" }
            );

            routes.MapRoute(
               name: "Flavors",
               url: "Flavors",
               defaults: new { controller = "Flavors", action = "Index" },
               new[] { "App.Controllers" }
           );

            routes.MapRoute(
                name: "FlavorDetail",
                url: "Flavors/Details/{metatitle}-{id}",
                defaults: new { controller = "Flavors", action = "Details" },
                new[] { "App.Controllers" }
            );

            routes.MapRoute(
                name: "Feedbacks",
                url: "Feedbacks",
                defaults: new { controller = "Feedbacks", action = "Index" },
                new[] { "App.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "App.Controllers" }
            );
        }
    }
}
