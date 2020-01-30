using App.Helpers;
using App.Models;
using FizzWare.NBuilder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Controllers
{
    public class SeedersController : Controller
    {
        IceCreamEntities db = new IceCreamEntities();

        Random random = new Random();

        // GET: Migrations
        public void Index()
        {
            //SeedPermissions();
            //SeedLevels();
            //SeedStaffs();
            //SeedBookCategories();
            //SeedBooks();
            //SeedSuppliers();
            //SeedImports();
            //SeedFlavors();
            //SeedFeedBacks();
            //SeedCustomers();
            //SeedCustomer();
            //SeedOrders();
        }

        private void SeedOrders()
        {
            List<Customer> customers = db.Customers.Where(q => q.DeletedAt == null).ToList();
            List<Book> books = db.Books.Where(q => q.DeletedAt == null).ToList();

            var orders = Builder<Order>.CreateListOfSize(500)
                    .All()
                        .With(c => c.CustomerId = customers[random.Next(0, customers.Count() - 1)].Id)
                        .With(c => c.StaffId = null)
                        .With(c => c.ShippingFee = 5.00m)
                        .With(c => c.Tax = 0)
                        .With(c => c.Status = Order.PROCESSING)
                        .With(c => c.ArrivalDate = DateTime.Now.AddDays(5))
                        .With(c => c.PaymentSum = 30.00m)
                        .With(c => c.CreatedAt = DateTime.Now)
                        .With(c => c.UpdatedAt = DateTime.Now)
                    .Build();

            db.Orders.AddRange(orders);
            db.SaveChanges();

            foreach (Order order in orders)
            {
                var orderDetails = Builder<OrderDetail>.CreateListOfSize(5)
                    .All()
                        .With(c => c.BookId = books[random.Next(0, books.Count() - 1)].Id)
                        .With(c => c.OrderId = order.Id)
                        .With(c => c.Quantity = 1)
                        .With(c => c.Price = 6.00m)
                        .With(c => c.Discount = 0)
                        .With(c => c.InputMoney = 5)
                    .Build();

                db.OrderDetails.AddRange(orderDetails);
                db.SaveChanges();
            }
        }

        private void SeedCustomer()
        {
            var customer = new Customer()
            {
                Name = "Customer",
                Password = Hashing.HashPassword("123"),
                Email = "customer@gmail.com",
                Address = "Da Nang",
                Phone = "123123",
                Avatar = "123",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            db.Customers.Add(customer);
            db.SaveChanges();
        }

        private void SeedCustomers()
        {
            try
            {
                var customers = Builder<Customer>.CreateListOfSize(10)
                    .All()
                        .With(c => c.Name = string.Join(" ", Faker.Lorem.Words(3)))
                        .With(c => c.ResetPasswordToken = null)
                        .With(c => c.CreatedAt = DateTime.Now)
                        .With(c => c.UpdatedAt = DateTime.Now)
                        .With(c => c.DeletedAt = null)
                        .With(c => c.DeletedBy = null)
                    .Build();

                db.Customers.AddRange(customers);
                db.SaveChanges();
            }
            catch
            {
                
            }
        }

        private void SeedPermissions()
        {   
            var listPermissions = new List<Permission>();

            string[] crudModules = { "Book", "BookCategory", "Level", "Media", "Staff", "Supplier" };
            string[] rdModules = { "Customer" };
            string[] rudModules = { "Feedback" };
            string[] ruModules = { "Order" };
            string[] crModules = { "Input", "Payout" };

            foreach (string module in crudModules)
            {
                listPermissions.Add(new Permission() { Name = "Create " + module, Key = module.ToLower() + "_create", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, });
                listPermissions.Add(new Permission() { Name = "View " + module, Key = module.ToLower() + "_view", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, });
                listPermissions.Add(new Permission() { Name = "Update " + module, Key = module.ToLower() + "_update", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, });
                listPermissions.Add(new Permission() { Name = "Delete " + module, Key = module.ToLower() + "_delete", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, });
            }

            foreach (string module in rdModules)
            {
                listPermissions.Add(new Permission() { Name = "View " + module, Key = module.ToLower() + "_view", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, });
                listPermissions.Add(new Permission() { Name = "Delete " + module, Key = module.ToLower() + "_delete", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, });
            }

            foreach (string module in rudModules)
            {
                listPermissions.Add(new Permission() { Name = "View " + module, Key = module.ToLower() + "_view", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, });
                listPermissions.Add(new Permission() { Name = "Update " + module, Key = module.ToLower() + "_update", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, });
                listPermissions.Add(new Permission() { Name = "Delete " + module, Key = module.ToLower() + "_delete", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, });
            }

            foreach (string module in ruModules)
            {
                listPermissions.Add(new Permission() { Name = "View " + module, Key = module.ToLower() + "_view", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, });
                listPermissions.Add(new Permission() { Name = "Update " + module, Key = module.ToLower() + "_update", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, });
            }

            foreach (string module in crModules)
            {
                listPermissions.Add(new Permission() { Name = "Create " + module, Key = module.ToLower() + "_create", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, });
                listPermissions.Add(new Permission() { Name = "View " + module, Key = module.ToLower() + "_view", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, });
            }

            db.Permissions.AddRange(listPermissions);
            db.SaveChanges();
        }

        private void SeedLevels()
        {
            Level level = db.Levels.Add(new Level() {
                Name = "Owner",
                Key = "owner",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            });
            
            foreach (Permission permission in db.Permissions)
            {
                level.Permissions.Add(permission);
            }

            db.SaveChanges();
        }

        private void SeedStaffs()
        {
            db.Staffs.Add(new Staff()
            {
                LevelId = db.Levels.Where(q => q.Key == "owner").FirstOrDefault().Id,
                Name = "Owner",
                Password = Hashing.HashPassword("123123"),
                Email = "owner@gmail.com",
                Address = "Da Nang",
                Phone = "123123123",
                Avatar = "/test.png",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            });
            db.SaveChanges();
        }

        private void SeedBookCategories()
        {
            var categories = Builder<BookCategory>.CreateListOfSize(10)
                    .All()
                        .With(c => c.Name = string.Join(" ", Faker.Lorem.Words(3)))
                        .With(c => c.Slug = Slugify.GenerateSlug(string.Join(" ", Faker.Lorem.Words(3))))
                        .With(c => c.CreatedAt = DateTime.Now)
                        .With(c => c.UpdatedAt = DateTime.Now)
                        .With(c => c.DeletedAt = null)
                        .With(c => c.DeletedBy = null)
                    .Build();

            db.BookCategories.AddRange(categories);
            db.SaveChanges();
        }

        private void SeedBooks()
        {
            string[] images = {
                "https://salt.tikicdn.com/cache/200x200/ts/product/02/08/5a/afe5d11f12e2c183ffd4d0d84c463a92.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/93/02/93/1848f40aa4dd33d52e82bf5d12f53d21.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/e5/50/24/c7e310369e1df1349edeeebe57ec5d54.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/52/f9/50/cfe86656244e1dffbe24c48845b86cc3.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/n/h/nhat_ky_hoc_lam_banh_1_.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/71/df/a2/17c4419162aadbca1bf358ecede6b602.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/a6/2c/0d/f643e29d33c182665e6e259b40ef7ba4.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/n/h/nhat-ky-hoc-lam-banh-2_1.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/69/e7/85/9e271bc73498c75c7f3bd33ef6597281.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/21/f5/a8/be4bf3d75bd56b1094343e44423548fd.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/6d/86/32/cb806d0177d973ab0bcf823212d0f6a4.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/9f/81/9b/f743199b3cdd89eafedf77d59c11be74.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/a0/0f/7a/927e3ab5b0e7f10cf20a5da6d7827d95.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/26/a0/6e/3442a00dcf5b684c5ace2d5744a8f325.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/b/i/bia-70-mon-uong-take-away-cs2.u2769.d20170303.t163059.160650.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/d2/e2/92/0fe4abae50fb512237df425c92712fd2.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/a3/cb/7b/afeefe2a4735e1c14f2d0922ce53f395.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/i/m/img379_1_7.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/5f/99/71/54058ba5e681ece1d4088b5e499f52a9.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/09/25/32/40893f14ce7f843f3c485ba00a98f5ba.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/n/g/ngau-hung-cung-cake.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/l/a/lam-banh_3.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/e0/c4/3d/bef58b1d595c622b3444bbfb8c03b933.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/11/19/e7/37ef57c5cd341aa23d623cef8f85585c.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/8/0/80-mon-an-vat.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/98/82/20/eb494fbf1fbb234975e0792f10ed4e52.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/i/m/img270.u547.d20160709.t141800.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/m/i/mieng-nho.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/98/da/88/5393c1eacc84cd62e49e401181e9c29c.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/2f/9d/c9/af6a3ac3b8ae2097ac46112c7dd1e6fb.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/58/81/d6/8d530d6ab8d9975f55c2c59315dc641a.JPG",
                "https://salt.tikicdn.com/cache/200x200/ts/product/96/9c/5c/219e3ba4956d23a4e9f3a2d7aae47119.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/n/g/ngot-ngao-huong-vi-banh-mi_3.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/m/o/mon-la-mien-nam.u2751.d20170404.t174408.36375.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/i/m/img770.u335.d20161024.t135737.120774.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/ce/6f/ea/807a41f6f440fd8e9838d6e005898d80.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/i/m/img569_8.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/h/o/hon_ca_an_ngon.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/i/m/img062.u335.d20160617.t132515.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/i/m/img064.u335.d20160617.t141254.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/3c/b2/14/4c7d9f0ea84bc28270fd0372074f1ba1.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/c/a/cac-ky-nang-yoga-danh-cho-nhatri-lieu.jpg",
                "https://salt.tikicdn.com/cache/200x200/ts/product/df/98/cf/5fdc9c8f78f7296877838d4384a9f19a.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/i/m/img763_6.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/p/h/pho-va-cac-mon-nuoc-1.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/i/m/image1--1-.u4972.d20170508.t173350.873384.jpg",
                "https://salt.tikicdn.com/cache/200x200/media/catalog/product/c/a/cac_mon_an_danh_cho_nguoi_benh_tieu_duong_1.jpg"
            };
            int imageLength = images.Length;

            var books = Builder<Book>.CreateListOfSize(100)
                    .All()
                        .With(c => c.CategoryId = db.BookCategories.First().Id)
                        .With(c => c.StaffId = db.Staffs.Where(q => q.Name == "Owner").First().Id)
                        .With(c => c.Images = JsonConvert.SerializeObject(new string[] { images[random.Next(0, imageLength - 1)] }))
                        .With(c => c.DeletedAt = null)
                        .With(c => c.DeletedBy = null)
                    .Build();

            db.Books.AddRange(books);
            db.SaveChanges();
        }

        private void SeedSuppliers()
        {
            var suppliers = Builder<Supplier>.CreateListOfSize(10)
                    .All()
                        .With(c => c.DeletedAt = null)
                        .With(c => c.DeletedBy = null)
                    .Build();

            db.Suppliers.AddRange(suppliers);
            db.SaveChanges();
        }

        private void SeedImports()
        {
            var suppliers = db.Suppliers.ToArray();
            Random r = new Random();

            var inputs = Builder<Input>.CreateListOfSize(db.Books.Count())
                .All()
                    .With(c => c.StaffId = db.Staffs.Where(q => q.Name == "Owner").First().Id)
                    .With(c => c.SupplierId = suppliers[random.Next(0, suppliers.Count() - 1)].Id)
                .Build();

            db.Inputs.AddRange(inputs);
            db.SaveChanges();

            /////////////////////////////////////////////////// Input Details

            inputs = db.Inputs.ToArray();
            var books = db.Books.ToArray();
            int i = 0;

            var inputDetails = Builder<InputDetail>.CreateListOfSize(inputs.Count())
                .All()
                    .With(c => c.InputId = inputs[i].Id)
                    .With(c => c.BookId = books[i++].Id)
                .Build();

            db.InputDetails.AddRange(inputDetails);
            db.SaveChanges();

            /////////////////////////////////////////////////// Input Details

            inputDetails = db.InputDetails.ToArray();
            i = 0;

            var inputQuantities = Builder<InputQuantity>.CreateListOfSize(inputDetails.Count())
                .All()
                    .With(c => c.InputDetailId = inputDetails[i].Id)
                    .With(c => c.RemainQuantity = inputDetails[i++].Quantity)
                .Build();

            db.InputQuantities.AddRange(inputQuantities);
            db.SaveChanges();
        }

        private void SeedFlavors()
        {
            string[] images = {
                "https://toinayangi.vn/wp-content/uploads/2014/11/cach-lam-kem-va-ni-1.jpg",
                "https://anh.24h.com.vn/upload/2-2016/images/2016-05-17/1463463353-lam-kem-vani5-1463393317.jpg",
                "https://photo-2-baomoi.zadn.vn/w1000_r1/2018/08/27/338/27470782/1_50581.jpg",
                "https://img-global.cpcdn.com/005_recipes/f5ef2ce6b1700969/751x532cq70/kem-vani-cho-ng%C6%B0%E1%BB%9Di-an-kieng-recipe-main-photo.jpg",
                "https://static.hotdeal.vn/images/1065/1065330/500x500/279351-kem-thien-ly-danh-cho-1-nguoi.jpg",
                "https://c7.staticflickr.com/8/7313/27899153446_012660f29f_o.jpg",
                "http://cdn.nhanh.vn/cdn/store/9106/ps/20170901/P2618_Ly_Kem_Cao_222ml_800x800.png",
                "https://hd1.hotdeal.vn/images/uploads/2016/Thang%205/13/257047/257047-kem-thien-ly-body-%20%288%29.jpg" ,
                "https://png.pngtree.com/png-clipart/20190614/original/pngtree-red-ice-cream-cup-png-image_3686628.jpg" ,
                "https://phuongnga.com/wp-content/uploads/2017/06/G12-Kayla-2.jpg" ,
                "http://bepvang.org.vn/userfiles/upload/images/modules/news/2017/4/5/0_nhung-hinh-anh-dep-cua-nhung-ly-kem-mat-lanh-11-10.jpg" ,
                "http://amthuc01.websieutoc.com.vn/uploads/images/amthuc01/sanpham/a22_(FILEminimizer).jpg" ,
                "https://www.seriouseats.com/2018/06/20180625-no-churn-vanilla-ice-cream-vicky-wasik-13-1500x1125.jpg" ,
                "https://assets3.thrillist.com/v1/image/2825085/size/gn-gift_guide_variable_c;jpeg_quality=20.jpg" ,
                "http://thebeeroness.com/wp-content/uploads/2019/07/Beer-and-Doughnut-Ice-Cream4.jpg" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTu3Gdos0agHqlZyLyrpQjwOwQhQLc-031bJ6Sga1RLjpI3pk_M" ,
                "https://bakingamoment.com/wp-content/uploads/2018/09/IMG_9763-how-to-make-ice-cream-cone-cupcakes-square.jpg" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTDXVH20TmXWkF8l0XdeL09ykh4JffEh4AOjatvqVZWjm0Y_zfeSg" ,
                "https://www.lifegetsbetter.ph/uploads/recipes/featured/Fruit-Pandan-Ice-Cream-Salad.jpg" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRUS0phmnVlPARI-fIfv23Is2HrJYbcUvS3uja1DIXtqxUW5Qsr2A" ,
                "https://insidebrucrewlife.com/wp-content/uploads/2019/07/Chocolate-Peanut-Butter-Fudge-Oreo-Ice-Cream-4-1.jpg" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQMjetCgywN4ioTqm0_rkpMdrHWPVZEtv11y2k2hZ9_yeluy0qL" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRKO77ZIuzlqu2GGUMe2bonWB7-bm39oy2805Z8QR2X6GrTlima" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRMzTyUMCbpnw5ykwXxcOuZExB22J12mlzGYHY93MX7FZq5SoMTZQ" ,
                "https://hips.hearstapps.com/hmg-prod/images/delish-neapolitan-ice-cream-cake-still001-2-1565369559.jpg" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT6SFF2A8gXZzyc21TT1p4XlXuFpW8Q4yUfs28IPvc_XyjbjvQX5w" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTe1RFs7uzxJWQn5aDNEU8B1CRXoQWNpU1Doqo6fjx1asFL4MiP" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSpxQrcBzdNauAW1SdX0yswqu3xKLzgHzEgFq--SyrTDRsYSVLKNA" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTNlwXBL_xMZwbq8bXaUqQdsjaG4okJFavCEnhq6XZW5anIxuJP" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRD0Z9wJFKM6Y5eB4w29jS4qg6GngbuKtyrd-69WbKVoE3aaFUy" ,
                "https://img.taste.com.au/1Gjeh9Br/w720-h480-cfill-q80/taste/2018/11/mango-raspberry-ice-cream-bombe-144081-2.jpg" ,
                "https://www.chewoutloud.com/wp-content/uploads/2016/05/Dairy-Free-Creamy-Ice-Cream-Watermark-e1498622162953.jpg" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQcQftqpMbSWlXmhKfmDhYjyLvoDY-nsNAP6SxjKqdnqsomhkvNyg" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT2UDxbSNcKakKXhnjQxYSwXoibqHFL2IIHjcgWSkDPHOlZv8u33A" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRzq8oso8ka9IGUe7Uqr1uVdZk5UMX9-rmBSToRlmUWDiVz6Cgucg" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTC5m0Eie0hEmEUshTVPKAgvBaprc0Y-VgU8Bzr6P8Q-_1aP4Waew" ,
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRt-nBXuXKzqFIoqs_5t0uinC5lJRdfHwSf_TVNUM97k5vylqCYLQ"
            };
            int imageLength = images.Length;
            Random r = new Random();

            var flavors = Builder<Flavor>.CreateListOfSize(100)
                    .All()
                        .With(c => c.UserId = db.Staffs.Where(q => q.Name == "Owner").First().Id)
                        .With(c => c.UserType = 1)
                        .With(c => c.Images = JsonConvert.SerializeObject(new string[] { images[random.Next(0, imageLength - 1)] }))
                        .With(c => c.IsApproved = true)
                        .With(c => c.DeletedAt = null)
                        .With(c => c.DeletedBy = null)
                    .Build();

            db.Flavors.AddRange(flavors);
            db.SaveChanges();
        }

        private void SeedFeedBacks()
        {
            var feedbacks = Builder<Feedback>.CreateListOfSize(100)
                    .All()
                        .With(c => c.StaffId = null)
                        .With(c => c.IsRead = false)
                        .With(c => c.DeletedAt = null)
                        .With(c => c.DeletedBy = null)
                    .Build();

            db.Feedbacks.AddRange(feedbacks);
            db.SaveChanges();
        }
    }
}