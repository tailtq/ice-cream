using App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;

namespace App.Controllers
{
    public class FeedbacksController : Controller
    {
        IceCreamEntities db = new IceCreamEntities();

        public ActionResult Index()
        {
            return View();
        }

        // POST: Feedbacks/Create
        [HttpPost]
        public ActionResult Create(Feedback feedback)
        {
            if (!this.IsCaptchaValid(""))
            {
                ViewBag.captchaError = "Invalid Captcha";
                return View("Index", feedback);

            }
            feedback.CreatedAt = DateTime.Now;
            feedback.UpdatedAt = DateTime.Now;
            db.Feedbacks.Add(feedback);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}
