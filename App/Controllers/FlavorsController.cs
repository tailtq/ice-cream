using App.App_Start.Authorization;
using App.Helpers;
using App.Infrastructure.Controllers;
using App.Models;
using App.ViewModels.Admin.Flavor;
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Controllers
{
    public class FlavorsController : BaseController
    {
        // GET: Flavors
        IceCreamEntities db = new IceCreamEntities();

        // GET: Flavors
        public ActionResult Index(string search, int page = 1)
        {
            GetFavourites();
            int pageSize = 8;
            ViewBag.search = search;
            double totalRecord = 0;
            var flavors = db.Flavors.Where(x => x.DeletedAt == null && x.IsApproved == true).AsEnumerable();
            if (search != null)
            {
                flavors = flavors.Where(x => x.Name.ToLower().Contains(search.ToLower()));
                totalRecord = flavors.Count();
            }
            else
            {
                totalRecord = flavors.Count();
            }
            flavors = flavors.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();

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


            return View(flavors);
        }

        // GET: Flavors/Rank
        public ActionResult Rank()
        {
            return View();
        }

        // GET: Flavors/Details/5
        public ActionResult Details(int id)
        {
            if (((Customer)Session["Customer"]) == null)
            {
                return RedirectToAction("Login", "Customer");
            }
            if (((Customer)Session["Customer"]).MembershipStatus != 0)
            {
                GetFavourites();
                var flavor = db.Flavors.Where(x => x.Id == id).FirstOrDefault();
                if (flavor.UserType == 1)
                {
                    ViewBag.User = db.Staffs.Where(x => x.Id == flavor.UserId).FirstOrDefault();
                }
                else
                {
                    ViewBag.User = db.Customers.Where(x => x.Id == flavor.UserId).FirstOrDefault();
                }
                if (flavor == null)
                {
                    // check 404
                    return HttpNotFound();
                }
                else
                {
                    Random rd = new Random();
                    var listFlavors = db.Flavors.Where(x => x.DeletedAt == null && x.Id != id).ToList();
                    var relatedFlavors = listFlavors.Skip(rd.Next(listFlavors.Count - 6)).Take(5);
                    ViewBag.RelatedFlavors = relatedFlavors;
                    return View(flavor);
                }
            }
            return RedirectToAction("Login", "Customer");
        }

        [CustomerAuthorize]
        // GET: Flavors/Create
        public ActionResult Create()
        {
            if (((Customer)Session["Customer"]).MembershipStatus == 0)
            {
                return HttpNotFound();
            }
            return View();
        }

        // POST: Flavors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomerAuthorize]
        public ActionResult Create(HttpPostedFileBase avatarFile, FlavorViewModel flavorViewModel)
        {
            if (ModelState.IsValid)
            {
                Flavor flavor = new Flavor()
                {
                    UserId = ((Customer)Session["Customer"]).Id,
                    UserType = Flavor.CUSTOMER,
                    Name = flavorViewModel.Name,
                    Description = flavorViewModel.Description,
                    Images = SaveImages.SaveImagesFile(avatarFile, flavorViewModel.Name),
                    PreparationTime = flavorViewModel.PreparationTime,
                    TotalTime = flavorViewModel.TotalTime,
                    Ingredients = flavorViewModel.Ingredients,
                    Recipe = flavorViewModel.Recipe,
                    IsApproved = false,
                    Slug = Slugify.GenerateSlug(flavorViewModel.Name),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                Session["Image"] = "/" + ConfigurationManager.AppSettings["CusImages"] + flavor.Images;
                db.Flavors.Add(flavor);
                db.SaveChanges();
                return RedirectToAction("Flavors", "Customer");
            }

            return View();
        }

        // GET: Flavors/Edit/1
        [CustomerAuthorize]
        public ActionResult Edit(int id)
        {
            var customerId = ((Customer)Session["Customer"]).Id;
            var flavor = db.Flavors.Where(q => q.Id == id && q.UserId == customerId && q.UserType == Flavor.CUSTOMER)
                                   .FirstOrDefault();
            if (flavor == null)
            {
                return HttpNotFound();
            }

            FlavorViewModel flavorVM = new FlavorViewModel
            {
                Id = flavor.Id,
                Name = flavor.Name,
                Description = flavor.Description,
                Images = flavor.DecodedImages()[0],
                Ingredients = flavor.Ingredients,
                PreparationTime = flavor.PreparationTime,
                Recipe = flavor.Recipe,
                TotalTime = flavor.TotalTime
            };
            Session["Image"] = flavorVM.Images;
            return View(flavorVM);
        }

        // POST: Flavors/Edit/1
        [HttpPost]
        [CustomerAuthorize]
        public ActionResult Edit(HttpPostedFileBase avatarFile, FlavorViewModel flavorVM)
        {
            try
            {
                var flavor = db.Flavors.Find(flavorVM.Id);
                if (flavor.IsApproved == true)
                {
                    return RedirectToAction("Flavors", "Customer");
                }
                flavor.Name = flavorVM.Name;
                if (avatarFile != null)
                {
                    flavor.Images = SaveImages.SaveImagesFile(avatarFile, flavorVM.Name);
                }
                flavor.Description = flavorVM.Description;
                flavor.PreparationTime = flavorVM.PreparationTime;
                flavor.TotalTime = flavorVM.TotalTime;
                flavor.Ingredients = flavorVM.Ingredients;
                flavor.Slug = Slugify.GenerateSlug(flavor.Name);
                flavor.UpdatedAt = DateTime.Now;
                flavor.Recipe = flavorVM.Recipe;
                flavor.IsApproved = false;

                db.Entry(flavor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Flavors", "Customer");
            }
            catch
            {
                return View();
            }
        }

        // POST: Flavors/Favourite/1
        [HttpPost]
        [Authorize]
        public ActionResult Favourite(int? id, byte? itemType)
        {
            if (id == null || itemType == null)
            {
                return NotFound();
            }
            var customerId = ((Customer)Session["customer"]).Id;
            if (customerId == null)
            {
                return NotFound();
            }
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
                ViewBag.favourite = db.Favourites.Where(x => x.ItemType == App.Models.Favourite.FLAVORS && x.CustomerId == customerId && x.DeletedAt == null).ToList();
            }
        }
    }
}
