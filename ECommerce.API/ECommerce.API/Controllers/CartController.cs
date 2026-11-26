using ECommerce.API.DTOs;
using ECommerce.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
        private const string GuestUserId = "guest";

        [HttpGet]
        public async Task<ActionResult<CartDto>> GetCart() => Ok(await cartService.GetCartAsync(GuestUserId));

        [HttpPost("items")]
        public async Task<ActionResult<CartDto>> AddToCart([FromBody] AddToCartDto dto)
        {
            try { return Ok(await cartService.AddToCartAsync(GuestUserId, dto)); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("items/{itemId:int}")]
        public async Task<ActionResult<CartDto>> UpdateCartItem(int itemId, [FromBody] UpdateCartItemDto dto)
        {
            try
            {
                var cart = await cartService.UpdateCartItemAsync(GuestUserId, itemId, dto);
                return cart == null ? NotFound(new { message = "Cart item not found." }) : Ok(cart);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("items/{itemId:int}")]
        public async Task<ActionResult<CartDto>> RemoveCartItem(int itemId)
        {
            var cart = await cartService.RemoveCartItemAsync(GuestUserId, itemId);
            return cart == null ? NotFound(new { message = "Cart item not found." }) : Ok(cart);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            await cartService.ClearCartAsync(GuestUserId);
            return NoContent();
        }
    }
}