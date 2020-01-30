using System.Collections.Generic;

namespace Api.Areas.Product.Contracts
{
    public interface IProductService
    {
        IEnumerable<string> List();
    }
}