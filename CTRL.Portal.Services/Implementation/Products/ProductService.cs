using CTRL.Inventory.Client.Interfaces;
using CTRL.Inventory.Common.Contracts;
using CTRL.Portal.Services.Interfaces.Products;
using System;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Implementation.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductApiProvider _productApiProvider;

        public ProductService(IProductApiProvider productApiProvider)
        {
            _productApiProvider = productApiProvider ?? throw new ArgumentNullException(nameof(productApiProvider));
        }

        public async Task<Product> AddProduct(Product product)
        {
            var result = await _productApiProvider.AddProduct(product);

            return result;
        }
    }
}
