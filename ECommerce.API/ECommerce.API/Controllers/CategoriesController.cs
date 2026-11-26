using ECommerce.API.DTOs;
using ECommerce.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories() => Ok(await categoryService.GetAllCategoriesAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await categoryService.GetCategoryByIdAsync(id);
            return category == null ? NotFound(new { message = $"Category with id {id} not found." }) : Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CategoryDto dto)
        {
            try
            {
                var created = await categoryService.CreateCategoryAsync(dto);
                return CreatedAtAction(nameof(GetCategory), new { id = created.Id }, created);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, [FromBody] CategoryDto dto)
        {
            var updated = await categoryService.UpdateCategoryAsync(id, dto);
            return updated == null ? NotFound(new { message = $"Category with id {id} not found." }) : Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var deleted = await categoryService.DeleteCategoryAsync(id);
                return deleted ? NoContent() : NotFound(new { message = $"Category with id {id} not found." });
            }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}