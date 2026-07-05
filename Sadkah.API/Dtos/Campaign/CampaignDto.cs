using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.API.Dtos.Campaign
{
    public class CampaignDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime Deadline { get; set; }
        public CampaignStatus Status { get; set; }
        public bool IsVerified { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public List<DonationDto> Donations { get; set; } = new ();
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string Barangay { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public List<DonationMethodDto> DonationMethods { get; set; } = new();
    }
}