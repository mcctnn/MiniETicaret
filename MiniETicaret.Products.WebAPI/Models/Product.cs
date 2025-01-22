namespace MiniETicaret.Products.WebAPI.Models
{
    public sealed class Product
    {
        public Product()
        {
            ProductId = Guid.NewGuid();
        }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
