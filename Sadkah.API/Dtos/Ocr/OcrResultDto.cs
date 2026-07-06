namespace Sadkah.API.Dtos.Ocr
{
    public class OcrResultDto
    {
        public decimal? Amount { get; set; }
        public string? ReferenceNumber { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
