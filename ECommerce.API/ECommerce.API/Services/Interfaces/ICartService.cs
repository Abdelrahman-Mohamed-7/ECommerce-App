using ECommerce.API.DTOs;

namespace ECommerce.API.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(string userId);
        Task<CartDto> AddToCartAsync(string userId, AddToCartDto dto);
        Task<CartDto?> UpdateCartItemAsync(string userId, int cartItemId, UpdateCartItemDto dto);
        Task<CartDto?> RemoveCartItemAsync(string userId, int cartItemId);
        Task ClearCartAsync(string userId);
    }
}