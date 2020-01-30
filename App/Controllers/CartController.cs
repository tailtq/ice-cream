using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using App.Infrastructure.Controllers;
using App.ViewModels.Client.Cart;
using App.Models;
using App.ViewModels.Client.Customer;
using App.Services;
using System.Collections.Generic;
using PayPal.Api;
using System.Net;
using App.Mappings;
using App.Helpers;
using System.Threading.Tasks;

namespace App.Controllers
{
    public class CartController : BaseController
    {
        string CookieName = "cart";

        IceCreamEntities db = new IceCreamEntities();

        PaypalPaymentService PaymentService = new PaypalPaymentService();

        public PartialViewResult CartBlock()
        {
            CartViewModel cart = GetCart();

            return PartialView("PartialView/CartBlock", cart);
        }

        // GET: /Cart
        public ActionResult Index()
        {
            // Show remain quantity
            var cart = GetCart();

            for (int i = 0; i < cart.ListItem.Count; i++)
            {
                CartItemViewModel item = cart.ListItem[i];
                var book = db.Books.Where(q => q.DeletedAt == null && q.Id == item.Id).FirstOrDefault();

                if (book == null)
                {
                    cart.ListItem.RemoveAt(i);
                }
                else
                {
                    item.RemainQuantity = book.Quantity;
                }
            }

            return View(cart);
        }

        // POST: /Cart
        [HttpPost]
        public ActionResult Index(CartViewModel model)
        {
            foreach (var item in model.ListItem)
            {
                var book = db.Books.Where(q => q.DeletedAt == null && q.Id == item.Id).FirstOrDefault();
                if (book == null)
                {
                    return NotFound();
                }
                else if (model.GetItem(book.Id).Quantity > book.Quantity)
                {
                    return BadRequest("NOT_ENOUGH_BOOK");
                }
            }

            SaveCart(model);

            return Success("OK");
        }

        // POST: Cart/AddBook/1
        [HttpPost]
        public ActionResult AddBook(int id, AddToCartViewModel model)
        {
            var book = db.Books.Where(q => q.DeletedAt == null && q.Id == id).FirstOrDefault();
            if (book == null)
            {
                return NotFound();
            }

            CartViewModel cart = GetCart();
            CartItemViewModel cartItem = new CartItemViewModel()
            {
                Id = book.Id,
                Name = book.Name,
                Slug = book.Slug,
                Quantity = model.Quantity <= 0 ? 1 : model.Quantity,
                Price = book.Price,
                Discount = book.Discount,
                Image = book.DecodedImages()[0]
            };
            cart.UpdateItem(cartItem);
            if (cart.GetItem(book.Id).Quantity > book.Quantity)
            {
                return BadRequest("NOT_ENOUGH_BOOK");
            }

            SaveCart(cart);

            return Success(cart);
        }

        // GET: Cart/Checkout
        public ActionResult Checkout()
        {
            var cart = GetCartWithTruePrice();
            if (cart.ListItem.Count <= 0 || cart.HasInvalidQuantity())
            {
                return RedirectToAction("Index");
            }

            Customer customer = (Customer)Session["Customer"];
            var model = MappingProfile.mapper.Map<Customer, CheckoutViewModel>(customer);
            ViewBag.cart = cart;

            return View(model);
        }

        [HttpPost]
        public ActionResult Checkout(CheckoutViewModel model)
        {
            CartViewModel cart = GetCartWithTruePrice();
            if (cart.HasInvalidQuantity())
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                List<Item> items = new List<Item>();
                foreach (var item in cart.ListItem)
                {
                    items.Add(new Item()
                    {
                        name = item.Name,
                        currency = "USD",
                        price = item.GetPriceAfterDiscount().ToString(),
                        quantity = 
                        item.Quantity.ToString(),
                    });
                }

                var paymentParam = new PaypalPaymentService.PaypalPaymentParam()
                {
                    Session = Session,
                    CallbackUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/Cart/PaymentCallback",
                    Items = items
                };
                string url = PaymentService.SendPayment(paymentParam);
                if (url == "NG")
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                // Credit card here
                Session["Cart"] = cart;
                Session["CheckoutViewModel"] = model;

                return Redirect(url);
            }
            ViewBag.cart = cart;

            return View(model);
        }

        // GET: Customer/PaymentCallback
        [Obsolete]
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

            Customer customer = SaveCustomer((CheckoutViewModel)Session["CheckoutViewModel"], ((Customer)Session["Customer"])?.Id);
            SaveOrder((CartViewModel)Session["Cart"], customer);
            SaveCart(new CartViewModel());
            // decrease quantity
            // notify.....

            return RedirectToAction("Index");
        }

        [Obsolete]
        public Customer SaveCustomer(CheckoutViewModel model, int? id)
        {
            Customer customer = db.Customers.Where(q => q.Email == model.Email).FirstOrDefault();

            if (customer == null)
            {
                string password = CommonHelper.RandomString(12).ToLower();
                customer = db.Customers.Add(new Customer()
                {
                    Name = model.Name,
                    Password = Hashing.HashPassword(password),
                    Email = model.Email,
                    Address = model.Address,
                    Phone = model.Phone,
                    Avatar = "/Content/Client/img/others/blank-avatar.png",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                });
                var newModel = new NewAccountViewModel()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = password,
                    LoginUrl = CommonHelper.GetBaseUrl() + Url.Action("Login", "Customer"),
                };
                Task.Factory.StartNew(() => {
                    EmailHandler.Handle(newModel, newModel.Email, "New Account", "Views/Customer/Template/NewAccountTemplate.cshtml");
                });
            }
            else
            {
                customer.Name = model.Name;
                customer.Email = model.Email;
                customer.Address = model.Address;
                customer.Phone = model.Phone;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            }
            return customer;
        }

        public App.Models.Order SaveOrder(CartViewModel cart, Customer customer)
        {
            Models.Order order = db.Orders.Add(new Models.Order()
            {
                CustomerId = customer.Id,
                Code = GetUniqueOrderCode(),
                ShippingFee = (byte)CommonConstants.SHIPPING_FEE,
                Tax = 0,
                Status = Models.Order.PROCESSING,
                ArrivalDate = DateTime.Now.AddDays(5),
                PaymentSum = decimal.Parse(cart.PaymentSum.ToString()),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            });
            List<OrderDetail> details = new List<OrderDetail>();
            foreach (var item in cart.ListItem)
            {
                details.Add(new OrderDetail()
                {
                    OrderId = order.Id,
                    BookId = item.Id,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Discount = item.Discount,
                    InputMoney = 0.01m,
                    InputInformation = "",
                });
            }
            db.OrderDetails.AddRange(details);
            DecreaseQuantity(details);
            db.SaveChanges();

            return order;
        }

        public string GetUniqueOrderCode()
        {
            string code = CommonHelper.RandomString(8, "digit");
            while (db.Orders.Where(q => q.Code == code).FirstOrDefault() != null)
            {
                code = CommonHelper.RandomString(8, "digit");
            }
            return code;
        }

        public void DecreaseQuantity(List<OrderDetail> orderDetails)
        {
            foreach (var item in orderDetails)
            {
                var inputDetails = db.InputDetails.Where(q => q.BookId == item.BookId).ToList();

                foreach (var inputDetail in inputDetails)
                {
                    var inputQuantity = inputDetail.InputQuantities.ToList()[0];
                    if (inputQuantity.RemainQuantity >= item.Quantity)
                    {
                        inputQuantity.RemainQuantity -= item.Quantity;
                        db.Entry(inputQuantity).State = System.Data.Entity.EntityState.Modified;
                        break;
                    }
                    else
                    {
                        item.Quantity -= inputQuantity.RemainQuantity;
                        inputQuantity.RemainQuantity = 0;
                    }
                    db.Entry(inputQuantity).State = System.Data.Entity.EntityState.Modified;
                }
            }
        }

        public CartViewModel GetCart()
        {
            var cookies = ControllerContext.HttpContext.Request.Cookies;
            if (cookies == null || !cookies.AllKeys.Contains(CookieName))
            {
                return new CartViewModel();
            }

            var cart = JsonConvert.DeserializeObject<CartViewModel>(cookies[CookieName].Value);
            if (cart != null)
            {
                return cart;
            }

            return new CartViewModel();
        }

        public CartViewModel GetCartWithTruePrice()
        {
            CartViewModel cart = GetCart();

            for (int i = 0; i < cart.ListItem.Count; i++)
            {
                CartItemViewModel cartItem = cart.ListItem[i];
                if (cartItem != null)
                {
                    Book book = db.Books.Where(q => q.DeletedAt == null && q.Id == cartItem.Id).FirstOrDefault();
                    if (book != null)
                    {
                        cartItem.Name = book.Name;
                        cartItem.Price = book.Price;
                        cartItem.Discount = book.Discount;
                        cartItem.RemainQuantity = book.Quantity;
                    }
                    else
                    {
                        cart.ListItem.RemoveAt(i);
                    }
                }
            }

            cart.RecalculateTotalAndQuantity();

            return cart;
        }

        public void SaveCart(CartViewModel cart)
        {
            HttpCookie cookie = new HttpCookie(CookieName);
            cookie.Value = JsonConvert.SerializeObject(cart);
            cookie.Expires = DateTime.Now + TimeSpan.FromDays(30);

            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}

