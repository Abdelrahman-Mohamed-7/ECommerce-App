using ECommerce.API.DTOs;

namespace ECommerce.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParameters parameters);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(int id);
    }
}