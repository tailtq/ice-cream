using App.Helpers;
using App.Mappings;
using App.Models;
using App.ViewModels.Admin.Book;
using App.ViewModels.Admin.BookCategory;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Areas.Admin.Controllers
{
    [Authorize]
    public class BookCategoriesController : Controller
    {
        IceCreamEntities db = new IceCreamEntities();
        // GET: Admin/BookCategories
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var model = db.BookCategories.Where(x => x.DeletedAt == null).OrderByDescending(x => x.CreatedAt).ToPagedList(page, pageSize);
            return View(model);
        }

        // GET: Admin/BookCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/BookCategories/Create
        [HttpPost]
        public ActionResult Create(BookCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var check = db.BookCategories.Where(x => x.Name == model.Name).FirstOrDefault();
                if (check == null)
                {
                    BookCategory bookCategory = new BookCategory()
                    {
                        Name = model.Name,
                        Slug = Slugify.GenerateSlug(model.Name),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    db.BookCategories.Add(bookCategory);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.statusCheck = "Name already exists";

                }
            }
            return View();
        }

        // GET: Admin/BookCategories/Edit/5
        public ActionResult Edit(int id)
        {
            var bookCat = db.BookCategories.Where(x => x.Id == id).FirstOrDefault();
            var bookCatVM = MappingProfile.mapper.Map<BookCategory, BookCategoryViewModel>(bookCat);
            return View(bookCatVM);
        }

        // POST: Admin/BookCategories/Edit/5
        [HttpPost]
        public ActionResult Edit(BookCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {

                var check = db.BookCategories.Where(x => x.Name == model.Name).FirstOrDefault();
                if (check == null)
                {
                    var category = db.BookCategories.Where(x => x.Id == model.Id).FirstOrDefault();
                    category.Name = model.Name;
                    category.Slug = Slugify.GenerateSlug(model.Name);
                    category.UpdatedAt = DateTime.Now;
                    db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.statusCheck = "Name already exists";
                }
            }
            return View();
        }

        // GET: Admin/BookCategories/Delete/5
        public ActionResult Delete(int id)
        {
            var category = db.BookCategories.Where(x => x.Id == id).FirstOrDefault();
            category.DeletedAt = DateTime.Now;
            category.DeletedBy = ((Staff)Session["Staff"]).Id;
            db.Entry(category).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Admin/BookCategories/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
