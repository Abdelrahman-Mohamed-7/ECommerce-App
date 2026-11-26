namespace ECommerce.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "COD"; // Cash on Delivery

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}