namespace MiniETicaret.ShoppingCarts.WebAPI.Models
{
    public sealed class ShoppingCart
    {
        public ShoppingCart()
        {
            ShoppingCartId = Guid.NewGuid();
        }
        public Guid ShoppingCartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
