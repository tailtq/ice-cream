using System;
using PagedList;
using App.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using App.Infrastructure.Controllers;

namespace App.Areas.Admin.Controllers
{
    [Authorize]
    public class CustomersController : BaseController
    {
        IceCreamEntities db = new IceCreamEntities();

        // GET: Admin/Customers
        public ActionResult Index(string search, int page = 1, int pageSize = 10)
        {
            var query = db.Customers.OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
            }

            return View(query.ToPagedList(page, pageSize));
        }

        public ActionResult Detail(int id)
        {
            var customer = db.Customers.Find(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // POST: Admin/Customers/Block/5
        [HttpPost]
        public ActionResult Block(int id, FormCollection collection)
        {
            try
            {
                var customer = db.Customers.Find(id);

                if (customer == null)
                {
                    return NotFound();
                }
                if (customer.DeletedAt == null)
                {
                    customer.DeletedAt = DateTime.Now;
                    customer.DeletedBy = ((Staff)Session["Staff"]).Id;
                }
                else
                {
                    customer.DeletedAt = null;
                    customer.DeletedBy = null;
                }

                customer.UpdatedAt = DateTime.Now;
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();

                return Success(customer.DeletedAt != null);
            }
            catch (Exception e)
            {
                return InternalError(e.Message);
            }
        }

        // POST: Admin/Customers/Export
        public ActionResult Export()
        {
            return RedirectToAction("Index");
        }
    }
}
