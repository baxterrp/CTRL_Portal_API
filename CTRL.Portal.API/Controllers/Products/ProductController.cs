using CTRL.Inventory.Common.Contracts;
using CTRL.Portal.Services.Interfaces.Products;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Controllers.Products
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [HttpPost("addProduct")]
        public async Task<IActionResult> AddProduct([FromBody]Product product)
        {
            var result = await _productService.AddProduct(product);

            return Ok(result);
        }
    }
}
