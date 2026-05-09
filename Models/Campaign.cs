using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.Backend.Models
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
        public List<Donation> Donations { get; set; } = new List<Donation>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ArchivedAt { get; set; }
    }
}