using App.Helpers;
using App.Infrastructure.Controllers;
using App.Models;
using App.Services;
using App.ViewModels.Admin.Order;
using PagedList;
using PayPal.Api;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace App.Areas.Admin.Controllers
{
    [Authorize]
    public class OrdersController : BaseController
    {
        IceCreamEntities db = new IceCreamEntities();

        PaypalPaymentService PaymentService = new PaypalPaymentService();

        // GET: Admin/Orders
        public ActionResult Index(string search, int page = 1)
        {
            ViewBag.searchKey = search;
            var query = db.Orders.Include("OrderDetails").Include("Customer").Select(i => new OrderViewModel()
            {
                Id = i.Id,
                StaffName = i.Staff.Name,
                Code = i.Code,
                CustomerName = i.Customer.Name,
                OrderDetails = i.OrderDetails,
                Status = i.Status,
                Message = i.Message,
                CreatedAt = i.CreatedAt,
            }).OrderByDescending(x => x.CreatedAt).AsEnumerable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Code.ToLower().Contains(search.ToLower()));
            }

            return View(query.ToPagedList(page, PAGE_SIZE));
        }

        // POST: Admin/Orders/UpdateStatus/5
        [HttpPost]
        [System.Obsolete]
        public ActionResult UpdateStatus(int id, UpdateOrderStatus model)
        {
            if (ModelState.IsValid)
            {
                int currentStaffId = ((Staff)Session["Staff"]).Id;
                var order = db.Orders.Where(q => q.Id == id).FirstOrDefault();

                if (order == null) {
                    return NotFound();
                }
                else if ((order.StaffId != null && order.StaffId != currentStaffId)
                    || model.Status == App.Models.Order.PROCESSING
                    || model.Status == App.Models.Order.WANT_TO_CANCEL
                    || order.Status == App.Models.Order.RECEIVED
                    || order.Status == App.Models.Order.CANCELED)
                {
                    return BadRequest();
                }
                order.StaffId = currentStaffId;
                order.Message = order.Message ?? model.Message;
                order.Status = model.Status;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                EmailHandler.Handle(order, order.Customer.Email, "Detail about order " + order.Code, "Areas/Admin/Views/Orders/Template/OrderTemplate.cshtml");
                
                // Transfer money back if status is canceled
                if (order.Status == App.Models.Order.CANCELED)
                {
                    var customer = db.Customers.Where(q => q.Id == order.CustomerId).First();
                    PaymentService.SendPayout("Order is canceled", new List<PayoutItem>()
                    {
                        new PayoutItem()
                        {
                            recipient_type = (customer.CreditCard != null ? PayoutRecipientType.PAYPAL_ID : PayoutRecipientType.EMAIL),
                            amount = new Currency
                            {
                                value = order.GrandTotal.ToString(),
                                currency = "USD"
                            },
                            receiver = customer.CreditCard ?? customer.Email,
                            note = "Money of order #" + order.Code + " is sent back",
                            sender_item_id = "payout_" + order.Id,
                        }
                    });
                }

                return Success(new {
                    Id = order.Id,
                    Status = order.Status,
                    StaffName = order.Staff.Name
                });
            }
            return BadRequest();
        }
    }
}
