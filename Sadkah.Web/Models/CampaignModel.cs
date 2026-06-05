namespace Sadkah.Web.Models
{
    public class CampaignModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime Deadline { get; set; }
        public CampaignStatus Status { get; set; }
        public bool IsVerified { get; set; }
        public string OwnerName { get; set; } = string.Empty;
    }
}
