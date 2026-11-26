namespace ECommerce.API.DTOs
{
    public class CartDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public List<CartItemDto> Items { get; set; } = [];
        public decimal Total => Items.Sum(i => i.SubTotal);
        public int TotalItems => Items.Sum(i => i.Quantity);
    }
}