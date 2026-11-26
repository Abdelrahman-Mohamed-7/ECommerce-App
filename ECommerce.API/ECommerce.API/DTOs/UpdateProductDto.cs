namespace ECommerce.API.DTOs
{
    public class UpdateProductDto : CreateProductDto
    {
        public bool IsActive { get; set; }
    }
}