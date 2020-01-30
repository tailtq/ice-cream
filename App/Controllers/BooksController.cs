using App.Infrastructure.Controllers;
using App.Models;
using App.ViewModels.Client.Home;
using System;
using System.Linq;
using System.Web.Mvc;

namespace App.Controllers
{
    public class BooksController : BaseController
    {
        IceCreamEntities db = new IceCreamEntities();


        // GET: Books
        public ActionResult Index(int? catBook, int page = 1)
        {
            GetFavourites();

            int pageSize = 8;
            ViewBag.catBook = catBook;
            double totalRecord = 0;
            var books = db.Books.Where(x => x.DeletedAt == null).AsEnumerable();
            if (catBook != null)
            {
                var category = db.BookCategories.Find(catBook);
                ViewBag.category = category;
                books = books.Where(x => x.CategoryId == catBook);
                totalRecord = db.Books.Where(x => x.CategoryId == catBook).Count();
            }
            else
            {
                totalRecord = db.Books.Where(x => x.DeletedAt == null).Count();
            }
            books = books.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Page = page;
            ViewBag.totalRecord = totalRecord;

            int maxPage = 4;
            int totalPage = 0;
            totalPage = (int)Math.Ceiling(totalRecord / pageSize);
            ViewBag.TotalPage = totalPage;
            ViewBag.MaxPage = maxPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;

            return View(books);
        }

        // GET: Search/keyword=...
        public ActionResult Search(string search, int page = 1)
        {
            GetFavourites();

            ViewBag.searchKey = search;
            double totalRecord = 0;
            var books = db.Books.Where(x => x.DeletedAt == null).AsEnumerable();

            books = books.Where(x => x.Name.ToLower().Contains(search.ToLower()));

            books = books.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();

            totalRecord = db.Books.Where(x => x.Name.ToLower().Contains(search.ToLower()) && x.DeletedAt == null).Count();

            ViewBag.Page = page;
            ViewBag.totalRecord = totalRecord;

            int maxPage = 4;
            int totalPage = 0;
            totalPage = (int)Math.Ceiling(totalRecord / PAGE_SIZE);
            ViewBag.TotalPage = totalPage;
            ViewBag.MaxPage = maxPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;

            return View(books);
        }

        public JsonResult SearchAuto(string text)
        {
            var books = db.Books.Where(p => p.Name.Contains(text)).Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.Images
            }).Take(5).ToList();
            return Json(books, JsonRequestBehavior.AllowGet);
        }

        // GET
        public ActionResult Rank()
        {
            //var bookRaking = db.
            return View();
        }

        // GET: Books/Details/5
        public ActionResult Detail(int id)
        {
            GetFavourites();

            var book = db.Books.Where(x => x.Id == id).FirstOrDefault();
            if (book == null)
            {
                // check 404
                return HttpNotFound();
            }
            else
            {
                Random rd = new Random();
                var listBook = db.Books.Where(x => x.DeletedAt == null && x.Id != id).ToList();
                var relatedBook = listBook.Skip(rd.Next(listBook.Count - 6)).Take(5);
                ViewBag.RelatedBook = relatedBook;
                return View(book);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Favourite(int? id, byte? itemType)
        {
            if (id == null || itemType == null)
            {
                return NotFound();
            }
            var customerId = ((Customer)Session["customer"]).Id;
            var favouriteResult = db.Favourites.Where(x => x.CustomerId == customerId && x.ItemId == id && x.ItemType == itemType).FirstOrDefault();
            if (favouriteResult == null)
            {
                db.Favourites.Add(new Favourite
                {
                    CustomerId = customerId,
                    ItemId = int.Parse(id.ToString()),
                    ItemType = byte.Parse(itemType.ToString()),
                    CreatedAt = DateTime.Now
                });
                db.SaveChanges();
                return Success("1");
            }
            else if (favouriteResult.DeletedAt != null)
            {
                favouriteResult.DeletedAt = null;
                db.Entry(favouriteResult).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Success("1");
            }
            else if (favouriteResult.DeletedAt == null)
            {
                favouriteResult.DeletedAt = DateTime.Now;
                db.Entry(favouriteResult).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Success("0");
            }
            return Success("0");
        }

        public void GetFavourites()
        {
            if (Session["Customer"] != null)
            {
                var customerId = ((Customer)Session["Customer"]).Id;
                if (customerId != 0)
                {
                    ViewBag.favourite = db.Favourites.Where(x => x.ItemType == App.Models.Favourite.BOOKS && x.CustomerId == customerId && x.DeletedAt == null).ToList();
                }
            }
        }
    }
}
