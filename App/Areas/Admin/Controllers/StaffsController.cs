using App.Helpers;
using App.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Diagnostics;
using System.Data.Entity;
using System.Net;
using AutoMapper;
using System.Data.Entity.Validation;
using System;
using App.Mappings;
using App.ViewModels.Admin.Staff;
using System.Configuration;
using System.Web;
using PagedList;

namespace App.Areas.Admin.Controllers
{
    [Authorize]
    public class StaffsController : Controller
    {
        IceCreamEntities db = new IceCreamEntities();

        // GET: Admin/Staffs
        public ActionResult Index(string search, int page = 1, int pageSize = 10)
        {
            ViewBag.search = search;
            var query = db.Staffs.OrderByDescending(x => x.CreatedAt).AsEnumerable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()) || x.Email.Contains(search.ToLower()));
            }

            return View(query.ToPagedList(page, pageSize));
        }

        // GET: Admin/Staffs/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Session["Staff"] != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        // GET: Admin/Staffs/Login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel viewModel, string returnUrl)
        {
            Staff staff = db.Staffs.Where(q => q.Email == viewModel.Email).FirstOrDefault();

            if (staff == null)
            {
                ViewBag.LoginStatus = "Email does not exist";
                return View();
            }
            else
            {
                if (!Hashing.ValidatePassword(viewModel.Password, staff.Password))
                {
                    ViewBag.LoginStatus = "Password is not correct";
                    return View();
                }
                else if (staff.DeletedAt != null)
                {
                    ViewBag.LoginStatus = "Account locked";
                    return View();
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(staff.Id.ToString(), false);
                    Session["Staff"] = staff;
                    Session["StaffName"] = staff.Name;
                    Session["Avatar"] = "/" + ConfigurationManager.AppSettings["CusAvatar"] + staff.Avatar;
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                }
            }

            return RedirectToAction("Index", "Dashboard");
        }

        // GET: Admin/Staffs/Create
        public ActionResult Create()
        {
            SetLevel();
            return View();
        }

        // POST: Admin/Staffs/Create
        [HttpPost]
        public ActionResult Create(StaffViewModel viewModel)
        {
            SetLevel();
            if (ModelState.IsValid)
            {
                var check = db.Staffs.Where(x => x.Email == viewModel.Email).FirstOrDefault();
                if (check == null)
                {
                    Staff staff = new Staff()
                    {
                        Name = viewModel.Name,
                        Password = Hashing.HashPassword(viewModel.Password),
                        Email = viewModel.Email,
                        Address = viewModel.Address,
                        Phone = viewModel.Phone,
                        Avatar = "default-avatar.png",
                        LevelId = viewModel.LevelId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    db.Staffs.Add(staff);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.CheckStatus = "Email already exists";
                }
            }
            return View();
        }

        /*
         * Le Trinh Quoc Tai
         */
        // GET: Admin/Staffs/Me
        public ActionResult Me()
        {
            Staff session = (Staff)Session["Staff"];
            Staff staff = db.Staffs.Where(q => q.Id == session.Id && q.DeletedAt == null).First();
            var update = MappingProfile.mapper.Map<Staff, UpdateProfileViewModel>(staff);
            return View(update);
        }

        /*
         * Le Trinh Quoc Tai
         */
        // POST: Admin/Staffs/Me
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Me(HttpPostedFileBase avatarFile, UpdateProfileViewModel input)
        {
            int staffId = ((Staff)Session["Staff"]).Id;
            if (staffId != input.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var staff = db.Staffs.Where(q => q.Id == staffId).FirstOrDefault();

                staff.Id = input.Id;
                staff.Name = input.Name;
                staff.Address = input.Address;
                staff.Phone = input.Phone;
                if (avatarFile != null)
                {
                    staff.Avatar = SaveImages.SaveAvatarFile(avatarFile, staff.Email);
                }

                Session["Avatar"] = "/" + ConfigurationManager.AppSettings["CusAvatar"] + staff.Avatar;
                db.Entry(staff).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Me");
        }

        // GET: Admin/Staffs/ChangePassword
        public ActionResult ChangePassword()
        {
            int staffId = ((Staff)Session["Staff"]).Id;
            ViewBag.OldPassword = TempData["OldPassword"]?.ToString();
            ViewBag.Phone = ((Staff)Session["Staff"]).Phone;
            ViewBag.CreateAt = ((Staff)Session["Staff"]).CreatedAt;
            ChangePasswordViewmodel viewmodel = new ChangePasswordViewmodel();
            viewmodel.Id = staffId;
            return View(viewmodel);
        }

        // POST: Admin/Staffs/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewmodel model)
        {
            if (ModelState.IsValid)
            {
                var staff = db.Staffs.Where(x => x.Id == model.Id).FirstOrDefault();
                if (!Hashing.ValidatePassword(model.OldPassword, staff.Password))
                {
                    TempData["OldPassword"] = "Wrong password please try again";
                    return RedirectToAction("ChangePassword");
                }
                else
                {
                    staff.Password = Hashing.HashPassword(model.ConfirmNewPassword);
                    db.Entry(staff).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            return RedirectToAction("Me");
        }

        // GET: Admin/Staffs/Edit/5
        public ActionResult Edit(int id)
        {
            var staff = db.Staffs.Where(x => x.Id == id).FirstOrDefault();
            SetLevel(staff.LevelId);
            var staffVM = MappingProfile.mapper.Map<Staff, UpdateStaffViewModel>(staff);
            return View(staffVM);
        }

        // POST: Admin/Staffs/Edit/5
        [HttpPost]
        public ActionResult Edit(UpdateStaffViewModel viewModel)
        {
            SetLevel(viewModel.LevelId);

            var staff = db.Staffs.Where(x => x.Id == viewModel.Id).FirstOrDefault();
            staff.Name = viewModel.Name;
            staff.Address = viewModel.Address;
            staff.Phone = viewModel.Phone;
            staff.LevelId = viewModel.LevelId;
            staff.UpdatedAt = DateTime.Now;
            db.Entry(staff).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Admin/Staffs/Block/5
        public ActionResult Block(int id)
        {
            var staff = db.Staffs.Where(x => x.Id == id).FirstOrDefault();
            if (staff.DeletedAt == null)
            {
                staff.DeletedAt = DateTime.Now;
            }
            else
            {
                staff.DeletedAt = null;
            }
            db.Entry(staff).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Admin/Staffs/Block/5
        [HttpPost]
        public ActionResult Block(int id, FormCollection collection)
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

        // POST: Admin/Staffs/Export
        public ActionResult Export()
        {
            return RedirectToAction("Index");
        }

        public void SetLevel(int? selectedItem = null)
        {
            var level = db.Levels.ToList();
            ViewBag.level = new SelectList(level, "Id", "Name", selectedItem);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["Staff"] = null;

            return RedirectToAction("Login");
        }
    }
}
