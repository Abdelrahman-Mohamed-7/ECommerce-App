namespace ECommerce.API.Services.Interfaces
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile file);
        Task<bool> DeleteImageAsync(string imageUrl);
    }
}