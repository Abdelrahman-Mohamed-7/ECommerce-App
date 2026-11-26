using ECommerce.API.Data;
using ECommerce.API.DTOs;
using ECommerce.API.Models;
using ECommerce.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services.Implementations
{
    public class ProductService(AppDbContext context) : IProductService
    {
        public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParameters parameters)
        {
            var query = context.Products.Include(p => p.Category).AsNoTracking().Where(p => p.IsActive).AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var term = parameters.SearchTerm.Trim();
                query = query.Where(p => p.Name.Contains(term) || (p.Description != null && p.Description.Contains(term)));
            }

            if (parameters.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == parameters.CategoryId.Value);

            if (parameters.MinPrice.HasValue)
                query = query.Where(p => p.Price >= parameters.MinPrice.Value);

            if (parameters.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= parameters.MaxPrice.Value);

            query = (parameters.SortBy?.ToLower(), parameters.SortOrder?.ToLower()) switch
            {
                ("price", "desc") => query.OrderByDescending(p => p.Price),
                ("price", _) => query.OrderBy(p => p.Price),
                ("name", "desc") => query.OrderByDescending(p => p.Name),
                ("name", _) => query.OrderBy(p => p.Name),
                ("createdat", "desc") => query.OrderByDescending(p => p.CreatedAt),
                ("createdat", _) => query.OrderBy(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Id)
            };

            var totalCount = await query.CountAsync();
            var items = await query.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).Select(p => MapToDto(p)).ToListAsync();

            return new PagedResult<ProductDto> { Items = items, TotalCount = totalCount, PageNumber = parameters.PageNumber, PageSize = parameters.PageSize };
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await context.Products.Include(p => p.Category).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            return product == null ? null : MapToDto(product);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            if (!await context.Categories.AnyAsync(c => c.Id == dto.CategoryId))
                throw new ArgumentException($"Category with Id {dto.CategoryId} does not exist.");

            var product = new Product { Name = dto.Name, Description = dto.Description, Price = dto.Price, ImageUrl = dto.ImageUrl, StockQuantity = dto.StockQuantity, CategoryId = dto.CategoryId, IsActive = true, CreatedAt = DateTime.UtcNow };

            context.Products.Add(product);
            await context.SaveChangesAsync();
            await context.Entry(product).Reference(p => p.Category).LoadAsync();

            return MapToDto(product);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return null;

            if (!await context.Categories.AnyAsync(c => c.Id == dto.CategoryId))
                throw new ArgumentException($"Category with Id {dto.CategoryId} does not exist.");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.ImageUrl = dto.ImageUrl;
            product.StockQuantity = dto.StockQuantity;
            product.IsActive = dto.IsActive;
            product.CategoryId = dto.CategoryId;

            await context.SaveChangesAsync();
            await context.Entry(product).Reference(p => p.Category).LoadAsync();

            return MapToDto(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null) return false;

            product.IsActive = false; // Soft delete
            await context.SaveChangesAsync();
            return true;
        }

        private static ProductDto MapToDto(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name
        };
    }
}