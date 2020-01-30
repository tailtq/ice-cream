using Api.Areas.Product.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Areas.Product.Services
{
    public class ProductService : IProductService
    {
        public IEnumerable<string> List()
        {
            return new string[] { "123123123", "value2" };
        }
    }
}