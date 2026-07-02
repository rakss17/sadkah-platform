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
        public List<Donation> Donations { get; set; } = new List<Donation>();
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
        public List<Campaign> Campaigns { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}