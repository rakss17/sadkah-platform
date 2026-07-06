namespace Sadkah.API.Interfaces
{
    public interface IOcrService
    {
        Task<string> ExtractTextAsync(IFormFile file);
    }
}
