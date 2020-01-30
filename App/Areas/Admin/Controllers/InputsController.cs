using App.Models;
using App.ViewModels.Admin.Book;
using App.ViewModels.Admin.Input;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace App.Areas.Admin.Controllers
{
    public class InputsController : Controller
    {
        IceCreamEntities db = new IceCreamEntities();
        // GET: Admin/Inputs
        public ActionResult Index(string search, int page = 1, int pageSize = 10)
        {
            ViewBag.search = search;
            var query = db.Inputs.OrderByDescending(x => x.CreatedAt).AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Code.Contains(search) || x.Supplier.Name.Contains(search));
            }

            return View(query.ToPagedList(page, pageSize));
        }

        // GET: Admin/Inputs/Details/5
        public ActionResult Details(int id, int page = 1, int pageSize = 10)
        {
            var inputdetails = db.InputDetails.Where(x => x.Id == id).OrderByDescending(x => x.CreatedAt).ToPagedList(page, pageSize);

            return View(inputdetails);
        }

        // GET: Admin/Inputs/Create
        public ActionResult Create()
        {
            ViewBag.suppliers = db.Suppliers.Where(q => q.DeletedAt == null);
            ViewBag.books = db.Books.Where(q => q.DeletedAt == null)
                                    .Select(q => new InputDetailViewModel() { Id = q.Id, Sku = q.Sku });

            return View();
        }

        // POST: Admin/Inputs/Create
        [HttpPost]
        public ActionResult Create(InputViewModel model)
        {
            if (ModelState.IsValid)
            {
                var input = db.Inputs.Add(new Input()
                {
                    StaffId = ((Staff)Session["Staff"]).Id,
                    SupplierId = model.SupplierId,
                    Code = model.Code,
                    ImportedAt = model.ImportedAt,
                    CreatedAt = DateTime.Now
                });
                foreach (var detail in model.InputDetails)
                {
                    var inputDetail = db.InputDetails.Add(new InputDetail()
                    {
                        InputId = input.Id,
                        BookId = detail.BookId,
                        Price = detail.Price,
                        Quantity = detail.Quantity,
                        CreatedAt = DateTime.Now
                    });
                    db.InputQuantities.Add(new InputQuantity()
                    {
                        InputDetailId = inputDetail.Id,
                        RemainQuantity = detail.Quantity
                    });
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.suppliers = db.Suppliers.Where(q => q.DeletedAt == null);
            ViewBag.books = db.Books.Where(q => q.DeletedAt == null)
                                    .Select(q => new InputDetailViewModel() { Id = q.Id, Sku = q.Sku });

            return View();
        }
    }
}
