using Bogus;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MiniETicaret.Products.WebAPI.Context;
using MiniETicaret.Products.WebAPI.Dtos;
using MiniETicaret.Products.WebAPI.Models;
using TS.Result;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/seedData", (ApplicationDbContext context) =>
{
    for (int i = 0; i < 100; i++)
    {
        Faker faker = new Faker();
        Product product = new Product()
        {
            ProductName =faker.Commerce.ProductName(),
            Price =Convert.ToDecimal( faker.Commerce.Price()),
            Stock = faker.Commerce.Random.Int(1,100)
        };
        context.Add(product);
        
    }
    context.SaveChanges();

    return Results.Ok(Result<string>.Succeed("Seed datalar çalýþtýrýldý ve ürünler oluþturuldu"));
});

app.MapGet("/getall", async(ApplicationDbContext context, CancellationToken token) =>
{
    var products=await context.Products.OrderBy(p=>p.ProductName).ToListAsync(token);
    Result<List<Product>> response = products;
    return response;
});

app.MapPost("/create",async (CreateProductDto dto,ApplicationDbContext context, CancellationToken token) =>
{
    bool isNameExists = await context.Products.AnyAsync(p => p.ProductName == dto.ProductName, token);
    if (isNameExists)
    {
        var response= Result<string>.Failure("Ürün adý daha önce oluþturuldu");
        return Results.BadRequest(response);
    }
    Product product = new Product
    {
        ProductName = dto.ProductName,
        Price = dto.Price,
        Stock = dto.Stock
    };

    context.AddAsync(product);
    context.SaveChangesAsync();

    return Results.Ok(Result<string>.Succeed("Ürün kaydý oluþturuldu"));
});

using (var scoped=app.Services.CreateScope())
{
    var srv = scoped.ServiceProvider;
    var context= srv.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}
app.Run();
