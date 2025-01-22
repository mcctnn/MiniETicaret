namespace MiniETicaret.ShoppingCarts.WebAPI.Dtos
{
    public sealed record ProductDto(Guid ProductId,string ProductName,decimal Price,int Stock);
}
