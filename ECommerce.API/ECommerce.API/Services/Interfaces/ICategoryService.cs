using ECommerce.API.DTOs;

namespace ECommerce.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CategoryDto dto);
        Task<CategoryDto?> UpdateCategoryAsync(int id, CategoryDto dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}