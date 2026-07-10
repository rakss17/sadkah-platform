using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.API.Dtos.Donation
{
    public class DonationDto
    {
        public Guid Id { get; set; }
        public string DonorId { get; set; } = string.Empty;
        public string DonorName { get; set; } = string.Empty;
        public Guid CampaignId { get; set; }
        public decimal Amount { get; set; }
        public bool IsAnonymous { get; set; }
        public string PaymentReference { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string ReceiptImageUrl { get; set; } = string.Empty;
        public string ReceiptPublicId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

    }
}