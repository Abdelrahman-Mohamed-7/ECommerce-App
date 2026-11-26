using ECommerce.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController(IImageService imageService) : ControllerBase
    {
        [HttpPost("upload")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try { return Ok(new { imageUrl = await imageService.SaveImageAsync(file) }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string url) => await imageService.DeleteImageAsync(url) ? NoContent() : NotFound();
    }
}