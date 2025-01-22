using Microsoft.EntityFrameworkCore;
using MiniETicaret.ShoppingCarts.WebAPI.Context;
using MiniETicaret.ShoppingCarts.WebAPI.Dtos;
using MiniETicaret.ShoppingCarts.WebAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/getall", async (ApplicationDbContext context, CancellationToken token) =>
{
    List<ShoppingCart> shoppingCarts = await context.ShoppingCarts.ToListAsync(token);
    
    HttpClient client = new HttpClient();

    var message = await client.GetAsync("http://localhost:5001/getall");

    Result<List<ProductDto>>? products = new ();

    if (message.IsSuccessStatusCode)
    {
        products =await message.Content.ReadFromJsonAsync<Result<List<ProductDto>>>();

    }

    List<ShoppingCartDto> response = shoppingCarts.Select(s => new ShoppingCartDto()
    {
        ShoppingCartId=s.ShoppingCartId,
        ProductId=s.ProductId,
        Quantity=s.Quantity,
        ProductName= products!.Data!.First(p=>p.ProductId==s.ProductId).ProductName,
        ProductPrice= products.Data!.First(p => p.ProductId == s.ProductId).Price
    }).ToList();

    return new Result<List<ShoppingCartDto>>(response);
});

app.MapPost("/create", async (CreateShoppingCartDto dto, ApplicationDbContext context, CancellationToken token) =>
{
    ShoppingCart shoppingCart = new ShoppingCart()
    {
        ProductId = dto.ProductId,
        Quantity = dto.Quantity
    };

    await context.AddAsync(shoppingCart,token);
    await context.SaveChangesAsync(token);

    return Results.Ok(new Result<string>("Ürün sepeti baþarýyla eklendi"));
});

using (var scoped = app.Services.CreateScope())
{
    var srv = scoped.ServiceProvider;
    var context = srv.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
