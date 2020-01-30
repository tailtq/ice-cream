using App.App_Start.Authorization;
using App.Helpers;
using App.Mappings;
using App.Models;
using App.ViewModels.Admin.Staff;
using App.ViewModels.Client.Customer;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Net;
using App.Services;
using System.Collections.Generic;
using PayPal.Api;
using App.Infrastructure.Controllers;
using App.ViewModels.Client.Order;
using App.App_Start;

namespace App.Controllers
{
    [CustomerAuthorize]
    public class CustomerController : BaseController
    {
        IceCreamEntities db = new IceCreamEntities();

        PaypalPaymentService PaymentService = new PaypalPaymentService();

        // GET: Admin/Staffs/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Session["Customer"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // GET: Admin/Staffs/Login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Customer customer = db.Customers.Where(q => q.Email == model.Email).FirstOrDefault();
                if (customer == null || !Hashing.ValidatePassword(model.Password, customer.Password))
                {
                    ModelState.AddModelError("Email", "Email or password is not correct");
                    return View(model);
                }
                else if (customer.DeletedAt != null)
                {
                    ModelState.AddModelError("Email", "Account locked");
                    return View(model);
                }

                FormsAuthentication.SetAuthCookie(customer.Id.ToString(), false);
                Session["Customer"] = customer;

                if (returnUrl != null)
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // GET: Customer/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Customer/Register
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (db.Customers.Where(q => q.Email == model.Email).FirstOrDefault() != null)
                {
                    ModelState.AddModelError("Email", "Email existed");
                    return View(model);
                }

                var customer = db.Customers.Add(new Customer()
                {
                    Name = model.Name,
                    Password = Hashing.HashPassword(model.Password),
                    Email = model.Email,
                    Address = model.Address,
                    Phone = model.Phone,
                    Avatar = "/Content/Client/img/others/blank-avatar.png",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                });
                db.SaveChanges();
                FormsAuthentication.SetAuthCookie(customer.Id.ToString(), false);
                Session["Customer"] = customer;

                return RedirectToAction("Home", "Books");
            }

            return View(model);
        }

        // GET: Customer/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Customer/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [Obsolete]
        public ActionResult ForgotPassword(CustomerForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = db.Customers.Where(q => q.Email == model.Email).FirstOrDefault();
                if (customer == null)
                {
                    ModelState.AddModelError("Email", "Email is not correct");
                    return View(model);
                }
                else if (customer.DeletedAt != null)
                {
                    ModelState.AddModelError("Email", "Account locked");
                    return View(model);
                }

                customer.ResetPasswordToken = CommonHelper.GenerateToken(6);
                while (db.Customers.Where(q => q.ResetPasswordToken == customer.ResetPasswordToken && q.DeletedAt == null).FirstOrDefault() != null)
                {
                    customer.ResetPasswordToken = CommonHelper.GenerateToken(6);
                }

                customer.TokenExipredAt = DateTime.Now.AddMinutes(30);
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();

                model.Url = CommonHelper.GetBaseUrl() + Url.Action("ResetPassword") + "?token=" + customer.ResetPasswordToken;
                EmailHandler.Handle(model, model.Email, "Reset Password", "Views/Customer/Template/ForgotPasswordTemplate.cshtml");

                return Redirect(Request.UrlReferrer.ToString());
            }
                
            return View(model);
        }

        // GET: Customer/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string token)
        {
            var customer = db.Customers.Where(q => q.ResetPasswordToken == token && q.DeletedAt == null).FirstOrDefault();
            if (customer == null || customer.TokenExipredAt == null)
            {
                return HttpNotFound();
            }
            else if (DateTime.Now.CompareTo(customer.TokenExipredAt) > 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }

        // POST: Customer/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(CustomerResetPasswordViewModel model, string token)
        {
            if (ModelState.IsValid)
            {
                var customer = db.Customers.Where(q => q.ResetPasswordToken == token && q.DeletedAt == null).FirstOrDefault();
                if (customer == null || customer.TokenExipredAt == null)
                {
                    return HttpNotFound();
                }
                else if (DateTime.Now.CompareTo(customer.TokenExipredAt) > 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                customer.Password = Hashing.HashPassword(model.NewPassword);
                customer.ResetPasswordToken = null;
                customer.TokenExipredAt = null;
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(model);
        }

        // GET: Customer/Detail
        public ActionResult Detail()
        {
            Customer customer = (Customer)Session["Customer"];
            customer = db.Customers.Find(customer?.Id);
            var model = MappingProfile.mapper.Map<Customer, CustomerUpdateProfileViewModel>(customer);

            return View(model);
        }

        // POST: Customer/Detail
        [HttpPost]
        public ActionResult Detail(CustomerUpdateProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                Customer customer = (Customer)Session["Customer"];
                customer = db.Customers.Find(customer.Id);
                customer.Name = model.Name;
                customer.Phone = model.Phone;
                customer.Address = model.Address;
                customer.CreditCard = model.CreditCard;
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                Session["Customer"] = customer;

                return Redirect(Request.UrlReferrer.ToString());
            }
            return View(model);
        }

        // GET: Customer/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: Customer/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(CustomerChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                Customer customer = (Customer)Session["Customer"];
                customer = db.Customers.Where(x => x.Id == customer.Id).FirstOrDefault();
                if (!Hashing.ValidatePassword(model.OldPassword, customer.Password))
                {
                    ModelState.AddModelError("OldPassword", "Wrong password please try again");
                    return View(model);
                }

                customer.Password = Hashing.HashPassword(model.NewPassword);
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();

                return Redirect(Request.UrlReferrer.ToString());
            }
            return View(model);
        }

        // GET: Customer/Membership
        public ActionResult Membership()
        {
            var customer = (Customer)Session["Customer"];
            customer = db.Customers.Find(customer.Id);

            return View(customer);
        }

        // POST: Customer/Membership
        [HttpPost]
        public ActionResult Membership(RegisterMembershipViewModel model)
        {
            var customer = (Customer)Session["Customer"];
            if (customer.MembershipStatus != 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (ModelState.IsValid)
            {
                var paymentParam = new PaypalPaymentService.PaypalPaymentParam()
                {
                    Session = Session,
                    CallbackUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/Customer/PaymentCallback",
                    Items = new List<Item>()
                    {
                        new Item()
                        {
                            name = (model.MembershipType == CommonConstants.MONTHLY) ? "Monthly membership" : "Annual membership",
                            currency = "USD",
                            price = ((model.MembershipType == CommonConstants.MONTHLY) ? CommonConstants.MONTLY_TOTAL : CommonConstants.ANNUAL_TOTAL).ToString(),
                            quantity = "1",
                        }
                    },
                    CreditCard = model.CreditCard,
                };

                if (model.PaymentMethod == "paypal")
                {
                    string url = PaymentService.SendPayment(paymentParam);
                    if (url == "NG")
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                    return Redirect(url);
                }
                else
                {
                    var result = PaymentService.SendPaymentByCreditCard(paymentParam);

                    if (result != -1)
                    {
                        int customerId = ((Customer)Session["Customer"]).Id;
                        customer = RegisterMembership(customerId, result);
                        Session["Customer"] = customer;
                        // notify.....

                        return RedirectToAction("Membership");
                    }
                }
            }
            customer = db.Customers.Find(customer.Id);

            return View(customer);
        }

        // GET: Customer/PaymentCallback
        public ActionResult PaymentCallback()
        {
            var paymentParam = new PaypalPaymentService.ExecutePaymentParam()
            {
                Session = Session,
                PayerId = Request.Params["PayerID"],
                Guid = Request.Params["guid"],

            };
            float result = PaymentService.ExecutePayment(paymentParam);
            if (result == -1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }
            int customerId = ((Customer)Session["Customer"]).Id;
            var customer = RegisterMembership(customerId, result);
            Session["Customer"] = customer;
            // notify.....

            return RedirectToAction("Membership");
        }

        private Customer RegisterMembership(int customerId, float result)
        {
            var registration = db.MembershipRegistrations.Add(new MembershipRegistration()
            {
                CustomerId = customerId,
                PaymentSum = (decimal)result,
                MembershipType = (byte)(result == CommonConstants.ANNUAL_TOTAL ? CommonConstants.ANNUAL : CommonConstants.MONTHLY),
                CreatedAt = DateTime.Now
            });
            Customer customer = db.Customers.Find(customerId);
            customer.MembershipStatus = registration.MembershipType;
            db.Entry(customer).State = EntityState.Modified;
            db.SaveChanges();

            return customer;
        }

        // GET: Customer/Flavors
        public ActionResult Flavors()
        {
            int? customerId = ((Customer)Session["Customer"])?.Id;
            var flavors = db.Flavors.Where(q => q.UserId == customerId && q.UserType == Flavor.CUSTOMER)
                                  .OrderByDescending(q => q.CreatedAt)
                                  .ToList();

            return View(flavors);
        }

        // GET: Customer/Orders
        public ActionResult Orders()
        {
            int? customerId = ((Customer)Session["Customer"])?.Id;
            var orders = db.Orders.Where(q => q.CustomerId == customerId)
                                  .OrderByDescending(q => q.CreatedAt)
                                  .ToList();
            return View(orders);
        }

        // GET: Customer/Orders
        public ActionResult OrderDetail(string code)
        {
            int? customerId = ((Customer)Session["Customer"])?.Id;
            var order = db.Orders.Where(q => q.CustomerId == customerId && q.Code == code).FirstOrDefault();
            if (order == null)
            {
                return HttpNotFound();
            }

            return View("/Views/Customer/OrderDetail.cshtml", order);
        }

        [HttpPost]
        public ActionResult CancelOrder(CancelOrderViewModel model, int id)
        {
            int? customerId = ((Customer)Session["Customer"])?.Id;
            var order = db.Orders.Where(q => q.CustomerId == customerId
                                          && q.Id == id
                                          && q.Status == Models.Order.PROCESSING).FirstOrDefault();
            if (order == null)
            {
                return NotFound();
            }
            order.Status = Models.Order.WANT_TO_CANCEL;
            order.Message = model.Message;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            return Success("OK");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["Customer"] = null;

            return RedirectToAction("Login");
        }
    }
}