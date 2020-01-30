using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ICEDbContext : DbContext
    {
        public ICEDbContext() : base("ICECreamConnection")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) // phương thức này sẽ chạy khi khởi tạo entity 
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}