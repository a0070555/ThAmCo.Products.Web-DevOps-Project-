using Microsoft.AspNetCore.Mvc;
using ThAmCo.Products.Web.Data;

namespace ThAmCo.Products.Web.Repositories
{
    public class ProductsFakeRepository : IProductsRepository

    {
        private readonly Product[] _products =
        {
            new Product { ProductId = 1, Type="Standard", ProductName = "Test", Quantity = 2, Price = 0.5, Description = "This is a test" },
            new Product { ProductId = 2, Type="Custom", ProductName = "Test Two", Quantity = 1, Price = 1.5, Description = "This is a test of product 2" },
            new Product { ProductId = 3, Type="Standard", ProductName = "Test Three", Quantity = 5, Price = 5.0, Description = "This is a test of product 3" },
            new Product { ProductId = 4, Type="Custom", ProductName = "Test Four", Quantity = 10, Price = 6.2, Description = "This is a test of product 4" }

        };


        public Task<Product> CreateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetProductAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            return Task.FromResult(product);
        }

        public Task<IEnumerable<Product>> GetProductsAsync(string type)
        {
            var products = _products.AsEnumerable();
            if (type != null)
            {
                products = products.Where(p => p.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
            }
            return Task.FromResult(products);
        }

        public Task<Product> UpdateProductAsync(int id, Product product)
        {
            throw new NotImplementedException();
        }
    }
}
