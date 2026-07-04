using System.ComponentModel.DataAnnotations;
using Sadkah.Web.Enums;

namespace Sadkah.Web.Models
{
    public class DonationMethodModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Method type is required.")]
        public DonationMethodType? Type { get; set; }

        [Required(ErrorMessage = "Provider is required.")]
        public string Provider { get; set; } = string.Empty;

        [Required(ErrorMessage = "QR code image is required.")]
        public string? QrImageFileName { get; set; }

        [Required(ErrorMessage = "QR code image is required.")]
        public string? QrImageDataUrl { get; set; }

        public string? UploadError { get; set; }
    }
}
