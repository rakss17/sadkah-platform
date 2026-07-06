using Tesseract;

public class OcrService : IOcrService
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".bmp", ".tif", ".tiff"];
    private static readonly string TessDataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");

    public Task<string> ExtractTextAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Image is required.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new ArgumentException($"Unsupported file type '{extension}'. Allowed types: {string.Join(", ", AllowedExtensions)}.");

        return Task.Run(() =>
        {
            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);

            using var engine = new TesseractEngine(TessDataPath, "eng", EngineMode.Default);
            using var image = Pix.LoadFromMemory(memoryStream.ToArray());
            using var page = engine.Process(image);

            return page.GetText().Trim();
        });
    }
}
