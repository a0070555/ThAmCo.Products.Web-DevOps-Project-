using Microsoft.EntityFrameworkCore;

namespace ThAmCo.Products.Web.Data
{
    public class ProductsContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public ProductsContext(DbContextOptions<ProductsContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>(p =>
            {
                p.Property(p => p.ProductId)
                .ValueGeneratedNever();
            });
        }
    }
}
