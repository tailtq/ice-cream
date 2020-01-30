using App.Models;
using System.Linq;
using System.Web.Mvc;

namespace App.Controllers
{
    public class BookCategoriesController : Controller
    {
        IceCreamEntities db = new IceCreamEntities();

        // GET: BookCategories
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult Navbar()
        {
            var model = db.BookCategories.Where(x => x.DeletedAt == null).ToList();
            return PartialView("/Views/Shared/PartialView/NavigationBar.cshtml", model);
        }
    }
}