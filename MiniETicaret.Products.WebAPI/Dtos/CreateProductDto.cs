namespace MiniETicaret.Products.WebAPI.Dtos
{
    public sealed record CreateProductDto(string ProductName,decimal Price,int Stock);
}
