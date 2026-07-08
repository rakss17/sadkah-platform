using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.Web.Models
{
    public class DonationMethodResult
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string QrImageUrl { get; set; } = string.Empty;
        public string QrImagePublicId { get; set; } = string.Empty;
        public Guid CampaignId { get; set; }
    }
}