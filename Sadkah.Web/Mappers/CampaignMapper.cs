namespace Sadkah.Web.Mappers
{
    internal static class CampaignMapper
    {
        public static CampaignSummary ToSummary(this CampaignModel campaign, string? currentUserFullName)
        {
            return new CampaignSummary(
                campaign.Title,
                campaign.CategoryName ?? "Uncategorized",
                campaign.OwnerName,
                GetDaysLeft(campaign.Deadline),
                campaign.CurrentAmount,
                campaign.TargetAmount,
                campaign.IsVerified,
                IsCurrentUsersCampaign(campaign.OwnerName, currentUserFullName));
        }

        private static bool IsCurrentUsersCampaign(string ownerName, string? currentUserFullName)
        {
            return !string.IsNullOrWhiteSpace(currentUserFullName)
                && string.Equals(ownerName, currentUserFullName, StringComparison.OrdinalIgnoreCase);
        }

        private static int GetDaysLeft(DateTime? deadline)
        {
            if (!deadline.HasValue) return 0;
            return Math.Max(0, (deadline.Value.Date - DateTime.Today).Days);
        }

        private static string GetStatusLabel(CampaignStatus status)
        {
            return status switch
            {
                CampaignStatus.Draft => "Draft",
                CampaignStatus.Active => "Active",
                CampaignStatus.Completed => "Completed",
                CampaignStatus.Cancelled => "Cancelled",
                _ => "Campaign"
            };
        }
    }
}
