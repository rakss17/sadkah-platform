using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

namespace Sadkah.Web.Models
{
    public class DonationModel
    {

        public const long MaxQrImageSizeBytes = 2 * 1024 * 1024;
        [Required]
        public Guid CampaignId { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter your donation amount manually.")]
        public decimal Amount { get; set; }
        [Required]
        public bool IsAnonymous { get; set; }
        [Required(ErrorMessage = "Please enter reference number mannually.")]
        public string PaymentReference { get; set; } = string.Empty;
        public string? Message { get; set; }
        [Required(ErrorMessage = "Please select donation method.")]
        public string Method { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please upload a receipt image.")]
        public IBrowserFile ReceiptImageFile { get; set; } = null!;
        public byte[] ReceiptImageBytes { get; set; } = [];
        public string? UploadError { get; set; }
    }
}