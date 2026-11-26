using ECommerce.API.Data;
using ECommerce.API.DTOs;
using ECommerce.API.Models;
using ECommerce.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services.Implementations
{
    public class CategoryService(AppDbContext context) : ICategoryService
    {
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            return await context.Categories.AsNoTracking().OrderBy(c => c.Name)
                .Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description }).ToListAsync();
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var c = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return c == null ? null : new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto dto)
        {
            if (await context.Categories.AnyAsync(c => c.Name == dto.Name))
                throw new ArgumentException($"Category '{dto.Name}' already exists.");

            var category = new Category { Name = dto.Name, Description = dto.Description, CreatedAt = DateTime.UtcNow };
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            dto.Id = category.Id;
            return dto;
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int id, CategoryDto dto)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null) return null;

            category.Name = dto.Name;
            category.Description = dto.Description;
            await context.SaveChangesAsync();

            return new CategoryDto { Id = category.Id, Name = category.Name, Description = category.Description };
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return false;
            if (category.Products.Any()) throw new InvalidOperationException("Cannot delete category with existing products.");

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return true;
        }
    }
}