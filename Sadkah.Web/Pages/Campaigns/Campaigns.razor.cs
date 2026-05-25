namespace Sadkah.Web.Pages.Campaigns
{
    public partial class Campaigns
    {
        private CampaignFilter selectedFilter = CampaignFilter.All;

        private readonly IReadOnlyList<CampaignCard> campaigns =
        [
            new(
                "Emergency Medical Support",
                "Healthcare",
                "Cagayan de Oro",
                12,
                84500,
                120000,
                true,
                false)
        ];

        private IEnumerable<CampaignCard> FilteredCampaigns => selectedFilter switch
        {
            CampaignFilter.MyCampaigns => campaigns.Where(campaign => campaign.IsMine),
            _ => campaigns
        };

        private bool IsSelected(CampaignFilter filter) => selectedFilter == filter;

        private void SelectFilter(CampaignFilter filter)
        {
            selectedFilter = filter;
        }

        private string GetFilterTabClass(CampaignFilter filter)
        {
            return IsSelected(filter)
                ? "filter-tab-button filter-tab-button--active"
                : "filter-tab-button";
        }

        private enum CampaignFilter
        {
            All,
            MyCampaigns
        }

        private sealed record CampaignCard(
            string Title,
            string Category,
            string Location,
            int DaysLeft,
            decimal RaisedAmount,
            decimal GoalAmount,
            bool IsVerified,
            bool IsMine);
    }
}
