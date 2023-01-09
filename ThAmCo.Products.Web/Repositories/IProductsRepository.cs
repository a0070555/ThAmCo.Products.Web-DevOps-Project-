using System;
using ThAmCo.Products.Web.Data;

namespace ThAmCo.Products.Web.Repositories
{
    public interface IProductsRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync(string type);

        Task<Product> GetProductAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(int id, Product product);
        Task DeleteProductAsync(int value);
    }
}
