using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Forms;
using Sadkah.Web.Enums;

namespace Sadkah.Web.Models
{
    public class DonationMethodModel
    {
        public const long MaxQrImageSizeBytes = 2 * 1024 * 1024;

        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Method type is required.")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DonationMethodType? Type { get; set; }

        [Required(ErrorMessage = "Provider is required.")]
        public string Provider { get; set; } = string.Empty;

        [Required(ErrorMessage = "QR code image is required.")]
        public IBrowserFile? QrImageFile { get; set; }

        public byte[]? QrImageBytes { get; set; }

        public string? UploadError { get; set; }

        public Guid CampaignId { get; set; }
    }
}
