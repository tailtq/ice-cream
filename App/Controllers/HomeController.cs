using App.Models;
using App.Services;
using System.Web.Mvc;
using System.Linq;
using App.ViewModels.Client.Home;

namespace App.Controllers
{
    public class HomeController : Controller
    {
        IceCreamEntities db = new IceCreamEntities();

        // GET: /
        public ActionResult Index()
        {
            GetFavourites();
            // Get books ranking
            var queryBooks = from a in db.Books.Where(x => x.DeletedAt == null)
                             select new BookRankingViewModel
                             {
                                 Id = a.Id,
                                 Name = a.Name,
                                 Description = a.Description,
                                 Images = a.Images,
                                 Slug = a.Slug,
                                 Sku = a.Sku,
                                 Price = a.Price,
                                 CategoryName = a.BookCategory.Name,
                                 Discount = a.Discount,
                                 Count = (from b in db.Favourites where b.ItemId == a.Id && b.ItemType == App.Models.Favourite.BOOKS && b.DeletedAt == null select b).Count()
                             };
            var bookRaking = queryBooks.Where(x => x.Count > 0).OrderByDescending(x => x.Count).Take(12).ToList();

            // Get flavors ranking
            var queryFlavors = from a in db.Flavors.Where(x => x.DeletedAt == null && x.IsApproved == true)
                               select new FlavorRakingViewModel
                               {
                                   Id = a.Id,
                                   Name = a.Name,
                                   Description = a.Description,
                                   Images = a.Images,
                                   PreparationTime = a.PreparationTime,
                                   TotalTime = a.TotalTime,
                                   Ingredients = a.Ingredients,
                                   Recipe = a.Recipe,
                                   Slug = a.Slug,
                                   Count = (from b in db.Favourites where b.ItemId == a.Id && b.ItemType == App.Models.Favourite.FLAVORS && b.DeletedAt == null select b).Count()
                               };
            var flavorsRaking = queryFlavors.Where(x => x.Count > 0).OrderByDescending(x => x.Count).Take(12).ToList();

            // Get user posted recipe
            var user = from a in db.Customers.Where(x => x.DeletedAt == null)
                       join b in db.Flavors.Where(x => x.DeletedAt == null && x.IsApproved == true && x.UserType == Flavor.CUSTOMER)
                       on a.Id equals b.UserId
                       where b.UserId == a.Id
                       select new UserPostRecipeViewModel
                       {
                           Name = a.Name,
                           Address = a.Address,
                           Email = a.Email,
                           Avatar = a.Avatar,
                           FlavorCreateAt = b.CreatedAt,
                           Phone = a.Phone,
                       };
            var userPostRecipe = user.OrderByDescending(x => x.FlavorCreateAt).Take(3).ToList();

            ViewBag.booksRaking = bookRaking;
            ViewBag.flavorsRaking = flavorsRaking;
            ViewBag.userPostRecipe = userPostRecipe;
            return View();
        }

        // GET: Contact
        public ActionResult Contact()
        {
            return View();
        }

        // GET: About
        public ActionResult About()
        {
            return View();
        }

        public PartialViewResult NavigationBar()
        {
            return PartialView("PartialView/NavigationBar");
        }

        public void GetFavourites()
        {
            if (Session["Customer"] != null)
            {
                var customerId = ((Customer)Session["Customer"]).Id;
                if (customerId != 0)
                {
                    ViewBag.favouriteBooks = db.Favourites.Where(x => x.ItemType == App.Models.Favourite.BOOKS && x.CustomerId == customerId && x.DeletedAt == null).ToList();
                    ViewBag.favouriteFlavors = db.Favourites.Where(x => x.ItemType == App.Models.Favourite.FLAVORS && x.CustomerId == customerId && x.DeletedAt == null).ToList();
                }
            }
        }
    }
}
