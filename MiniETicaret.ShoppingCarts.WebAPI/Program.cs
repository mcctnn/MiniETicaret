using Microsoft.EntityFrameworkCore;
using MiniETicaret.ShoppingCarts.WebAPI.Context;
using MiniETicaret.ShoppingCarts.WebAPI.Dtos;
using MiniETicaret.ShoppingCarts.WebAPI.Models;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/getall", async (ApplicationDbContext context, IConfiguration configuration, CancellationToken token) =>
{
    List<ShoppingCart> shoppingCarts = await context.ShoppingCarts.ToListAsync(token);


    HttpClient httpClient = new();
    string productsEndpoint = $"http://{configuration.GetSection("HttpRequest:Products").Value}/getall";
    var message = await httpClient.GetAsync(productsEndpoint);

    Result<List<ProductDto>>? products = new();

    if (message.IsSuccessStatusCode)
    {
        products = await message.Content.ReadFromJsonAsync<Result<List<ProductDto>>>();

    }

    List<ShoppingCartDto> response = shoppingCarts.Select(s => new ShoppingCartDto()
    {
        Id = s.Id,
        ProductId = s.ProductId,
        Quantity = s.Quantity,
        ProductName = products!.Data!.First(p => p.ProductId == s.ProductId).ProductName,
        ProductPrice = products.Data!.First(p => p.ProductId == s.ProductId).Price
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

    await context.AddAsync(shoppingCart, token);
    await context.SaveChangesAsync(token);

    return Results.Ok(new Result<string>("Ürün sepeti baþarýyla eklendi"));
});

app.MapGet("/createOrder", async (ApplicationDbContext context, IConfiguration configuration, CancellationToken token) =>
{
    List<ShoppingCart> shoppingCarts = await context.ShoppingCarts.ToListAsync(token);


    HttpClient httpClient = new();
    string productsEndpoint = $"http://{configuration.GetSection("HttpRequest:Products").Value}/getall";
    var message = await httpClient.GetAsync(productsEndpoint);

    Result<List<ProductDto>>? products = new();

    if (message.IsSuccessStatusCode)
    {
        products = await message.Content.ReadFromJsonAsync<Result<List<ProductDto>>>();

    }

    List<CreateOrderDto> response = shoppingCarts.Select(s => new CreateOrderDto
    {
        ProductId = s.ProductId,
        Quantity = s.Quantity,
        Price = products!.Data!.First(p => p.ProductId == s.ProductId).Price
    }).ToList();

    string ordersEndpoint = $"http://{configuration.GetSection("HttpRequest:Orders").Value}/create";

    string json = JsonSerializer.Serialize(response);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var orderMessage = await httpClient.PostAsync(ordersEndpoint, content);

    if (orderMessage.IsSuccessStatusCode)
    {
        List<ChangeProductStockDto> ChangeProductStockDtos = shoppingCarts.Select(s => new ChangeProductStockDto
        (s.ProductId,s.Quantity)).ToList();

        string changeProductStockEndpoint = $"http://{configuration.GetSection("HttpRequest:Products").Value}/change-product-stock";

        string productjson = JsonSerializer.Serialize(response);
        var productscontent = new StringContent(productjson, Encoding.UTF8, "application/json");
        

        await httpClient.PostAsync(changeProductStockEndpoint, productscontent);


        context.RemoveRange(shoppingCarts);
        await context.SaveChangesAsync(token);
    }

    return Results.Ok(new Result<string>("Sipariþ baþarýyla oluþturuldu"));
});


using (var scoped = app.Services.CreateScope())
{
    var srv = scoped.ServiceProvider;
    var context = srv.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
