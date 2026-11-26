using ECommerce.API.DTOs;
using ECommerce.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts([FromQuery] ProductQueryParameters parameters)
            => Ok(await productService.GetProductsAsync(parameters));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await productService.GetProductByIdAsync(id);
            return product == null ? NotFound(new { message = $"Product with id {id} not found." }) : Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto dto)
        {
            try
            {
                var created = await productService.CreateProductAsync(dto);
                return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            try
            {
                var updated = await productService.UpdateProductAsync(id, dto);
                return updated == null ? NotFound(new { message = $"Product with id {id} not found." }) : Ok(updated);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await productService.DeleteProductAsync(id);
            return deleted ? NoContent() : NotFound(new { message = $"Product with id {id} not found." });
        }
    }
}