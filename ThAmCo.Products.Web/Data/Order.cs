namespace ThAmCo.Products.Web.Data
{
    public class Order
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}
