using App.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using App.Infrastructure.Controllers;
using System.Data.Entity;

namespace App.Areas.Admin.Controllers
{
    [Authorize]
    public class FeedbacksController : BaseController
    {
        IceCreamEntities db = new IceCreamEntities();

        // GET: Admin/Feedbacks
        public ActionResult Index(string search, int page = 1, int pageSize = 10)
        {
            ViewBag.search = search;
            var query = db.Feedbacks.OrderBy(x => x.IsRead)
                                    .ThenByDescending(x => x.CreatedAt)
                                    .Where(x => x.DeletedAt == null);
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()) || x.Email.ToLower().Contains(search.ToLower()));
            }

            return View(query.ToPagedList(page, pageSize));
        }

        // POST: Admin/Feedbacks/Read/5
        [HttpPost]
        public ActionResult Read(int id)
        {
            try
            {
                var feedback = db.Feedbacks.Where(x => x.DeletedAt == null && x.Id == id).FirstOrDefault();

                if (feedback == null)
                {
                    return NotFound();
                }

                feedback.IsRead = true;
                feedback.Staff = db.Staffs.Find(((Staff)Session["Staff"]).Id);
                feedback.UpdatedAt = DateTime.Now;
                db.Entry(feedback).State = EntityState.Modified;
                db.SaveChanges();

                return Success("OK");
            }
            catch(Exception e)
            {
                return InternalError(e.ToString());
            }
        }

        // POST: Admin/Feedbacks/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                Feedback feedback = db.Feedbacks.Where(x => x.Id == id && x.DeletedAt == null && x.IsRead == false).FirstOrDefault();

                if (feedback == null)
                {
                    return NotFound();
                }

                feedback.DeletedAt = DateTime.Now;
                feedback.DeletedBy = ((Staff)Session["Staff"]).Id;
                db.Entry(feedback).State = EntityState.Modified;
                db.SaveChanges();

                return Success("OK");
            }
            catch(Exception e)
            {
                return InternalError(e.ToString());
            }
        }

        // POST: Admin/Staffs/Export
        public ActionResult Export()
        {
            return RedirectToAction("Index");
        }
    }
}
