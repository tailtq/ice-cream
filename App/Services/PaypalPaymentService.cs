using System;
using PayPal.Api;
using App.App_Start;
using System.Collections.Generic;
using System.Web;
using App.Helpers;

namespace App.Services
{
    /**
     * Document: https://www.c-sharpcorner.com/article/paypal-payment-gateway-integration-in-asp-net-mvc/
     */
    public class PaypalPaymentService
    {
        public class PaypalPaymentParam
        {
            public string Guid { get; set; }

            public string CallbackUrl { get; set; }

            public List<Item> Items { get; set; }

            public string Description { get; set; }

            public string InvoiceNumber { get; set; }

            public float Tax { get; set; } = 0;

            public float Shipping { get; set; } = 0;

            public string PaymentMethod { get; set; } = "paypal";

            public HttpSessionStateBase Session { get; set; }

            public CreditCard CreditCard { get; set; }
        }

        public class ExecutePaymentParam
        {
            public string PayerId { get; set; }

            public string Guid { get; set; }

            public HttpSessionStateBase Session { get; set; }
        }

        Payment payment;

        public string SendPayment(PaypalPaymentParam paymentParam)
        {
            try
            {
                APIContext apiContext = PaypalConfig.GetAPIContext();
                var guid = Convert.ToString((new Random()).Next(100000));
                var createdPayment = CreatePayment(apiContext, paymentParam.CallbackUrl + "?guid=" + guid, paymentParam);
                var links = createdPayment.links.GetEnumerator();
                string paypalRedirectUrl = null;

                while (links.MoveNext())
                {
                    Links lnk = links.Current;
                    if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                    {
                        paypalRedirectUrl = lnk.href;
                    }
                }
                paymentParam.Session.Add(guid, ((Payment)createdPayment).id);

                return paypalRedirectUrl;
            }
            catch
            {
                return "NG";
            }
        }

        public float ExecutePayment(ExecutePaymentParam paymentParam)
        {
            try
            {
                APIContext apiContext = PaypalConfig.GetAPIContext();
                var paymentExecution = new PaymentExecution() { payer_id = paymentParam.PayerId };
                payment = new Payment() { id = paymentParam.Session[paymentParam.Guid] as string };

                var executedPayment = payment.Execute(apiContext, paymentExecution);

                if (executedPayment.state.ToLower() != "approved")
                {
                    return -1;
                }
                return GetTotalMoney(executedPayment.transactions);
            }
            catch
            {
                return -1;
            }
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl, PaypalPaymentParam paymentParam)
        {
            var payer = new Payer();
            payer.payment_method = paymentParam.PaymentMethod;

            var redirectUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = GetTransactions(paymentParam),
                redirect_urls = redirectUrls
            };

            return payment.Create(apiContext);
        }


        public float SendPaymentByCreditCard(PaypalPaymentParam paymentParam)
        {
            CreditCard creditCard = new CreditCard()
            {
                billing_address = new Address() { city = "Johnstown", country_code = "US", line1 = "52 N Main ST", postal_code = "43210", state = "OH" },
                cvv2 = paymentParam.CreditCard.cvv2,
                expire_month = paymentParam.CreditCard.expire_month,
                expire_year = paymentParam.CreditCard.expire_year,
                first_name = paymentParam.CreditCard.first_name,
                last_name = paymentParam.CreditCard.last_name,
                type = "visa",
                number = paymentParam.CreditCard.number,
            };

            FundingInstrument fundInstrument = new FundingInstrument();
            fundInstrument.credit_card = creditCard;

            var fundingInstruments = new List<FundingInstrument>();
            fundingInstruments.Add(fundInstrument);

            Payer payer = new Payer()
            {
                funding_instruments = fundingInstruments,
                payment_method = "credit_card"
            };
            var payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = GetTransactions(paymentParam),
            };

            try
            {
                APIContext apiContext = PaypalConfig.GetAPIContext();
                Payment createdPayment = payment.Create(apiContext);

                if (createdPayment.state.ToLower() != "approved")
                {
                    return -1;
                }
                return GetTotalMoney(createdPayment.transactions); ;
            }
            catch
            {
                return -1;
            }
        }

        private List<Transaction> GetTransactions(PaypalPaymentParam paymentParam)
        {
            var transactions = new List<Transaction>();
            // Adding description about the transaction  
            transactions.Add(new Transaction()
            {
                description = paymentParam.Description,
                invoice_number = paymentParam.InvoiceNumber, //Generate an Invoice No  
                amount = GetAmount(paymentParam),
                item_list = GetItemList(paymentParam)
            });

            return transactions;
        }

        private Amount GetAmount(PaypalPaymentParam paymentParam)
        {
            var details = new Details()
            {
                tax = paymentParam.Tax.ToString(),
                shipping = paymentParam.Shipping.ToString(),
                subtotal = (paymentParam.Tax + paymentParam.Shipping).ToString(),
            };
            var amount = new Amount()
            {
                currency = CommonConstants.CURRENCY,
                total = GetTotalMoney(paymentParam).ToString(), // Total must be equal to sum of tax, shipping and subtotal.  
                details = details.subtotal == "0" ? null : details
            };
            return amount;
        }

        private ItemList GetItemList(PaypalPaymentParam paymentParam)
        {
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            itemList.items.AddRange(paymentParam.Items);

            return itemList;
        }

        private float GetTotalMoney(PaypalPaymentParam paymentParam)
        {
            float total = paymentParam.Shipping + paymentParam.Tax;

            foreach (Item item in paymentParam.Items)
            {
                total += float.Parse(item.price) * float.Parse(item.quantity);
            }
            
            return total;
        }

        private float GetTotalMoney(List<Transaction> transactions)
        {
            float total = 0;

            foreach (Transaction transaction in transactions)
            {
                total += float.Parse(transaction.amount.total);
            }

            return total;
        }

        public PayoutBatch SendPayout(string subject, List<PayoutItem> payouts)
        {
            try
            {
                var payout = new Payout()
                {
                    sender_batch_header = new PayoutSenderBatchHeader
                    {
                        sender_batch_id = "batch_" + System.Guid.NewGuid().ToString().Substring(0, 8),
                        email_subject = subject
                    },
                    items = payouts
                };

                return payout.Create(PaypalConfig.GetAPIContext(), false);
            }
            catch
            {
                return null;
            }
        }
    }
}