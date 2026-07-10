using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.API.Models
{
    public class Donation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string DonorId { get; set; } = string.Empty;
        public User? Donor { get; set; }
        public Guid CampaignId { get; set; }
        public Campaign Campaign { get; set; } = null!;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public bool IsAnonymous { get; set; } = false;
        public string PaymentReference { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string ReceiptImageUrl { get; set; } = string.Empty;
        public string ReceiptImagePublicId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
    }
}