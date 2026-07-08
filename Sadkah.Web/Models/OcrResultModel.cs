namespace Sadkah.Web.Models
{
    public class OcrResultModel
    {
        public decimal? Amount { get; set; }
        public string? ReferenceNumber { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
