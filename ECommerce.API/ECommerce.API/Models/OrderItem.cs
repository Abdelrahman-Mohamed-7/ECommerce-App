namespace ECommerce.API.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty; // بنحفظ الاسم وقت الشراء
        public decimal UnitPrice { get; set; } // بنحفظ السعر وقت الشراء
        public int Quantity { get; set; }

        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}