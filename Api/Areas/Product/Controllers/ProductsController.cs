using Api.Areas.Product.Contracts;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Api.Areas.Product.Controllers
{
    public class ProductsController : ApiController
    {
        IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        // GET: /api/products
        public IEnumerable<string> Get()
        {
            return productService.List();
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
