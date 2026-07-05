
using System.ComponentModel.DataAnnotations;

namespace Sadkah.API.Dtos.Campaign
{
       public class DonationMethodDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string QrImageUrl { get; set; } = string.Empty;
        public string QrImagePublicId { get; set; } = string.Empty;
        public Guid CampaignId { get; set; }
    }
}