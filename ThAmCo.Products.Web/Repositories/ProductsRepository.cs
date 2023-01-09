using ThAmCo.Products.Web.Data;
using System.Net;

namespace ThAmCo.Products.Web.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly HttpClient _client;
        public ProductsRepository(HttpClient client, IConfiguration configuration)
        {
            var baseUrl = configuration["WebServices:Products:BaseURL"];
            client.BaseAddress = new System.Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = client;
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var response = await _client.GetAsync("api/products/" + id);
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                response.EnsureSuccessStatusCode();
                var product = await response.Content.ReadAsAsync<Product>();
                return product;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(string type)
        {
            var uri = "api/products";
            if (type != null)
            {
                uri = uri + "&type=" + type;
            }
            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadAsAsync<IEnumerable<Product>>();
            return products;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            var response = await _client.PostAsJsonAsync("api/products", product);
            response.EnsureSuccessStatusCode();
            var createdProduct = await response.Content.ReadAsAsync<Product>();
            return createdProduct;
        }

        public async Task<Product> UpdateProductAsync(int id, Product product)
        {
            var response = await _client.PutAsJsonAsync("api/products/" + id, product);
            response.EnsureSuccessStatusCode();
            var updatedProduct = await response.Content.ReadAsAsync<Product>();
            return updatedProduct;
        }

        public async Task DeleteProductAsync(int id)
        {
            var response = await _client.DeleteAsync("api/products/" + id);
            response.EnsureSuccessStatusCode();
        }

    }
}
