using Microsoft.EntityFrameworkCore;

namespace ThAmCo.Products.Api.Models
{
    public class OrdersContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        private readonly IHostEnvironment _hostEnv;
        public OrdersContext(DbContextOptions<OrdersContext> options,
               IHostEnvironment env) : base(options)
        {
            _hostEnv = env;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
