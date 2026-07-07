using System.Net.Http.Headers;

namespace Sadkah.Web.Services
{
    public sealed class OcrService : IOcrService
    {
        private readonly IApiClient apiClient;

        public OcrService(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<ServiceResult<OcrResultModel>> ExtractReceiptTextAsync(byte[] fileBytes, string fileName, string contentType)
        {
            return await apiClient.PostMultipartAsync<OcrResultModel>(
                "api/ocr/extract-receipt-text",
                () => BuildContent(fileBytes, fileName, contentType),
                requiresAuthentication: true);
        }

        private static MultipartFormDataContent BuildContent(byte[] fileBytes, string fileName, string contentType)
        {
            var content = new MultipartFormDataContent();

            var byteContent = new ByteArrayContent(fileBytes);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Add(byteContent, "file", fileName);

            return content;
        }
    }
}
