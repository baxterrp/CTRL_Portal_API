using CTRL.Inventory.Common.Contracts;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Interfaces.Products
{
    public interface IProductService
    {
        Task<Product> AddProduct(Product product);
    }
}
