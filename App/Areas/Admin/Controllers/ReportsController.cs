using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Areas.Admin.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Admin/Reports
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Reports/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Reports/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Reports/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
