using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.API.Models
{
    public class Campaign
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string OwnerId { get; set; } = string.Empty;
        public User Owner { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal TargetAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentAmount { get; set; }
        public DateTime Deadline { get; set; } = DateTime.UtcNow;
        public CampaignStatus Status { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsArchived { get; set; } = false;
        public Guid CategoryId { get; set; }
        public CampaignCategory Category { get; set; } = null!;
        public ICollection<Donation> Donations { get; set; } = [];
        public ICollection<DonationMethod> DonationMethods { get; set; } = [];
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string Barangay { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ArchivedAt { get; set; }
    }

    public class CampaignCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public ICollection<Campaign> Campaigns { get; set; } = [];
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class DonationMethod
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string QrCodeImageUrl { get; set; } = string.Empty;
        public string QrCodeImagePublicId { get; set; } = string.Empty;
        public Guid CampaignId { get; set; }
        public Campaign Campaign { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}