using CloudinaryDotNet.Actions;

namespace Sadkah.API.Interfaces
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file, string directory);
    }
}