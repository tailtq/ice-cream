using App.Helpers;
using App.Infrastructure.Controllers;
using App.Mappings;
using App.Models;
using App.ViewModels.Admin.Book;
using PagedList;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Areas.Admin.Controllers
{
    [Authorize]
    public class BooksController : BaseController
    {
        IceCreamEntities db = new IceCreamEntities();
        // GET: Admin/Books
        public ActionResult Index(string search, int? searchCategory, int page = 1)
        {
            SetViewBag(searchCategory);
            var query = db.Books.AsEnumerable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
            }
            if (searchCategory.HasValue)
            {
                query = query.Where(x => x.CategoryId == searchCategory);
            }
            query = query.Where(x => x.DeletedAt == null).OrderByDescending(x => x.CreatedAt).ToPagedList(page, PAGE_SIZE);
            ViewBag.search = search;
            ViewBag.searchCat = searchCategory;
            return View(query);
        }

        // GET: Admin/Books/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Books/Create
        public ActionResult Create()
        {
            SetViewBag();
            return View();
        }

        // POST: Admin/Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase avatarFile, BookViewModel viewModel)
        {
            SetViewBag();
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var book = MappingProfile.mapper.Map<BookViewModel, Book>(viewModel);
                book.StaffId = ((Staff)Session["Staff"]).Id;
                book.Slug = Slugify.GenerateSlug(book.Name);
                book.CreatedAt = DateTime.Now;
                book.UpdatedAt = DateTime.Now;
                book.Sku = rd.Next(1000, 9999).ToString();
                book.Images = SaveImages.SaveImagesFile(avatarFile, viewModel.Name);
                db.Books.Add(book);
                db.SaveChanges();
            };
            return RedirectToAction("Index");
        }

        // GET: Admin/Books/Edit/5
        public ActionResult Edit(int id)
        {
            var book = db.Books.Where(x => x.Id == id).FirstOrDefault();
            SetViewBag(book.CategoryId);
            var bookVM = MappingProfile.mapper.Map<Book, BookViewModel>(book);
            bookVM.Images = book.DecodedImages()[0];
            Session["Image"] = bookVM.Images;
            return View(bookVM);
        }

        // POST: Admin/Books/Edit/5
        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase avatarFile, BookViewModel viewModel)
        {
            SetViewBag(viewModel.CategoryId);
            if (ModelState.IsValid)
            {
                var book = db.Books.Where(x => x.Id == viewModel.Id).FirstOrDefault();
                book.Name = viewModel.Name;
                book.Price = viewModel.Price;
                book.Slug = Slugify.GenerateSlug(viewModel.Name);
                book.CategoryId = viewModel.CategoryId;
                book.Images = SaveImages.SaveImagesFile(avatarFile, viewModel.Name);
                book.Description = viewModel.Description;
                book.Discount = viewModel.Discount;
                book.UpdatedAt = DateTime.Now;
                db.Entry(book).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Admin/Books/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var book = db.Books.Where(x => x.Id == id).FirstOrDefault();
                if (book == null)
                {
                    return NotFound();
                }
                book.DeletedAt = DateTime.Now;
                book.DeletedBy = ((Staff)Session["Staff"]).Id;
                db.Entry(book).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Success("Ok");
            }
            catch (Exception e)
            {

                return InternalError(e.Message);
            }
        }

        public void SetViewBag(int? selectedId = null)
        {
            var CategoryId = db.BookCategories.Where(x => x.DeletedAt == null).ToList();
            ViewBag.CategoryId = new SelectList(CategoryId, "Id", "Name", selectedId);
        }
    }
}
