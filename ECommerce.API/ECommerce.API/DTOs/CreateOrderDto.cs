namespace ECommerce.API.DTOs
{
    public class CreateOrderDto
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "COD";
    }
}