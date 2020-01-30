using App.Helpers;
using App.Infrastructure.Controllers;
using App.Models;
using App.ViewModels.Admin.Level;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Areas.Admin.Controllers
{
    [Authorize]
    public class LevelsController : BaseController
    {
        IceCreamEntities db = new IceCreamEntities();

        // GET: Admin/Levels
        public ActionResult Index(string search, int page = 1, int pageSize = 10)
        {
            ViewBag.search = search;
            var query = db.Levels.OrderByDescending(x => x.CreatedAt).Where(q => q.Key != "owner");

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
            }

            return View(query.ToPagedList(page, pageSize));
        }

        // GET: Admin/Levels/Create
        public ActionResult Create()
        {
            ViewBag.permissions = db.Permissions.ToList();

            return View();
        }

        // POST: Admin/Levels/Create
        [HttpPost]
        public ActionResult Create(LevelViewModel model)
        {
            try
            {
                db.Levels.Add(new Level()
                {
                    Name = model.Name,
                    Key = Slugify.GenerateSlug(model.Name),
                    Permissions = db.Permissions.Where(q => model.Permissions.Contains(q.Id)).ToList(),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Create");
            }
        }

        // GET: Admin/Levels/Edit/5
        public ActionResult Edit(int id)
        {
            var level = db.Levels.Where(q => q.Id == id).FirstOrDefault();

            if (level == null)
            {
                return HttpNotFound();
            }

            ViewBag.currentPermissions = level.Permissions.Select(q => q.Id).ToList();
            ViewBag.permissions = db.Permissions.ToList();

            return View(level);
        }

        // POST: Admin/Levels/Edit/5
        [HttpPost]
        public ActionResult Edit(LevelViewModel model, int id)
        {
            var level = db.Levels.Find(id);

            if (level == null)
            {
                return HttpNotFound();
            }
            foreach (Permission permission in level.Permissions.ToList())
            {
                level.Permissions.Remove(permission);
            }

            level.Name = model.Name;
            level.Key = Slugify.GenerateSlug(model.Name);
            level.UpdatedAt = DateTime.Now;
            level.Permissions = db.Permissions.Where(q => model.Permissions.Contains(q.Id)).ToList();

            db.Entry(level).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: Levels/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (db.Staffs.Where(q => q.LevelId == id).Count() > 0)
            {
                return BadRequest();
            }

            Level level = db.Levels.Find(id);
            if (level == null)
            {
                return NotFound();
            }
            foreach (Permission permission in level.Permissions.ToList())
            {
                level.Permissions.Remove(permission);
            }
            db.Levels.Remove(level);
            db.SaveChanges();

            return Success("OK");
        }
    }
}
