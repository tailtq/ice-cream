using App.App_Start;
using App.Helpers;
using App.Infrastructure.Controllers;
using App.Mappings;
using App.Models;
using App.Services;
using App.ViewModels.Admin.Flavor;
using App.ViewModels.Admin.Payout;
using PagedList;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Areas.Admin.Controllers
{
    [Authorize]
    public class FlavorsController : BaseController
    {
        IceCreamEntities db = new IceCreamEntities();

        PaypalPaymentService PaymentService = new PaypalPaymentService();

        // GET: Admin/Flavors
        public ActionResult Index(string search, int page = 1)
        {
            ViewBag.search = search;
            var query = db.Flavors.Where(x => x.DeletedAt == null).OrderByDescending(x => x.CreatedAt).AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
            }
            var result = query.ToPagedList(page, PAGE_SIZE);

            foreach (Flavor flavor in result)
            {
                if (flavor.UserType == Flavor.CUSTOMER)
                {
                    flavor.AuthorCustomer = db.Customers.Find(flavor.UserId);
                }
                else
                {
                    flavor.AuthorStaff = db.Staffs.Find(flavor.UserId);
                }
            }

            return View(result);
        }

        // GET: Admin/Flavors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Flavors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase avatarFile, FlavorViewModel flavorVM)
        {
            try
            {
                Flavor flavor = new Flavor()
                {
                    UserId = ((Staff)Session["Staff"]).Id,
                    UserType = Flavor.STAFF,
                    Name = flavorVM.Name,
                    Description = flavorVM.Description,
                    Images = SaveImages.SaveImagesFile(avatarFile, flavorVM.Name),
                    PreparationTime = flavorVM.PreparationTime,
                    TotalTime = flavorVM.TotalTime,
                    Ingredients = flavorVM.Ingredients,
                    Recipe = flavorVM.Recipe,
                    IsApproved = true,
                    Slug = Slugify.GenerateSlug(flavorVM.Name),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                Session["Image"] = "/" + ConfigurationManager.AppSettings["CusImages"] + flavor.Images;
                db.Flavors.Add(flavor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Flavors/Edit/5
        public ActionResult Edit(int id)
        {
            var flavor = db.Flavors.Where(x => x.DeletedAt == null && x.Id == id).FirstOrDefault();
            if (flavor == null)
            {
                return HttpNotFound();
            }
            var flavorVM = MappingProfile.mapper.Map<Flavor, FlavorViewModel>(flavor);
            flavorVM.Images = flavor.DecodedImages()[0];
            Session["Image"] = flavorVM.Images;
            return View(flavorVM);
        }

        // POST: Admin/Flavors/Edit/5
        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase avatarFile, FlavorViewModel flavorVM)
        {
            try
            {
                var flavor = db.Flavors.Find(flavorVM.Id);
                if (flavor.IsApproved == false)
                {
                    return RedirectToAction("Index");
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
                flavor.IsApproved = true;

                db.Entry(flavor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // POST: Admin/Flavors/Approve/5
        [HttpPost]
        public ActionResult Approve(ApproveFlavorViewModel model, int id)
        {
            try
            {
                var flavor = db.Flavors.Where(x => x.DeletedAt == null && x.Id == id).FirstOrDefault();
                if (flavor == null)
                {
                    return NotFound();
                }

                flavor.IsApproved = true;
                flavor.UpdatedAt = DateTime.Now;
                db.Entry(flavor).State = System.Data.Entity.EntityState.Modified;

                string message = "Thank you very much for your contribution.";
                var payout = db.Payouts.Add(new Models.Payout()
                {
                    FlavorId = flavor.Id,
                    CustomerId = flavor.UserId,
                    StaffId = ((Staff)Session["Staff"]).Id,
                    SumTotal = model.SumTotal,
                    Message = message,
                    CreatedAt = DateTime.Now
                });
                db.SaveChanges();

                var customer = db.Customers.Where(q => q.Id == flavor.UserId).First();
                PaymentService.SendPayout("Flavor is approved", new List<PayoutItem>()
                {
                    new PayoutItem()
                    {
                        recipient_type = (customer.CreditCard != null ? PayoutRecipientType.PAYPAL_ID : PayoutRecipientType.EMAIL),
                        amount = new Currency
                        {
                            value = model.SumTotal.ToString(),
                            currency = "USD"
                        },
                        receiver = customer.CreditCard ?? customer.Email,
                        note = message,
                        sender_item_id = "payout_" + payout.Id,
                    }
                });

                return Success("Ok");
            }
            catch (Exception e)
            {
                return InternalError(e.Message);
            }
        }

        // POST: Admin/Flavors/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var flavor = db.Flavors.Where(x => x.DeletedAt == null && x.Id == id).FirstOrDefault();
                if (flavor == null)
                {
                    return NotFound();
                }

                flavor.DeletedAt = DateTime.Now;
                flavor.DeletedBy = ((Staff)Session["Staff"]).Id;
                db.Entry(flavor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Success("Ok");
            }
            catch (Exception e)
            {
                return InternalError(e.Message);
            }
        }

        public ActionResult TestPayout()
        {
            var payout = new PayPal.Api.Payout()
            {
                sender_batch_header = new PayoutSenderBatchHeader
                {
                    sender_batch_id = "batch_" + System.Guid.NewGuid().ToString().Substring(0, 8),
                    email_subject = "You have a payment"
                },

                items = new List<PayoutItem>
                {
                    new PayoutItem
                    {
                        recipient_type = PayoutRecipientType.EMAIL,
                        amount = new Currency
                        {
                            value = "0.99",
                            currency = "USD"
                        },
                        receiver = "shirt-supplier-one@mail.com",
                        note = "Thank you.",
                        sender_item_id = "item_1"
                    },
                    new PayoutItem
                    {
                        recipient_type = PayoutRecipientType.EMAIL,
                        amount = new Currency
                        {
                            value = "0.90",
                            currency = "USD"
                        },
                        receiver = "shirt-supplier-two@mail.com",
                        note = "Thank you.",
                        sender_item_id = "item_2"
                    },
                    new PayoutItem
                    {
                        recipient_type = PayoutRecipientType.EMAIL,
                        amount = new Currency
                        {
                            value = "2.00",
                            currency = "USD"
                        },
                        receiver = "4532216992026816",
                        note = "Thank you.",
                        sender_item_id = "item_3"
                    }
                }
            };
            var createdPayout = payout.Create(PaypalConfig.GetAPIContext(), false);

            return Success(1);
        }
    }
}
