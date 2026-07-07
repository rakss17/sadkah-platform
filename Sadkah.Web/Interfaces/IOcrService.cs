namespace Sadkah.Web.Interfaces
{
    public interface IOcrService
    {
        Task<ServiceResult<OcrResultModel>> ExtractReceiptTextAsync(byte[] fileBytes, string fileName, string contentType);
    }
}
