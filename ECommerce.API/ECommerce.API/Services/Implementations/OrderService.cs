using ECommerce.API.Data;
using ECommerce.API.DTOs;
using ECommerce.API.Models;
using ECommerce.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services.Implementations
{
    public class OrderService(AppDbContext context) : IOrderService
    {
        public async Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto dto)
        {
            var cart = await context.Carts.Include(c => c.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null || !cart.Items.Any()) throw new ArgumentException("Cart is empty.");

            var order = new Order
            {
                UserId = userId,
                ShippingAddress = dto.ShippingAddress,
                PaymentMethod = dto.PaymentMethod,
                TotalAmount = cart.Items.Sum(i => i.UnitPrice * i.Quantity),
                OrderDate = DateTime.UtcNow,
                Status = "Pending"
            };

            foreach (var item in cart.Items)
            {
                if (item.Product != null && item.Product.StockQuantity < item.Quantity) throw new ArgumentException($"Product {item.Product.Name} is out of stock.");

                order.Items.Add(new OrderItem { ProductId = item.ProductId, ProductName = item.Product?.Name ?? "Unknown", UnitPrice = item.UnitPrice, Quantity = item.Quantity });
                if (item.Product != null) item.Product.StockQuantity -= item.Quantity;
            }

            context.Orders.Add(order);
            context.CartItems.RemoveRange(cart.Items);
            await context.SaveChangesAsync();

            return MapToDto(order);
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await context.Orders.Include(o => o.Items).Where(o => o.UserId == userId).OrderByDescending(o => o.OrderDate).ToListAsync();
            return orders.Select(MapToDto);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id, string userId)
        {
            var order = await context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
            return order == null ? null : MapToDto(order);
        }

        private static OrderDto MapToDto(Order o) => new()
        {
            Id = o.Id,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            Status = o.Status,
            ShippingAddress = o.ShippingAddress,
            PaymentMethod = o.PaymentMethod,
            Items = o.Items.Select(i => new OrderItemDto { ProductId = i.ProductId, ProductName = i.ProductName, UnitPrice = i.UnitPrice, Quantity = i.Quantity }).ToList()
        };
    }
}