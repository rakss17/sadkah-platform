using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<ImageUploadResult> UploadImageAsync(IFormFile file, string directory)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Image is required.");

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = directory
        };

        return await _cloudinary.UploadAsync(uploadParams);
    }
}