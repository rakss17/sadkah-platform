using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.API.Dtos.Donation
{
    public class CreateDonationRequestDto
    {
        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "DonorId must be between 5 and 100 characters.")]
        public string DonorId { get; set; } = string.Empty;
        [Required]
        public Guid CampaignId { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive decimal.")]
        public decimal Amount { get; set; }
        [Required]
        public bool? IsAnonymous { get; set; }
        [Required]
        public string PaymentReference { get; set; } = string.Empty;
        [Required]
        public string Method { get; set; } = string.Empty;
        [Required(ErrorMessage = "Receipt image is required.")]
        public IFormFile ReceiptImageFile { get; set; } = null!;
    }
}