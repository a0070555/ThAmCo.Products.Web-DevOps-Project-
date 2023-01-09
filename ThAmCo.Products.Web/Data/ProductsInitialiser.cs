using ThAmCo.Products.Web.Data;

namespace Thamco.Products.Web.Data
{
    public class ProductsInitialiser
    {
        public static async Task InsertTestData(ProductsContext context)
        {
            if (context.Products.Any())
            {
                return;
            }

            var products = new List<Product>
            {
                new Product { ProductId = 1, Type="Standard", ProductName = "Product1", Price = 4.5, Quantity = 5, Description = "The first product" },
                new Product { ProductId = 2, Type="Custom", ProductName = "Product2", Price = 5.5, Quantity = 10, Description = "The second product" },
                new Product { ProductId = 3, Type="Standard", ProductName = "Product3", Price = 10.2, Quantity = 2, Description = "The third product" },
                new Product { ProductId = 4, Type="Standard", ProductName = "Product4", Price = 12.5, Quantity = 6, Description = "The fourth product" },
                new Product { ProductId = 5, Type="Custom", ProductName = "Product5", Price = 1.5, Quantity = 9, Description = "The fifth product" }
            };
            products.ForEach(p => context.Products.Add(p));
            await context.SaveChangesAsync();
        }
    }
}
