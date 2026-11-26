using ECommerce.API.DTOs;
using ECommerce.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrderService orderService) : ControllerBase
    {
        private const string GuestUserId = "guest";

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto dto)
        {
            try { return Ok(await orderService.CreateOrderAsync(GuestUserId, dto)); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders() => Ok(await orderService.GetUserOrdersAsync(GuestUserId));
    }
}