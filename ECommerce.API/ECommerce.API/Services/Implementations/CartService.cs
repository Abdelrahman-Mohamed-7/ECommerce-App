using ECommerce.API.Data;
using ECommerce.API.DTOs;
using ECommerce.API.Models;
using ECommerce.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services.Implementations
{
    public class CartService(AppDbContext context) : ICartService
    {
        public async Task<CartDto> GetCartAsync(string userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            return MapToDto(cart);
        }

        public async Task<CartDto> AddToCartAsync(string userId, AddToCartDto dto)
        {
            var product = await context.Products.FindAsync(dto.ProductId) ?? throw new ArgumentException($"Product not found.");
            if (!product.IsActive) throw new ArgumentException("Product is not available.");
            if (dto.Quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
            if (product.StockQuantity < dto.Quantity) throw new ArgumentException($"Only {product.StockQuantity} items in stock.");

            var cart = await GetOrCreateCartAsync(userId);
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                if (existingItem.Quantity > product.StockQuantity) throw new ArgumentException($"Only {product.StockQuantity} items in stock.");
            }
            else
            {
                cart.Items.Add(new CartItem { ProductId = dto.ProductId, Quantity = dto.Quantity, UnitPrice = product.Price });
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            await ReloadCartItemsAsync(cart);
            return MapToDto(cart);
        }

        public async Task<CartDto?> UpdateCartItemAsync(string userId, int cartItemId, UpdateCartItemDto dto)
        {
            var cart = await context.Carts.Include(c => c.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null) return null;

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null) return null;

            if (dto.Quantity <= 0)
            {
                cart.Items.Remove(item);
                context.CartItems.Remove(item);
            }
            else
            {
                if (item.Product != null && dto.Quantity > item.Product.StockQuantity) throw new ArgumentException($"Only {item.Product.StockQuantity} items in stock.");
                item.Quantity = dto.Quantity;
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return MapToDto(cart);
        }

        public async Task<CartDto?> RemoveCartItemAsync(string userId, int cartItemId)
        {
            var cart = await context.Carts.Include(c => c.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null) return null;

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null) return null;

            context.CartItems.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            await ReloadCartItemsAsync(cart);
            return MapToDto(cart);
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null) return;

            context.CartItems.RemoveRange(cart.Items);
            cart.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }

        private async Task<Cart> GetOrCreateCartAsync(string userId)
        {
            var cart = await context.Carts.Include(c => c.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                context.Carts.Add(cart);
                await context.SaveChangesAsync();
            }
            return cart;
        }

        private async Task ReloadCartItemsAsync(Cart cart)
        {
            await context.Entry(cart).Collection(c => c.Items).Query().Include(i => i.Product).LoadAsync();
        }

        private static CartDto MapToDto(Cart cart) => new()
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Items = cart.Items.Select(i => new CartItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? string.Empty,
                ProductImageUrl = i.Product?.ImageUrl,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}