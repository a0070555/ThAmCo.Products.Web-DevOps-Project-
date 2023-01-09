using Microsoft.EntityFrameworkCore;
using ThAmCo.Products.Web.Data;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    //builder.Services.AddTransient<IProductsRepository, ProductsFakeRepository>();
}
else
{
    //builder.Services.AddHttpClient<IProductsRepository, ProductsRepository>()
    //    .AddPolicyHandler(GetRetryPolicy())
    //    .AddPolicyHandler(GetCircuitBreakerPolicy());
}

builder.Services.AddDbContext<ProductsContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var dbPath = System.IO.Path.Join(path, "products.db");
        options.UseSqlServer($"Data Source={dbPath}");
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
    else
    {
        var cs = builder.Configuration.GetConnectionString("ProductsConnection");
        options.UseSqlServer(cs);
        options.UseSqlServer(cs, sqlServerOptionsAction: sqlOptions =>
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(6),
                errorNumbersToAdd: null
            )
        );
    }
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
